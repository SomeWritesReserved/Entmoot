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

		/// <summary>The ordered history of entity state snapshots taken over the past N frame ticks.</summary>
		private readonly Queue<EntitySnapshot> entitySnapshotHistory;
		/// <summary>The list of clients currently connected to this server.</summary>
		private readonly List<ClientConnection> clients = new List<ClientConnection>(16);

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Server(int maxEntityHistory, int entityCapacity, ComponentsDefinition componentsDefinition, IEnumerable<ISystem> systems)
		{
			this.EntityArray = new EntityArray(entityCapacity, componentsDefinition);
			this.SystemCollection = new SystemCollection(systems);

			// Populate the entire history buffer with data that will be overwritten as needed
			this.entitySnapshotHistory = new Queue<EntitySnapshot>();
			for (int i = 0; i < maxEntityHistory; i++)
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
				CommandingEntityID = this.clients.Count,
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
				client.ProcessClientCommands(this.EntityArray);
			}

			this.SystemCollection.Update(this.EntityArray);

			// Take a snapshot of the latest entity state and add it to the snapshot history buffer (overwriting an old snapshot)
			EntitySnapshot newEntitySnapshot = this.entitySnapshotHistory.Dequeue();
			newEntitySnapshot.Update(this.FrameTick, this.EntityArray);
			this.entitySnapshotHistory.Enqueue(newEntitySnapshot);

			if (this.FrameTick % this.NetworkSendRate == 0)
			{
				foreach (ClientConnection client in this.clients)
				{
					client.SendClientUpdate(newEntitySnapshot);
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
			/// <summary></summary>
			private readonly ClientCommand<TCommandData>[] deserializedClientCommandHistory;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Constructor.
			/// </summary>
			public ClientConnection(Server<TCommandData> parentServer, INetworkConnection clientNetworkConnection)
			{
				this.parentServer = parentServer;
				this.clientNetworkConnection = clientNetworkConnection;

				// Populate the entire history buffer with data that will be overwritten as needed
				this.deserializedClientCommandHistory = new ClientCommand<TCommandData>[ClientCommand<TCommandData>.MaxClientCommandsPerUpdate];
				for (int i = 0; i < this.deserializedClientCommandHistory.Length; i++)
				{
					this.deserializedClientCommandHistory[i] = new ClientCommand<TCommandData>();
				}
			}

			#endregion Constructors

			#region Properties

			/// <summary>Gets the most recent client tick that we got from the client.</summary>
			public int LatestClientTickReceived { get; private set; } = -1;
			/// <summary>Gets the most recent frame tick we sent that we know was received by the client.</summary>
			public int LatestFrameTickAcknowledgedByClient { get; private set; } = -1;
			/// <summary>Gets or sets the entity that this client currently commands.</summary>
			public int CommandingEntityID { get; set; } = -1;

			#endregion Properties

			#region Methods

			/// <summary>
			/// Sends the client an update of the given entity state and other information.
			/// </summary>
			public void SendClientUpdate(EntitySnapshot entitySnapshot)
			{
				ServerUpdateSerializer.Send(this.clientNetworkConnection, entitySnapshot, this.LatestClientTickReceived, this.CommandingEntityID);
			}

			/// <summary>
			/// Checks for and processes any new commands coming in from the client.
			/// </summary>
			public void ProcessClientCommands(EntityArray entityArray)
			{
				byte[] packet;
				while ((packet = this.clientNetworkConnection.GetNextIncomingPacket()) != null)
				{
					// Todo: handle out of order packets here and make sure we only execute each command once (drop old packets)
					int numberOfCommands = ClientUpdateSerializer<TCommandData>.Deserialize(packet, this.deserializedClientCommandHistory, out int newlatestFrameTickAcknowledgedByClient);

					if (this.LatestFrameTickAcknowledgedByClient < newlatestFrameTickAcknowledgedByClient) { this.LatestFrameTickAcknowledgedByClient = newlatestFrameTickAcknowledgedByClient; }

					for (int i = 0; i < numberOfCommands; i++)
					{
						ClientCommand<TCommandData> clientCommand = this.deserializedClientCommandHistory[i];

						// Make sure we don't process a command we've already receieved and processed in a previous tick
						if (!clientCommand.HasData || clientCommand.ClientFrameTick <= this.LatestClientTickReceived) { continue; }

						// Make sure we have an entity to command, that the client thinks its commanding the same entity, and that the entity exists
						if (this.CommandingEntityID != -1 && clientCommand.CommandingEntityID == this.CommandingEntityID && entityArray.TryGetEntity(this.CommandingEntityID, out Entity entity))
						{
							clientCommand.CommandData.ApplyToEntity(entity);
						}

						if (this.LatestClientTickReceived < clientCommand.ClientFrameTick) { this.LatestClientTickReceived = clientCommand.ClientFrameTick; }
					}
				}
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
