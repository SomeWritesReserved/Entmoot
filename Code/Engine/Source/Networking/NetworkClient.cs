﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Manages a client on a network that can connect to a server and receive updates.
	/// </summary>
	/// <remarks>This class does not manage the game or update, it only manages the network connection and gives the game client incoming packets from the server.</remarks>
	public class NetworkClient : INetworkConnection
	{
		#region Fields

		/// <summary>The end point of the server to send messages to and to expect messages from.</summary>
		private EndPoint serverEndPoint;
		/// <summary>The local end point the client will be bound to and listen on.</summary>
		private readonly EndPoint boundEndPoint;
		/// <summary>The actual socket that will be used for network communication between this client and the server.</summary>
		private readonly Socket socket;

		/// <summary>The message buffer for creating outgoing messages and queuing/reading incoming messages.</summary>
		private readonly MessageBuffer messageBuffer;
		/// <summary>The outgoing message used to send out-of-band packets to unconnected or not-yet-fully connected clients, directly from this object.</summary>
		private readonly OutgoingMessage outgoingMessage;
		/// <summary>A temporary storage for incoming networking data received from the socket (used only for Socket.ReceiveFrom).</summary>
		private readonly IncomingMessage receivedIncomingMessage;
		/// <summary>A temporary storage for incoming networking end points on the socket (used only for Socket.ReceiveFrom).</summary>
		private EndPoint receivedEndPoint;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public NetworkClient(string applicationID, int maxMessageSize)
		{
			this.ApplicationID = applicationID;
			this.MaxMessageSize = maxMessageSize;

			this.boundEndPoint = new IPEndPoint(IPAddress.Any, 0);
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			this.socket.Blocking = false;
			this.socket.ReceiveBufferSize = 131071;
			this.socket.SendBufferSize = 131071;

			this.messageBuffer = new MessageBuffer(maxMessageSize, 2);
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
		/// Gets that maximum size in bytes for a single message.
		/// </summary>
		public int MaxMessageSize { get; }

		/// <summary>
		/// Gets whether or not this client is actually connected to the server.
		/// </summary>
		public bool IsConnected { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Connects this client to a server.
		/// </summary>
		public void Connect(IPEndPoint serverEndPoint)
		{
			this.serverEndPoint = serverEndPoint;
			this.socket.Bind(this.boundEndPoint);
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				const uint IOC_IN = 0x80000000;
				const uint IOC_VENDOR = 0x18000000;
				uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
				this.socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
			}

			this.sendConnectRequest(PacketType.ClientConnectRequest, PacketTypeDetail.None);
		}

		/// <summary>
		/// Disconnects this client from the server.
		/// </summary>
		public void Disconnect()
		{
			this.IsConnected = false;
			this.socket.Shutdown(SocketShutdown.Receive);
			this.socket.Close(1);
		}

		/// <summary>
		/// Updates the connection by reading data and writing data to the network.
		/// </summary>
		public void Update()
		{
			Log<LogNetworkClient>.StartNew();

			while (this.socket.Available > 0)
			{
				this.receivedIncomingMessage.Clear();
				this.receivedIncomingMessage.Length = this.socket.ReceiveFrom(this.receivedIncomingMessage.MessageData, ref this.receivedEndPoint);
				this.processIncomingMessage(this.receivedIncomingMessage, (IPEndPoint)this.receivedEndPoint);
				Log<LogNetworkClient>.Data.ReceivedBytes += this.receivedIncomingMessage.Length;
				Log<LogNetworkClient>.Data.ReceivedPackets++;
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

			if (packetType == PacketType.ServerConnectResponse)
			{
				if (packetTypeDetail == PacketTypeDetail.ConnectResponseAccept)
				{
					this.sendConnectRequest(PacketType.ClientConnectFinalize, PacketTypeDetail.None);
					this.IsConnected = true;
				}
			}
			else if (packetType == PacketType.GameUpdate && packetTypeDetail == PacketTypeDetail.GameUpdateFromServer)
			{
				this.enqueueGameUpdateIncomingMessage(incomingMessage);
			}
		}

		/// <summary>
		/// Immediately sends a packet to the given end point in response to a connection request.
		/// </summary>
		private void sendConnectRequest(PacketType packetType, PacketTypeDetail packetTypeInfo)
		{
			this.outgoingMessage.Reset();
			this.outgoingMessage.Write((byte)packetType);
			this.outgoingMessage.Write((byte)packetTypeInfo);
			this.outgoingMessage.Write(ReaderWriterHelper.GetStringHash(this.ApplicationID));
			this.outgoingMessage.Write(this.MaxMessageSize);
			this.outgoingMessage.Write("No name");
			((INetworkConnection)this).SendMessage(this.outgoingMessage);
		}

		/// <summary>
		/// Copies the given incoming message into the client's next incoming message queue for game updates.
		/// </summary>
		private void enqueueGameUpdateIncomingMessage(IncomingMessage incomingMessage)
		{
			IncomingMessage clientIncomingMessage = this.messageBuffer.GetMessageToAddToIncomingQueue();
			clientIncomingMessage.CopyFrom(incomingMessage);
		}

		/// <summary>
		/// Returns the next message that is coming in from the server. Returns null if no message is ready.
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
			OutgoingMessage outgoingMessage = this.messageBuffer.CreateOutgoingMessage();
			outgoingMessage.Write((byte)PacketType.GameUpdate);
			outgoingMessage.Write((byte)PacketTypeDetail.GameUpdateFromClient);
			return outgoingMessage;
		}

		/// <summary>
		/// Sends the given message over the network to the server.
		/// </summary>
		void INetworkConnection.SendMessage(OutgoingMessage outgoingMessage)
		{
			this.socket.SendTo(outgoingMessage.MessageData, outgoingMessage.Length, SocketFlags.None, this.serverEndPoint);
			Log<LogNetworkClient>.Data.SentBytes += outgoingMessage.Length;
			Log<LogNetworkClient>.Data.SentPackets++;
		}

		#endregion Methods
	}

	/// <summary>
	/// Log data for <see cref="NetworkClient"/>.
	/// </summary>
	public struct LogNetworkClient
	{
		#region Fields

		/// <summary>The number of bytes received over one entire update.</summary>
		public int ReceivedBytes;
		/// <summary>The number of complete packets received over one entire update.</summary>
		public int ReceivedPackets;
		/// <summary>The number of bytes sent over one entire update.</summary>
		public int SentBytes;
		/// <summary>The number of complete packets sent over one entire update.</summary>
		public int SentPackets;

		#endregion Fields
	}
}
