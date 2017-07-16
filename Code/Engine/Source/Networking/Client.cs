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

		public bool ShouldInterpolate { get; set; } = true;

		public int FrameTick { get; private set; }
		public int LastestReceivedServerTick { get; private set; } = -1;

		public bool IsInterpolationValid { get { return (this.InterpolationStartState != null && this.InterpolationEndState != null); } }
		public int NumberOfInvalidInterpolations { get; private set; }

		public StateSnapshot InterpolationStartState { get; private set; }
		public StateSnapshot InterpolationEndState { get; private set; }
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
					this.NumberOfInvalidInterpolations = 0;
				}

				this.LastestReceivedServerTick = stateSnapshot.FrameTick;
			}

			if (this.ShouldInterpolate)
			{
				// Todo: this delay should be: frame - (updaterate + 1/2latency) * 0.1fudgefactor
				int renderedFrameTick = this.FrameTick - 10;

				if (!this.IsInterpolationValid)
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

					this.RenderedState = StateSnapshot.Interpolate(this.InterpolationStartState, this.InterpolationEndState, renderedFrameTick);
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
