﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine.Client
{
	public class Client
	{
		#region Fields

		private int lastestReceivedServerPacket = -1;
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

		private int frameTick;
		public int FrameTick
		{
			get { return this.frameTick; }
		}

		private Entity[] entities = new Entity[0];
		public IList<Entity> Entities
		{
			get { return Array.AsReadOnly(this.entities); }
		}

		public bool ShouldInterpolate { get; set; } = true;
		public int RenderFrameTick { get; set; } = -8;
		public int InterpolatedStartTick { get; set; } = -1;
		public int InterpolatedEndTick { get; set; } = -1;
		public bool IsInterpolationValid { get; set; } = false;

		#endregion Properties

		#region Methods

		public void Update()
		{
			byte[] packet;
			while ((packet = this.serverNetworkConnection.GetNextIncomingPacket()) != null)
			{
				StateSnapshot stateSnapshot = StateSnapshot.DeserializePacket(packet);
				this.ReceivedStateSnapshots.Add(stateSnapshot.FrameTick, stateSnapshot);

				if (this.lastestReceivedServerPacket < 0)
				{
					// Debug: this is commented out since in the test app server/client are synced at zero so this just makes things confusing
					// Todo: add it back when for real implementation
					//this.frameTick = stateSnapshot.FrameTick;
				}

				this.lastestReceivedServerPacket = stateSnapshot.FrameTick;
				LogStats.Client_LatestReceivedServerPacket = this.lastestReceivedServerPacket;
			}
			this.frameTick++;

			if (this.ShouldInterpolate)
			{
				// Todo: this delay should be: frame - (updaterate + 1/2latency) * 0.1fudgefactor
				this.RenderFrameTick = this.frameTick - 10;
				StateSnapshot interpolatedBeginState = null;
				StateSnapshot interpolatedEndState = null;
				foreach (var kvp in this.ReceivedStateSnapshots)
				{
					StateSnapshot stateSnapshot = kvp.Value;
					// Todo: these should be more intelligent and grab the closest packets in either direction
					// Todo: make sure we can grab the end packet on the last frame of the interpolation range
					if (stateSnapshot.FrameTick <= this.RenderFrameTick)
					{
						interpolatedBeginState = stateSnapshot;
					}
					if (stateSnapshot.FrameTick > this.RenderFrameTick)
					{
						interpolatedEndState = stateSnapshot;
						break;
					}
				}

				if (interpolatedBeginState != null && interpolatedEndState != null)
				{
					this.InterpolatedStartTick = interpolatedBeginState.FrameTick;
					this.InterpolatedEndTick = interpolatedEndState.FrameTick;
					this.entities = StateSnapshot.Interpolate(interpolatedBeginState, interpolatedEndState, this.RenderFrameTick).Entities;
					this.IsInterpolationValid = true;
				}
				else
				{
					this.IsInterpolationValid = false;
				}
			}
			else
			{
				if (this.ReceivedStateSnapshots.Any())
				{
					this.entities = this.ReceivedStateSnapshots.Last().Value.Entities;
				}
			}
		}

		#endregion Methods
	}
}
