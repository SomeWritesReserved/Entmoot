using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// An authoritative server that will host clients in a simulation of entities.
	/// </summary>
	/// <typeparam name="TCommandData">The type of data expected from clients as a command.</typeparam>
	public class Server<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Fields

		/// <summary>The history of entity state snapshots taken over the past N ticks.</summary>
		private readonly Queue<EntitySnapshot> entitySnapshotHistory;
		/// <summary>The list of clients currently connected to this server.</summary>
		private readonly List<ClientConnection> clients = new List<ClientConnection>(16);

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Server(int maxHistory, int entityCapacity, ComponentsDefinition componentsDefinition, IEnumerable<ISystem> systems)
		{
			this.EntityArray = new EntityArray(entityCapacity, componentsDefinition);
			this.SystemCollection = new SystemCollection(systems);

			// Populate the whole history buffer with data that will be overwritten as needed
			this.entitySnapshotHistory = new Queue<EntitySnapshot>(maxHistory);
			for (int i = 0; i < maxHistory; i++)
			{
				this.entitySnapshotHistory.Enqueue(new EntitySnapshot(entityCapacity, componentsDefinition));
			}
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets or sets the rate at which the server will send updates to the clients (i.e. every Nth frame updates will be sent).</summary>
		public int NetworkSendRate { get; set; } = 3;

		/// <summary>Gets the current frame tick of the server.</summary>
		public int FrameTick { get; private set; }

		/// <summary>Gets the array of entities that are controlled and simulated by this server.</summary>
		public EntityArray EntityArray { get; }

		/// <summary>Gets the collection of systems that will update entities.</summary>
		public SystemCollection SystemCollection { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Adds the given connection as a client to this server.
		/// </summary>
		public void AddConnectedClient(INetworkConnection clientNetworkConnection)
		{
			this.clients.Add(new ClientConnection(this, clientNetworkConnection)
			{
				OwnedEntity = this.clients.Count,
			});
		}

		/// <summary>
		/// Updates the server state byprocessing client input, updating entities, and sending state to clients.
		/// </summary>
		public void Update()
		{
			this.FrameTick++;

			foreach (ClientConnection client in this.clients)
			{
				client.RecieveClientCommands();
			}

			this.SystemCollection.Update(this.EntityArray);

			// Take a snapshot of the latest entity state (recycle old snapshots) and add it to the snapshot history buffer
			EntitySnapshot entitySnapshot = this.entitySnapshotHistory.Dequeue();
			entitySnapshot.UpdateFrom(this.FrameTick, this.EntityArray);
			this.entitySnapshotHistory.Enqueue(entitySnapshot);

			if (this.FrameTick % this.NetworkSendRate == 0)
			{
				foreach (ClientConnection client in this.clients)
				{
					client.SendClientUpdate(entitySnapshot);
				}
			}
		}

		#endregion Methods

		#region Nested Types

		/// <summary>
		/// Represents a connection to a single client of a server.
		/// </summary>
		public class ClientConnection
		{
			#region Fields

			/// <summary>The parent server that owns this client connection.</summary>
			private readonly Server<TCommandData> parentServer;
			/// <summary>The network connection that communicates to the client.</summary>
			private readonly INetworkConnection clientNetworkConnection;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Constructor.
			/// </summary>
			public ClientConnection(Server<TCommandData> parentServer, INetworkConnection clientNetworkConnection)
			{
				this.parentServer = parentServer;
				this.clientNetworkConnection = clientNetworkConnection;
			}

			#endregion Constructors

			#region Properties

			/// <summary>Gets the last client tick that was actually received and acknowledged from the client.</summary>
			public int LatestClientTickAcknowledgedByServer { get; private set; } = -1;
			/// <summary>Gets the last server tick that was acknowledged by the client.</summary>
			public int LatestServerTickAcknowledgedByClient { get; private set; } = -1;

			/// <summary>Gets or sets the entity that this client currently commands.</summary>
			public int OwnedEntity { get; set; } = -1;

			#endregion Properties

			#region Methods

			/// <summary>
			/// Sends the client an update of the given entity state and other information.
			/// </summary>
			public void SendClientUpdate(EntitySnapshot entitySnapshot)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						binaryWriter.Write(this.LatestClientTickAcknowledgedByServer);
						binaryWriter.Write(this.OwnedEntity);
						entitySnapshot.Serialize(binaryWriter);
						this.clientNetworkConnection.SendPacket(memoryStream.ToArray());
					}
				}
			}

			/// <summary>
			/// Checks for and processes any new commands coming in from the client.
			/// </summary>
			public void RecieveClientCommands()
			{
				byte[] packet;
				while ((packet = this.clientNetworkConnection.GetNextIncomingPacket()) != null)
				{
					// Todo: handle out of order packets here and make sure we only execute each command once (drop old packets)
					ClientCommand<TCommandData>[] clientCommands = ClientCommand<TCommandData>.DeserializePacket(packet)
						.Where((cmd) => cmd.ClientFrameTick > this.LatestClientTickAcknowledgedByServer)
						.ToArray();

					foreach (ClientCommand<TCommandData> clientCommand in clientCommands)
					{
						// Ignore the commands that were for some other owned entity (these are old commands the client was trying to execute before it knew of its new entity)
						if (clientCommand.CommandingEntity != this.OwnedEntity) { continue; }

						if (this.OwnedEntity != -1)
						{
							//clientCommand.CommandData.ApplyToEntity(this.parentServer.CurrentState.Entities[this.OwnedEntity]);
						}

						if (this.LatestClientTickAcknowledgedByServer < clientCommand.ClientFrameTick) { this.LatestClientTickAcknowledgedByServer = clientCommand.ClientFrameTick; }
					}
				}
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
