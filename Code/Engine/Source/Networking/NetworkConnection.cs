using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a connection between two endpoints over the network.
	/// </summary>
	public interface INetworkConnection
	{
		#region Methods

		/// <summary>
		/// Returns the next message that is coming in over the network. Returns null if no message is ready.
		/// </summary>
		IncomingMessage GetNextIncomingMessage();

		/// <summary>
		/// Returns an <see cref="OutgoingMessage"/> that can be sent.
		/// </summary>
		OutgoingMessage GetOutgoingMessageToSend();

		/// <summary>
		/// Sends the given message over the network to the other endpoint.
		/// </summary>
		void SendMessage(OutgoingMessage outgoingMessage);

		#endregion Methods
	}

	/// <summary>
	/// Represents a connection between two local endpoints that communicate directly through memory.
	/// </summary>
	/// <remarks>This is not thread safe, it should only be used in a synchronous and sequential manner.
	/// This is designed to not generate garbage between either endpoint connections.</remarks>
	public class LocalNetworkConnection : INetworkConnection
	{
		#region Fields

		/// <summary>The initial size for all the queues.</summary>
		private const int initialMessageCapacity = 10;

		/// <summary>The outgoing message to use for serialization and sending (there is only one since the use case is to request exactly one, use it, then return it).</summary>
		private readonly OutgoingMessage outgoingMessage;
		/// <summary>The queue of messages that will be arriving next.</summary>
		private readonly Queue<IncomingMessage> nextIncomingMessages;
		/// <summary>The unused queue of messages that will be used as a pool for incoming messages that would arrive next.</summary>
		private readonly Queue<IncomingMessage> unusedIncomingMessages;
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

			this.outgoingMessage = new OutgoingMessage(new byte[this.MaxMessageSize]);
			this.nextIncomingMessages = new Queue<IncomingMessage>(LocalNetworkConnection.initialMessageCapacity);
			this.unusedIncomingMessages = new Queue<IncomingMessage>(LocalNetworkConnection.initialMessageCapacity);
			for (int i = 0; i < LocalNetworkConnection.initialMessageCapacity; i++)
			{
				this.unusedIncomingMessages.Enqueue(new IncomingMessage(new byte[this.MaxMessageSize]));
			}
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
			if (this.nextIncomingMessages.Count == 0) { return null; }
			IncomingMessage nextIncomingMessage = this.nextIncomingMessages.Dequeue();
			this.unusedIncomingMessages.Enqueue(nextIncomingMessage);
			return nextIncomingMessage;
		}

		/// <summary>
		/// Returns an <see cref="OutgoingMessage"/> that can be sent.
		/// </summary>
		public OutgoingMessage GetOutgoingMessageToSend()
		{
			this.outgoingMessage.Reset();
			return this.outgoingMessage;
		}

		/// <summary>
		/// Sends the given message over the network to the other endpoint.
		/// </summary>
		public void SendMessage(OutgoingMessage outgoingMessage)
		{
			if (this.pairedNetworkConnection == null) { return; }
			IncomingMessage nextIncomingMessage = this.pairedNetworkConnection.unusedIncomingMessages.Dequeue();
			nextIncomingMessage.Reset();
			outgoingMessage.CopyTo(nextIncomingMessage);
			this.pairedNetworkConnection.nextIncomingMessages.Enqueue(nextIncomingMessage);
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
