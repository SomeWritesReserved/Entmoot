using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public class Server : ISystem
	{
		#region Fields

		private readonly List<ClientConnection> clients = new List<ClientConnection>();
		private readonly Entity[] entities;

		#endregion Fields

		#region Constructors

		public Server(IEnumerable<INetworkConnection> clientNetworkConnections, Entity[] entities)
		{
			this.clients = clientNetworkConnections.Select((netConn, index) => new ClientConnection(netConn, entities[index])).ToList();
			this.entities = entities.ToArray();
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets or sets the rate at which the server will send updates to the clients (i.e. every Nth frame updates will be sent).</summary>
		public int NetworkSendRate { get; set; } = 3;

		/// <summary>Gets the current frame tick of the server.</summary>
		public int FrameTick { get; private set; }

		/// <summary>Gets the <see cref="StateSnapshot"/> that is currently the most up-to-date state.</summary>
		public StateSnapshot CurrentState { get; private set; }

		#endregion Properties

		#region Methods

		public void Update()
		{
			this.FrameTick++;

			foreach (ClientConnection client in this.clients)
			{
				client.RecieveClientCommands();
			}

			this.CurrentState = new StateSnapshot()
			{
				ServerFrameTick = this.FrameTick,
				Entities = this.entities,
			};

			if (this.FrameTick % this.NetworkSendRate == 0)
			{
				foreach (ClientConnection client in this.clients)
				{
					client.SendStateSnapshot(this.CurrentState);
				}
			}
		}

		#endregion Methods
	}

	public class ClientConnection
	{
		#region Fields

		private INetworkConnection clientNetworkConnection;
		private Entity ownedEntity;

		#endregion Fields

		#region Constructors

		public ClientConnection(INetworkConnection clientCetworkConnection, Entity ownedEntity)
		{
			this.clientNetworkConnection = clientCetworkConnection;
			this.ownedEntity = ownedEntity;
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets the last client tick that was actually received from the client.</summary>
		public int LatestReceivedClientTick { get; private set; } = -1;
		/// <summary>Gets the last server tick that was acknowledged by the client.</summary>
		public int LatestTickAcknowledgedByClient { get; private set; } = -1;

		#endregion Properties

		#region Methods

		public void SendStateSnapshot(StateSnapshot stateSnapshot)
		{
			// Overwrite the state's acked client tick since each client will have a different number here
			stateSnapshot.AcknowledgedClientTick = this.LatestReceivedClientTick;
			stateSnapshot.ClientOwnedEntity = 0;
			byte[] packet = stateSnapshot.SerializePacket();
			this.clientNetworkConnection.SendPacket(packet);
		}

		public void RecieveClientCommands()
		{
			byte[] packet;
			while ((packet = this.clientNetworkConnection.GetNextIncomingPacket()) != null)
			{
				ClientCommand[] clientCommands = ClientCommand.DeserializePacket(packet)
					.Where((cmd) => cmd.ClientFrameTick > this.LatestReceivedClientTick)
					.ToArray();

				foreach (ClientCommand clientCommand in clientCommands)
				{
					clientCommand.RunOnEntity(this.ownedEntity);
					if (this.LatestReceivedClientTick < clientCommand.ClientFrameTick) { this.LatestReceivedClientTick = clientCommand.ClientFrameTick; }
				}
			}
		}

		#endregion Methods
	}
}
