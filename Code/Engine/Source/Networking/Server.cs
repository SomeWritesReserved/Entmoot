using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public class Server<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Fields

		private readonly List<ClientConnection> clients = new List<ClientConnection>();
		private readonly Queue<StateSnapshot> stateSnapshotHistory = new Queue<StateSnapshot>(64);

		#endregion Fields

		#region Constructors

		public Server(EntityManager entityManager)
		{
			this.entityManager = entityManager;
		}

		#endregion Constructors

		#region Properties

		private readonly EntityManager entityManager;
		/// <summary>Gets the <see cref="EntityManager"/> this server is using to store and update entities.</summary>
		public EntityManager EntityManager
		{
			get { return this.entityManager; }
		}

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

			this.entityManager.Update();

			this.CurrentState = new StateSnapshot()
			{
				ServerFrameTick = this.FrameTick,
				Entities = this.EntityManager.Entities.ToArray(),
			};
			this.stateSnapshotHistory.Enqueue(StateSnapshot.Clone(this.CurrentState));

			if (this.FrameTick % this.NetworkSendRate == 0)
			{
				foreach (ClientConnection client in this.clients)
				{
					client.SendStateSnapshot(this.CurrentState);
				}
			}
		}

		public void AddConnectedClient(INetworkConnection clientNetworkConnection)
		{
			this.clients.Add(new ClientConnection(this, clientNetworkConnection));
		}

		#endregion Methods

		#region Nested Types

		public class ClientConnection
		{
			#region Fields

			private Server<TCommandData> parentServer;
			private INetworkConnection clientNetworkConnection;

			#endregion Fields

			#region Constructors

			public ClientConnection(Server<TCommandData> parentServer, INetworkConnection clientNetworkConnection)
			{
				this.parentServer = parentServer;
				this.clientNetworkConnection = clientNetworkConnection;
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
					ClientCommand<TCommandData>[] clientCommands = ClientCommand<TCommandData>.DeserializePacket(packet)
						.Where((cmd) => cmd.ClientFrameTick > this.LatestReceivedClientTick)
						.ToArray();

					foreach (ClientCommand<TCommandData> clientCommand in clientCommands)
					{
						// Ignore the commands that were for some other owned entity (these are old commands the client was trying to execute before it knew of its new entity)
						if (clientCommand.CommandingEntity != this.OwnedEntity) { continue; }

						if (this.OwnedEntity != -1)
						{
							clientCommand.RunOnEntity(this.parentServer.CurrentState.Entities[this.OwnedEntity]);
						}

						if (this.LatestReceivedClientTick < clientCommand.ClientFrameTick) { this.LatestReceivedClientTick = clientCommand.ClientFrameTick; }
					}
				}
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
