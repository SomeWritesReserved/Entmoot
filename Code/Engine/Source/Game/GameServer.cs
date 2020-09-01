using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// An authoritative game server that will host clients, taking input from them and updating them on the state of entities.
	/// </summary>
	/// <remarks>This class does not manage the actual network connections, it only handles updating connected clients.</remarks>
	/// <typeparam name="TCommandData">The type of data expected from clients as a command.</typeparam>
	public class GameServer<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Fields

		/// <summary>The ordered history of entity state snapshots taken over the past N frame ticks.</summary>
		private readonly Queue<EntitySnapshot> entitySnapshotHistory;
		/// <summary>The collection of client proxies (all client proxies exist all the time even if no client is connected for that client ID).</summary>
		private readonly ClientProxy[] clients;
		/// <summary>The delegate to update a client's commanding entity ID every frame. The first bool is whether the client is connected. The second integer is the current commanding entity ID.</summary>
		private readonly Func<bool, int, int> updateCommandingEntityID;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="clientNetworkConnections">The sequence of network connections that could be connected to clients (but probably aren't yet).
		/// This defines the maximum number of clients. These clients can connect/disconnect on the fly and the server will monitor their state and update accordingly.</param>
		/// <param name="maxEntityHistory">The maximum number of <see cref="EntitySnapshot"/> that will be remembered for the server's history.
		/// One snapshot is taken each frame so this number is effectively "the last N frames" of history.</param>
		/// <param name="entityCapacity">The maximum number of entities the world can have.</param>
		/// <param name="componentsDefinition">The definition of components that are supported.</param>
		/// <param name="systems">The collection of server systems used to update the world's state on the server.</param>
		/// <param name="updateCommandingEntityID">The delegate to update a client's commanding entity ID every frame.
		/// The first bool is whether the client is connected. The second integer is the current commanding entity ID.</param>
		public GameServer(IList<INetworkConnection> clientNetworkConnections, int maxEntityHistory, int entityCapacity,
			ComponentsDefinition componentsDefinition, IEnumerable<IServerSystem> systems, Func<bool, int, int> updateCommandingEntityID)
		{
			this.updateCommandingEntityID = updateCommandingEntityID;
			this.EntityArray = new EntityArray(entityCapacity, componentsDefinition);
			this.SystemArray = new ServerSystemArray(systems);

			// Populate the entire history buffer with data that will be overwritten as needed
			this.entitySnapshotHistory = new Queue<EntitySnapshot>();
			for (int i = 0; i < maxEntityHistory; i++)
			{
				this.entitySnapshotHistory.Enqueue(new EntitySnapshot(entityCapacity, componentsDefinition));
			}

			// Create all the client proxies now, bound directly to the client network connections
			this.clients = new ClientProxy[clientNetworkConnections.Count];
			for (int clientID = 0; clientID < clientNetworkConnections.Count; clientID++)
			{
				this.clients[clientID] = new ClientProxy(this, clientNetworkConnections[clientID]);
			}
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets or sets the rate at which the server will send updates to the clients (i.e. every Nth frame updates will be sent).</summary>
		public int NetworkSendRate { get; set; } = 3;

		/// <summary>Gets the current frame tick of the server.</summary>
		public int FrameTick { get; private set; }
		/// <summary>Gets the array of entities that are controlled and updated by this server.</summary>
		public EntityArray EntityArray { get; }
		/// <summary>Gets the collection of systems that will update server entities.</summary>
		public ServerSystemArray SystemArray { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates the server state by processing client input, updating entities, and sending state to clients.
		/// </summary>
		public void Update()
		{
			this.FrameTick++;

			foreach (ClientProxy client in this.clients)
			{
				int newCommandingEntityID = this.updateCommandingEntityID.Invoke(client.IsConnected, client.CommandingEntityID);
				if (client.IsConnected)
				{
					client.CommandingEntityID = newCommandingEntityID;
					client.ReceiveClientCommands(this.EntityArray);
				}
				else
				{
					client.Reset();
				}
			}

			this.SystemArray.ServerUpdate(this.EntityArray);

			// Take a snapshot of the latest entity state and add it to the snapshot history buffer (overwriting an old snapshot)
			EntitySnapshot newEntitySnapshot = this.entitySnapshotHistory.Dequeue();
			newEntitySnapshot.Update(this.FrameTick, this.EntityArray);
			this.entitySnapshotHistory.Enqueue(newEntitySnapshot);

			if (this.FrameTick % this.NetworkSendRate == 0)
			{
				foreach (ClientProxy client in this.clients)
				{
					if (client.IsConnected)
					{
						client.SendServerUpdate(newEntitySnapshot);
					}
				}
			}
		}

		/// <summary>
		/// Executes a given client command on behalf of the given client.
		/// </summary>
		private void executeClientCommand(ClientProxy client, ClientCommand<TCommandData> clientCommand)
		{
			// Make sure we have an entity to command, that the client thinks its commanding that same entity, and that the entity actually exists
			if (client.CommandingEntityID != -1 && clientCommand.CommandingEntityID == client.CommandingEntityID && this.EntityArray.TryGetEntity(client.CommandingEntityID, out Entity commandingEntity))
			{
				clientCommand.CommandData.ApplyToEntity(commandingEntity);
			}
		}

		/// <summary>
		/// Returns the stored entity snapshot that was taken at a taken at a given frame tick. Returns null if no snapshot is found
		/// (either because a snapshot wasn't taken at that tick or the tick is too old and the entity snapshot was already removed).
		/// </summary>
		private EntitySnapshot getEntitySnapshotForServerFrameTick(int serverFrameTick)
		{
			foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
			{
				if (entitySnapshot.ServerFrameTick == serverFrameTick) { return entitySnapshot; }
			}
			return null;
		}

		#endregion Methods

		#region Nested Types

		/// <summary>
		/// Represents a proxy of a single client of a server.
		/// </summary>
		public class ClientProxy
		{
			#region Fields

			/// <summary>The parent server that owns this client proxy.</summary>
			private readonly GameServer<TCommandData> parentServer;
			/// <summary>The network connection that communicates to the client.</summary>
			private readonly INetworkConnection clientNetworkConnection;
			/// <summary>The history buffer used to temporarily store incoming client command data from a single message.</summary>
			private readonly ClientCommand<TCommandData>[] deserializedClientCommandHistory;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Constructor.
			/// </summary>
			public ClientProxy(GameServer<TCommandData> parentServer, INetworkConnection clientNetworkConnection)
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

			/// <summary>Gets whether or not this client proxy is actually connected to the server.</summary>
			public bool IsConnected { get { return this.clientNetworkConnection.IsConnected; } }

			/// <summary>Gets the most recent client tick that we got from the client.</summary>
			public int LatestClientTickReceived { get; private set; } = -1;
			/// <summary>Gets the most recent frame tick we sent that we know was received by the client.</summary>
			public int LatestFrameTickAcknowledgedByClient { get; private set; } = -1;
			/// <summary>Gets or sets the entity that this client currently commands.</summary>
			public int CommandingEntityID { get; set; } = -1;

			#endregion Properties

			#region Methods

			/// <summary>
			/// Checks for and processes any new commands coming in from the client.
			/// </summary>
			public void ReceiveClientCommands(EntityArray entityArray)
			{
				IncomingMessage incomingMessage;
				while ((incomingMessage = this.clientNetworkConnection.GetNextIncomingMessage()) != null)
				{
					int numberOfCommands = ClientUpdateSerializer<TCommandData>.Deserialize(incomingMessage, this.deserializedClientCommandHistory, out int newlatestFrameTickAcknowledgedByClient);

					if (this.LatestFrameTickAcknowledgedByClient < newlatestFrameTickAcknowledgedByClient) { this.LatestFrameTickAcknowledgedByClient = newlatestFrameTickAcknowledgedByClient; }

					for (int i = 0; i < numberOfCommands; i++)
					{
						ClientCommand<TCommandData> clientCommand = this.deserializedClientCommandHistory[i];

						// Make sure we don't process a command we've already received and processed in a previous tick
						if (!clientCommand.HasData || clientCommand.ClientFrameTick <= this.LatestClientTickReceived) { continue; }

						this.parentServer.executeClientCommand(this, clientCommand);

						if (this.LatestClientTickReceived < clientCommand.ClientFrameTick) { this.LatestClientTickReceived = clientCommand.ClientFrameTick; }
					}
				}
			}

			/// <summary>
			/// Sends the client an update of the given entity state and other information.
			/// </summary>
			public void SendServerUpdate(EntitySnapshot latestEntitySnapshot)
			{
				EntitySnapshot previousEntitySnapshot = this.parentServer.getEntitySnapshotForServerFrameTick(this.LatestFrameTickAcknowledgedByClient);
				ServerUpdateSerializer.Send(this.clientNetworkConnection, previousEntitySnapshot, latestEntitySnapshot, this.LatestClientTickReceived, this.CommandingEntityID);
			}

			/// <summary>
			/// Resets this client proxy to the state it would be in before a client connected.
			/// </summary>
			public void Reset()
			{
				this.LatestClientTickReceived = -1;
				this.LatestFrameTickAcknowledgedByClient = -1;
				this.CommandingEntityID = -1;
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
