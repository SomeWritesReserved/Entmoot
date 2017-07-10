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

		private int lastestReceivedServerPacket = -1;
		private INetworkConnection serverNetworkConnection;
		private SortedList<int, StateSnapshot> receivedStateSnapshots = new SortedList<int, StateSnapshot>(64);

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

		#endregion Properties

		#region Methods

		public void Update()
		{
			byte[] packet;
			while ((packet = this.serverNetworkConnection.GetNextIncomingPacket()) != null)
			{
				StateSnapshot stateSnapshot = StateSnapshot.DeserializePacket(packet);
				this.receivedStateSnapshots.Add(stateSnapshot.FrameTick, stateSnapshot);

				if (this.lastestReceivedServerPacket < 0)
				{
					this.frameTick = stateSnapshot.FrameTick;
				}

				this.lastestReceivedServerPacket = stateSnapshot.FrameTick;
				LogStats.Client_LatestReceivedServerPacket = this.lastestReceivedServerPacket;
			}
			this.frameTick++;

			int renderFrameTick = this.frameTick - 8;
			StateSnapshot interpolatedBeginState = null;
			StateSnapshot interpolatedEndState = null;
			foreach (var kvp in this.receivedStateSnapshots)
			{
				StateSnapshot stateSnapshot = kvp.Value;
				if (stateSnapshot.FrameTick <= renderFrameTick)
				{
					interpolatedBeginState = stateSnapshot;
				}
				if (stateSnapshot.FrameTick > renderFrameTick)
				{
					interpolatedEndState = stateSnapshot;
					break;
				}
			}

			if (interpolatedBeginState != null && interpolatedEndState != null)
			{
				this.entities = StateSnapshot.Interpolate(interpolatedBeginState, interpolatedEndState, renderFrameTick).Entities;
			}
		}

		#endregion Methods
	}
}
