using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine.Client
{
	public class Client
	{
		#region Fields

		private INetworkConnection serverNetworkConnection;
		public SortedList<int, StateSnapshot> ReceivedStateSnapshots = new SortedList<int, StateSnapshot>(64);

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
		/// <summary>Gets or sets the maximum number of ticks that the client can extrapolate for (in the event of packet loss).</summary>
		public int MaxExtrapolationTicks { get; set; } = 10;

		/// <summary>Gets the current frame tick of the client (which may or may not be ahead of the tick that is currently being rendered).</summary>
		public int FrameTick { get; private set; }
		/// <summary>Gets the last server tick that was actually received from the server.</summary>
		public int LastestReceivedServerTick { get; private set; } = -1;

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

		public void Update()
		{
			this.FrameTick++;

			byte[] packet;
			while ((packet = this.serverNetworkConnection.GetNextIncomingPacket()) != null)
			{
				StateSnapshot stateSnapshot = StateSnapshot.DeserializePacket(packet);
				this.ReceivedStateSnapshots.Add(stateSnapshot.FrameTick, stateSnapshot);

				if (this.LastestReceivedServerTick < 0)
				{
					this.FrameTick = stateSnapshot.FrameTick;
				}

				this.LastestReceivedServerTick = stateSnapshot.FrameTick;
			}

			if (this.ShouldInterpolate)
			{
				// Todo: this delay should be: frame - (updaterate + 1/2latency) * 0.1fudgefactor
				int renderedFrameTick = this.FrameTick - 10;

				if (!this.HasInterpolationStarted)
				{
					StateSnapshot interpolationStartState = null;
					StateSnapshot interpolationEndState = null;
					foreach (var kvp in this.ReceivedStateSnapshots)
					{
						StateSnapshot stateSnapshot = kvp.Value;
						// Todo: these should be more intelligent and grab the closest packets in either direction
						// Todo: make sure we can grab the end packet on the last frame of the interpolation range
						if (stateSnapshot.FrameTick <= renderedFrameTick)
						{
							interpolationStartState = stateSnapshot;
						}
						if (stateSnapshot.FrameTick > renderedFrameTick)
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
				else
				{
					if (renderedFrameTick > this.InterpolationEndState.FrameTick)
					{
						// Find the next closest state snapshot to start interpolating to
						StateSnapshot closestSnapshot = null;
						foreach (var kvp in this.ReceivedStateSnapshots)
						{
							StateSnapshot stateSnapshot = kvp.Value;
							if (stateSnapshot.FrameTick >= renderedFrameTick && (closestSnapshot == null || closestSnapshot.FrameTick > stateSnapshot.FrameTick))
							{
								closestSnapshot = stateSnapshot;
							}
						}

						if (closestSnapshot != null)
						{
							this.InterpolationStartState = this.RenderedState;
							this.InterpolationEndState = closestSnapshot;
						}
					}

					if (renderedFrameTick - this.InterpolationEndState.FrameTick < this.MaxExtrapolationTicks)
					{
						this.RenderedState = StateSnapshot.Interpolate(this.InterpolationStartState, this.InterpolationEndState, renderedFrameTick);
						if (this.InterpolationEndState.FrameTick < renderedFrameTick) { this.NumberOfExtrapolatedFrames++; }
					}
					else
					{
						// Even though we aren't changing the state of the rendered frame, we still need to update its tick number since we are actively
						// deciding it is current. If we don't then when we go to interpolate away from this frame then it could be far in the past leading
						// to jumpy transitions.
						this.RenderedState.FrameTick = renderedFrameTick;
						this.NumberOfNoInterpolationFrames++;
					}
				}
			}
			else
			{
				if (this.ReceivedStateSnapshots.Any())
				{
					this.RenderedState = this.ReceivedStateSnapshots.Last().Value;
				}
			}
		}

		#endregion Methods
	}
}
