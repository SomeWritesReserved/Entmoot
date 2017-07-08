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

		private int lastReceivedServerTick = -1;
		private INetworkConnection serverNetworkConnection;

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
			while (this.serverNetworkConnection.HasIncomingPackets)
			{
				StateSnapshot stateSnapshot = StateSnapshot.DeserializePacket(this.serverNetworkConnection.GetNextIncomingPacket());
				if (stateSnapshot.FrameTick <= lastReceivedServerTick) { continue; }

				this.entities = stateSnapshot.Entities;
				this.lastReceivedServerTick = stateSnapshot.FrameTick;
			}
			this.frameTick++;
		}

		#endregion Methods
	}
}
