using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a connection between two local endpoints that communicate directly through memory.
	/// </summary>
	/// <remarks>This is not thread safe, it should only be used in a synchronous and sequential manner.
	/// This is designed to not generate garbage between either endpoint connections.</remarks>
	public class LocalNetworkConnection : INetworkConnection
	{
		#region Fields

		/// <summary>The default initial size for the incoming message queue.</summary>
		private const int initialMessageQueueCapacity = 2;

		/// <summary>The message buffer for creating outgoing messages and queueing/reading incoming messages.</summary>
		private readonly MessageBuffer messageBuffer;
		/// <summary>The corresponding network connection that represents the opposite endpoint.</summary>
		private LocalNetworkConnection pairedNetworkConnection;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public LocalNetworkConnection(int maxMessageSize)
		{
			this.MaxMessageSize = maxMessageSize;
			this.messageBuffer = new MessageBuffer(this.MaxMessageSize, LocalNetworkConnection.initialMessageQueueCapacity);
		}

		/// <summary>
		/// Constructor for creating a linked pair of network connections.
		/// </summary>
		private LocalNetworkConnection(LocalNetworkConnection pairedNetworkConnection) : this(pairedNetworkConnection.MaxMessageSize)
		{
			this.pairedNetworkConnection = pairedNetworkConnection;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets that maximum size in bytes for a single message.
		/// </summary>
		public int MaxMessageSize { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns the next message that is coming in over the network. Returns null if no message is ready.
		/// </summary>
		public IncomingMessage GetNextIncomingMessage()
		{
			return this.messageBuffer.GetNextIncomingMessage();
		}

		/// <summary>
		/// Returns an <see cref="OutgoingMessage"/> that can be sent.
		/// </summary>
		public OutgoingMessage GetOutgoingMessageToSend()
		{
			return this.messageBuffer.CreateOutgoingMessage();
		}

		/// <summary>
		/// Sends the given message over the network to the other endpoint.
		/// </summary>
		public void SendMessage(OutgoingMessage outgoingMessage)
		{
			if (this.pairedNetworkConnection == null) { return; }
			IncomingMessage nextIncomingMessage = this.pairedNetworkConnection.messageBuffer.GetMessageToAddToIncomingQueue();
			nextIncomingMessage.CopyFrom(outgoingMessage);
		}

		/// <summary>
		/// Returns a paired <see cref="LocalNetworkConnection"/> that represents the other endpoint.
		/// </summary>
		public LocalNetworkConnection GetPairedNetworkConnection()
		{
			if (this.pairedNetworkConnection == null) { this.pairedNetworkConnection = new LocalNetworkConnection(this); }
			return this.pairedNetworkConnection;
		}

		#endregion Methods
	}
}
