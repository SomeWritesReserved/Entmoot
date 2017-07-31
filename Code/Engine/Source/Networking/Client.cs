using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public class Client : ISystem
	{
		#region Fields

		private INetworkConnection serverNetworkConnection;
		public SortedList<int, StateSnapshot> ReceivedStateSnapshots = new SortedList<int, StateSnapshot>(64);
		public List<ClientCommand> SentClientCommands = new List<ClientCommand>(64);
		private SortedList<int, Vector3> predictedPositions = new SortedList<int, Vector3>();

		#endregion Fields

		#region Constructors

		public Client(INetworkConnection serverNetworkConnection)
		{
			this.serverNetworkConnection = serverNetworkConnection;
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets or sets whether the client should interpolate sent server state for a smoother rendered experience.</summary>
		public bool ShouldInterpolate { get; set; } = true;
		/// <summary>Gets or sets whether to perform client-side prediction on the user's input.</summary>
		public bool ShouldPredictInput { get; set; } = true;
		/// <summary>Gets or sets the delay in frames that the client will use to render interpolated data. This should be at least as large as the server's update rate plus client latency.</summary>
		public int InterpolationRenderDelay { get; set; } = 10;
		/// <summary>Gets or sets the maximum number of ticks that the client can extrapolate for (in the event of packet loss).</summary>
		public int MaxExtrapolationTicks { get; set; } = 10;

		/// <summary>Gets the current frame tick of the client (which may or may not be ahead of the tick that is currently being rendered).</summary>
		public int FrameTick { get; private set; }
		/// <summary>Gets the last server tick that was actually received from the server.</summary>
		public int LatestReceivedServerTick { get; private set; } = -1;
		/// <summary>Gets the last client tick that was acknowledged by the server.</summary>
		public int LatestTickAcknowledgedByServer { get; private set; } = -1;

		/// <summary>Gets whether or not the client has enough data from the server to start interpolation and that indeed interpolation has begun.</summary>
		public bool HasInterpolationStarted { get { return (this.InterpolationStartState != null && this.InterpolationEndState != null); } }
		/// <summary>Gets the number of total frames (over the course of the entire session) that had to be extrapolated (instead of interpolated) due to packet loss.</summary>
		public int NumberOfExtrapolatedFrames { get; private set; }
		/// <summary>Gets the number of total frames (over the course of the entire session) that had no interpolation or extrapolation due to severe packet loss and <see cref="MaxExtrapolationTicks"/>.</summary>
		public int NumberOfNoInterpolationFrames { get; private set; }

		/// <summary>Gets the <see cref="StateSnapshot"/> that is currently being used as the starting interpolation tick (where we are coming from).</summary>
		public StateSnapshot InterpolationStartState { get; private set; }
		/// <summary>Gets the <see cref="StateSnapshot"/> that is currently being used as the ending interpolation tick (where we are going to).</summary>
		public StateSnapshot InterpolationEndState { get; private set; }
		/// <summary>Gets the <see cref="StateSnapshot"/> that is currently the actively rendered state.</summary>
		public StateSnapshot RenderedState { get; private set; }

		#endregion Properties

		#region Methods

		public void Update(CommandKeys activeCommandKeys)
		{
			if (this.LatestReceivedServerTick >= 0)
			{
				// Only tick the client if we started getting info from the server
				this.FrameTick++;
			}

			byte[] packet;
			while ((packet = this.serverNetworkConnection.GetNextIncomingPacket()) != null)
			{
				StateSnapshot stateSnapshot = StateSnapshot.DeserializePacket(packet);
				this.ReceivedStateSnapshots.Add(stateSnapshot.ServerFrameTick, stateSnapshot);

				if (this.LatestReceivedServerTick < 0)
				{
					// Sync our ticks with the server's ticks once we start getting some data from the server
					this.FrameTick = stateSnapshot.ServerFrameTick;
				}

				if (this.LatestReceivedServerTick < stateSnapshot.ServerFrameTick)
				{
					this.LatestReceivedServerTick = stateSnapshot.ServerFrameTick;
				}
			}

			this.SentClientCommands.Add(new ClientCommand()
			{
				ClientFrameTick = this.FrameTick,
				AcknowledgedServerTick = this.LatestReceivedServerTick,
				CommandKeys = activeCommandKeys,
			});
			this.serverNetworkConnection.SendPacket(ClientCommand.SerializeCommands(this.SentClientCommands.Where((cmd) => cmd.ClientFrameTick > this.LatestTickAcknowledgedByServer).ToArray()));

			this.setupRenderSnapshot();

			// Client side prediction
			if (this.ShouldPredictInput && this.RenderedState != null)
			{
				Entity predictionEnt = new Entity() { Position = this.InterpolationEndState.Entities[0].Position };
				foreach (ClientCommand clientCommandNotAckedByServer in this.SentClientCommands.Where((cmd) => cmd.ClientFrameTick > this.InterpolationEndState.AcknowledgedClientTick))
				{
					// Reapply all the commands we've sent that the server hasn't processed yet to get us back to where we predicted we should be, starting
					// from where the server last gave us an authoritative response
					clientCommandNotAckedByServer.RunOnEntity(predictionEnt);
					clientCommandNotAckedByServer.RunOnEntity(this.RenderedState.Entities[0]);
				}

				this.predictedPositions.Add(this.FrameTick, predictionEnt.Position);
			}
		}

		private void setupRenderSnapshot()
		{
			int renderedFrameTick = this.FrameTick - this.InterpolationRenderDelay;

			if (!this.HasInterpolationStarted)
			{
				// We haven't received enough data from the server yet to start interpolation rendering,
				// so keep polling until we get enough data, once we have enough data we can begin rendering.
				StateSnapshot interpolationStartState = null;
				StateSnapshot interpolationEndState = null;
				foreach (var kvp in this.ReceivedStateSnapshots)
				{
					StateSnapshot stateSnapshot = kvp.Value;
					// Todo: these should be more intelligent and grab the closest packets in either direction
					// Todo: make sure we can grab the end packet on the last frame of the interpolation range
					if (stateSnapshot.ServerFrameTick <= renderedFrameTick)
					{
						interpolationStartState = stateSnapshot;
					}
					if (stateSnapshot.ServerFrameTick > renderedFrameTick)
					{
						interpolationEndState = stateSnapshot;
						break;
					}
				}
				if (interpolationStartState != null && interpolationEndState != null)
				{
					this.InterpolationStartState = interpolationStartState;
					this.InterpolationEndState = interpolationEndState;
				}
			}

			// Check to see if we still can't interpolate after going through the latest receieved updates
			if (!this.HasInterpolationStarted) { return; }

			if (renderedFrameTick > this.InterpolationEndState.ServerFrameTick)
			{
				// Find the next closest state snapshot to start interpolating to
				StateSnapshot closestSnapshot = null;
				foreach (var kvp in this.ReceivedStateSnapshots)
				{
					StateSnapshot stateSnapshot = kvp.Value;
					if (stateSnapshot.ServerFrameTick >= renderedFrameTick && (closestSnapshot == null || closestSnapshot.ServerFrameTick > stateSnapshot.ServerFrameTick))
					{
						closestSnapshot = stateSnapshot;
					}
				}

				if (closestSnapshot != null)
				{
					this.InterpolationStartState = this.RenderedState;
					this.InterpolationEndState = closestSnapshot;

					if (this.ShouldPredictInput)
					{
						// Since the client's entity is predicted, don't interpolate it just snap it to the end position because
						// later we'll reapply all the commands we've issued since then to get back to where we are
						Vector3 truePosition = this.InterpolationEndState.Entities[0].Position;
						this.InterpolationStartState.Entities[0].Position = truePosition;
					}
				}
			}

			if (this.ShouldInterpolate)
			{
				// Clamp the interpolation frame tick to the maximum number of frames we are allowed to extrapolate, then render that
				int interpolationFrameTick = Math.Min(renderedFrameTick, this.InterpolationEndState.ServerFrameTick + this.MaxExtrapolationTicks);
				this.RenderedState = StateSnapshot.Interpolate(this.InterpolationStartState, this.InterpolationEndState, interpolationFrameTick);

				// Always make sure the rendered state has the correct frame tick, even if extrapolation was clamped
				// Todo: Should this happen all the time, even when not interpolating? Doing so breaks the unit tests
				this.RenderedState.ServerFrameTick = renderedFrameTick;

				if (interpolationFrameTick < renderedFrameTick) { this.NumberOfNoInterpolationFrames++; }
				else if (this.InterpolationEndState.ServerFrameTick < renderedFrameTick) { this.NumberOfExtrapolatedFrames++; }
			}
			else
			{
				// Even though we aren't interpolating, we still want to use whatever the user picked as the render delay so
				// use the end interpolation state and just snap to it
				this.RenderedState = this.InterpolationEndState;
			}
		}

		#endregion Methods
	}
}
