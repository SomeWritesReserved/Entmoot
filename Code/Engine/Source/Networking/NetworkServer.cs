using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Manages a server on a network that can listen for and accept client connections.
	/// </summary>
	/// <remarks>This class does not manage the game or update, it only managed the network connections and give the game server incoming packets from connected clients.</remarks>
	public class NetworkServer
	{
		#region Fields

		/// <summary>The array of clients and their states (all client objects exist all the time even if no client is connected for that client ID).</summary>
		private readonly ClientNetworkConnection[] clients;
		/// <summary>A lookup table from an incoming packet's source end point to a client ID (which is the index into the <see cref="clients"/> field).</summary>
		private readonly Dictionary<IPEndPoint, int> endPointToClientID;

		/// <summary>The local EndPoint the server will be bound to.</summary>
		private readonly EndPoint localServerEndPoint;
		/// <summary>The actual socket that will be used for network communication between the server and hosted clients.</summary>
		private readonly Socket socket;

		/// <summary>The outgoing message used to send out-of-band packets to unconnected or not-yet-fully connected clients, directly from this object.</summary>
		private readonly OutgoingMessage outgoingMessage;
		/// <summary>A temporary storage for incoming networking data received from the socket (used only for Socket.ReceiveFrom).</summary>
		private readonly IncomingMessage receivedIncomingMessage;
		/// <summary>A temporary stroage for incoming networking end points on the socket (used only for Socket.ReceiveFrom).</summary>
		private EndPoint receivedEndPoint;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public NetworkServer(string applicationID, byte maxClients, int maxMessageSize, int listenPort)
		{
			this.ApplicationID = applicationID;
			this.MaxClients = maxClients;
			this.MaxMessageSize = maxMessageSize;

			this.endPointToClientID = new Dictionary<IPEndPoint, int>(this.MaxClients);
			this.clients = new ClientNetworkConnection[this.MaxClients];
			for (int clientID = 0; clientID < this.MaxClients; clientID++)
			{
				this.clients[clientID] = new ClientNetworkConnection(this, maxMessageSize);
				this.clients[clientID].SetStateDisconnected();
			}

			this.localServerEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			this.socket.Blocking = false;

			this.outgoingMessage = new OutgoingMessage(new byte[maxMessageSize]);
			this.receivedIncomingMessage = new IncomingMessage(new byte[maxMessageSize]);
			this.receivedEndPoint = new IPEndPoint(0, 0);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the application ID for this networked server (used to verify the clients are using the same application).
		/// </summary>
		public string ApplicationID { get; }

		/// <summary>
		/// Gets tha maximum number of clients that can be connected to the server at one time.
		/// </summary>
		public byte MaxClients { get; }

		/// <summary>
		/// Gets that maximum size in bytes for a single message.
		/// </summary>
		public int MaxMessageSize { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates the connections by reading data and writing data to the network, as well as updating the state
		/// of the clients.
		/// </summary>
		public void Update()
		{
			if (!this.socket.IsBound)
			{
				this.socket.Bind(this.localServerEndPoint);
			}

			while (this.socket.Available > 0)
			{
				this.receivedIncomingMessage.Clear();
				this.receivedIncomingMessage.Length = this.socket.ReceiveFrom(this.receivedIncomingMessage.MessageData, ref this.receivedEndPoint);
				this.processIncomingMessage(this.receivedIncomingMessage, (IPEndPoint)this.receivedEndPoint);
			}
		}

		/// <summary>
		/// Processes an incoming message and handles any of the possible types of packets it could be.
		/// </summary>
		private void processIncomingMessage(IncomingMessage incomingMessage, IPEndPoint endPoint)
		{
			if (incomingMessage.BytesLeft < 2) { return; }
			PacketType packetType = (PacketType)incomingMessage.ReadByte();
			PacketTypeDetail packetTypeDetail = (PacketTypeDetail)incomingMessage.ReadByte();

			if (this.endPointToClientID.TryGetValue(endPoint, out int clientID))
			{
				ClientNetworkConnection client = this.clients[clientID];
				if (packetType == PacketType.ClientConnectFinalize)
				{
					client.SetStateConnected(endPoint);
				}
				else if (packetType == PacketType.GameUpdate && packetTypeDetail == PacketTypeDetail.GameUpdateFromClient)
				{
					client.EnqueueIncomingMessage(incomingMessage);
				}
			}
			else
			{
				// A message from a non-connected endpoint, only process it if its a connection request
				if (packetType == PacketType.ClientConnectRequest)
				{
					this.processClientConnectRequest(incomingMessage, endPoint);
				}
			}
		}

		/// <summary>
		/// Processes an unconnected client's request to connect to the server for the first time.
		/// </summary>
		private void processClientConnectRequest(IncomingMessage incomingMessage, IPEndPoint endPoint)
		{
			if (incomingMessage.BytesLeft < 9) { return; }

			int clientAppIDHash = incomingMessage.ReadInt32();
			int clientMaxMessageSize = incomingMessage.ReadInt32();
			string clientName = incomingMessage.ReadString();

			int nextClientID = this.getNextAvailableClientID();
			if (clientAppIDHash != this.ApplicationID.GetHashCode() || clientMaxMessageSize != this.MaxMessageSize)
			{
				// Client is using a different app, reject
				this.sendConnectResponse(endPoint, 0, PacketTypeDetail.ConnectResponseRejectAppMismatch);
			}
			else if (nextClientID < 0)
			{
				// Server is full, reject
				this.sendConnectResponse(endPoint, 0, PacketTypeDetail.ConnectResponseRejectServerFull);
			}
			else
			{
				this.clients[nextClientID].SetStateAwaitingConnectFinalize(endPoint);
				this.endPointToClientID[endPoint] = nextClientID;
				this.sendConnectResponse(endPoint, (byte)nextClientID, PacketTypeDetail.ConnectResponseAccept);
			}
		}

		/// <summary>
		/// Immediately sends a packet to the given end point in response to a connection request.
		/// </summary>
		private void sendConnectResponse(IPEndPoint toEndPoint, byte clientID, PacketTypeDetail packetTypeInfo)
		{
			this.outgoingMessage.Reset();
			this.outgoingMessage.Write((byte)PacketType.ServerConnectResponse);
			this.outgoingMessage.Write((byte)packetTypeInfo);
			this.outgoingMessage.Write(clientID);
			this.outgoingMessage.Write(this.ApplicationID.GetHashCode());
			this.outgoingMessage.Write(this.MaxMessageSize);
			this.socket.SendTo(this.outgoingMessage.MessageData, toEndPoint);
		}

		/// <summary>
		/// Returns the next available client ID if one exists. Returns -1 if there are no available client IDs
		/// (i.e. the server is full).
		/// </summary>
		private int getNextAvailableClientID()
		{
			for (int clientID = 0; clientID < this.MaxClients; clientID++)
			{
				if (this.clients[clientID].ClientState == ClientState.Disconnected) { return clientID; }
			}
			return -1;
		}

		#endregion Methods

		#region Nested Types

		/// <summary>
		/// Represents a network connection from the server to a client (can also represent a client slot that hasn't been filled yet,
		/// see <see cref="ClientState.Disconnected"/>).
		/// </summary>
		private class ClientNetworkConnection : INetworkConnection
		{
			#region Fields

			/// <summary>The parent server that owns this client network connection.</summary>
			private readonly NetworkServer parentServer;
			/// <summary>The message buffer for creating outgoing messages and queueing/reading incoming messages.</summary>
			private readonly MessageBuffer messageBuffer;
			/// <summary>The end point of this client to send messages to and to expect messages from.</summary>
			private readonly IPEndPoint clientEndPoint;
			/// <summary>The last time a message or packet was received from this client.</summary>
			private double lastReceivedTime;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Constructor.
			/// </summary>
			public ClientNetworkConnection(NetworkServer parentServer, int maxMessageSize)
			{
				this.parentServer = parentServer;
				this.messageBuffer = new MessageBuffer(maxMessageSize, 2);
				this.clientEndPoint = new IPEndPoint(0, 0);
			}

			#endregion Constructors

			#region Properties

			/// <summary>
			/// Gets the current state of this client connection.
			/// </summary>
			public ClientState ClientState { get; private set; }

			/// <summary>
			/// Gets whether or not this client connection is actually connected to another endpoint.
			/// </summary>
			public bool IsConnected { get { return this.ClientState != ClientState.Disconnected; } }

			#endregion Properties

			#region Methods

			/// <summary>
			/// Sets the state of this client to the <see cref="ClientState.Disconnected"/> state.
			/// </summary>
			public void SetStateDisconnected()
			{
				this.ClientState = ClientState.Disconnected;
				this.clientEndPoint.Address = IPAddress.None;
				this.clientEndPoint.Port = 0;
				this.messageBuffer.Clear();
			}

			/// <summary>
			/// Sets the state of this client to the <see cref="ClientState.AwaitingConnectFinalize"/> state.
			/// </summary>
			public void SetStateAwaitingConnectFinalize(IPEndPoint endPoint)
			{
				this.ClientState = ClientState.AwaitingConnectFinalize;
				this.clientEndPoint.Address = endPoint.Address;
				this.clientEndPoint.Port = endPoint.Port;
			}

			/// <summary>
			/// Sets the state of this client to the <see cref="ClientState.Connected"/> state.
			/// </summary>
			public void SetStateConnected(IPEndPoint endPoint)
			{
				this.ClientState = ClientState.Connected;
				this.clientEndPoint.Address = endPoint.Address;
				this.clientEndPoint.Port = endPoint.Port;
			}

			/// <summary>
			/// Copies the given incoming message into this client's next incoming message queue.
			/// </summary>
			public void EnqueueIncomingMessage(IncomingMessage incomingMessage)
			{
				IncomingMessage clientIncomingMessage = this.messageBuffer.GetMessageToAddToIncomingQueue();
				clientIncomingMessage.CopyFrom(incomingMessage);
			}

			/// <summary>
			/// Returns the next message that is coming in from the client. Returns null if no message is ready.
			/// </summary>
			IncomingMessage INetworkConnection.GetNextIncomingMessage()
			{
				return this.messageBuffer.GetNextIncomingMessage();
			}

			/// <summary>
			/// Returns an <see cref="OutgoingMessage"/> that can be sent.
			/// </summary>
			OutgoingMessage INetworkConnection.GetOutgoingMessageToSend()
			{
				return this.messageBuffer.CreateOutgoingMessage();
			}

			/// <summary>
			/// Sends the given message over the network to the client.
			/// </summary>
			void INetworkConnection.SendMessage(OutgoingMessage outgoingMessage)
			{
				this.parentServer.socket.SendTo(outgoingMessage.MessageData, outgoingMessage.Length, SocketFlags.None, this.clientEndPoint);
			}

			#endregion Methods
		}

		/// <summary>
		/// Represents the state of a <see cref="ClientNetworkConnection"/> object with respect to the server.
		/// </summary>
		private enum ClientState : byte
		{
			/// <summary>No client is connected.</summary>
			Disconnected = 0,
			/// <summary>The client's connection request was accepted and the server is now waiting for the connection finalized packet from the client.</summary>
			AwaitingConnectFinalize,
			/// <summary>The client is fully connected and updates are being sent.</summary>
			Connected,
		}

		#endregion Nested Types
	}

	public enum PacketType : byte
	{
		None = 0,
		GameUpdate,
		ClientConnectRequest,
		ServerConnectResponse,
		ClientConnectFinalize,
	}

	public enum PacketTypeDetail : byte
	{
		None = 0,
		GameUpdateFromServer,
		GameUpdateFromClient,
		ConnectResponseAccept,
		ConnectResponseRejectAppMismatch,
		ConnectResponseRejectServerFull,
	}
}
