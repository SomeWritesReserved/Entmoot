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
			this.clients = clientNetworkConnections.Select((netConn, index) => new ClientConnection(this, netConn)).ToList();
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

		private Server parentServer;
		private INetworkConnection clientNetworkConnection;

		#endregion Fields

		#region Constructors

		public ClientConnection(Server parentServer, INetworkConnection clientCetworkConnection)
		{
			this.parentServer = parentServer;
			this.clientNetworkConnection = clientCetworkConnection;
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets the last client tick that was actually received from the client.</summary>
		public int LatestReceivedClientTick { get; private set; } = -1;
		/// <summary>Gets the last server tick that was acknowledged by the client.</summary>
		public int LatestTickAcknowledgedByClient { get; private set; } = -1;

		/// <summary>Gets or sets the entity that this client currently owns.</summary>
		public int OwnedEntity { get; set; } = -1;

		#endregion Properties

		#region Methods

		public void SendStateSnapshot(StateSnapshot stateSnapshot)
		{
			// Overwrite the state's acked client tick and owned entity since each client will be different
			stateSnapshot.AcknowledgedClientTick = this.LatestReceivedClientTick;
			stateSnapshot.ClientOwnedEntity = this.OwnedEntity;

			byte[] packet = stateSnapshot.SerializePacket();
			this.clientNetworkConnection.SendPacket(packet);
		}

		public void RecieveClientCommands()
		{
			byte[] packet;
			while ((packet = this.clientNetworkConnection.GetNextIncomingPacket()) != null)
			{
				// Todo: handle out of order packets here and make sure we only execute each command once (drop old packets)
				ClientCommand[] clientCommands = ClientCommand.DeserializePacket(packet)
					.Where((cmd) => cmd.ClientFrameTick > this.LatestReceivedClientTick)
					.ToArray();

				foreach (ClientCommand clientCommand in clientCommands)
				{
					// Ignore the commands that were for some other owned entity (these are old commands the client was trying to execute before it knew of its new entity)
					if (clientCommand.OwnedEntity != this.OwnedEntity) { continue; }

					if (this.OwnedEntity != -1)
					{
						clientCommand.RunOnEntity(this.parentServer.CurrentState.Entities[this.OwnedEntity]);
					}

					if ((clientCommand.CommandKeys & CommandKeys.Seat1) != 0)
					{
						this.OwnedEntity = 0;
					}
					else if ((clientCommand.CommandKeys & CommandKeys.Seat2) != 0)
					{
						this.OwnedEntity = 1;
					}

					if (this.LatestReceivedClientTick < clientCommand.ClientFrameTick) { this.LatestReceivedClientTick = clientCommand.ClientFrameTick; }
				}
			}
		}

		#endregion Methods
	}
}
