using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a buffer for creating and managing outgoing and incoming messages. Incoming messages will be pooled and
	/// queued for consumers to read in order. Outgoing messages are just pooled and returned to consumers.
	/// </summary>
	/// <remarks>This is not thread safe, it should only be used in a synchronous and sequential manner.
	/// This is designed to not generate garbage.</remarks>
	public class MessageBuffer
	{
		#region Fields

		/// <summary>The outgoing message to use for serialization and sending (there is only one since the use case is to request exactly one, use it, then return it).</summary>
		private readonly OutgoingMessage outgoingMessage;
		/// <summary>The queue of messages that have arrived and will be returned next.</summary>
		private readonly Queue<IncomingMessage> nextIncomingMessages;
		/// <summary>The queue of unused messages that will be used as a pool for incoming messages that could arrive next (to reduce GC pressure).</summary>
		private readonly Queue<IncomingMessage> pooledIncomingMessages;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public MessageBuffer(int maxMessageSize, int initialQueueCapacity)
		{
			this.MaxMessageSize = maxMessageSize;

			this.outgoingMessage = new OutgoingMessage(new byte[this.MaxMessageSize]);
			this.nextIncomingMessages = new Queue<IncomingMessage>(initialQueueCapacity);
			this.pooledIncomingMessages = new Queue<IncomingMessage>(initialQueueCapacity);
			for (int i = 0; i < initialQueueCapacity; i++)
			{
				this.pooledIncomingMessages.Enqueue(new IncomingMessage(new byte[this.MaxMessageSize]));
			}
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
		/// Returns the next message that has arrived. Returns null if no message is ready.
		/// </summary>
		public IncomingMessage GetNextIncomingMessage()
		{
			if (this.nextIncomingMessages.Count == 0) { return null; }
			IncomingMessage nextIncomingMessage = this.nextIncomingMessages.Dequeue();
			this.pooledIncomingMessages.Enqueue(nextIncomingMessage);
			nextIncomingMessage.Reset();
			return nextIncomingMessage;
		}

		/// <summary>
		/// Returns a message that will be added to the queue of messages that have arrived. This will always return a
		/// message and the message is immediately added to the queue. The consumer must fill in the data of the message.
		/// </summary>
		public IncomingMessage GetMessageToAddToIncomingQueue()
		{
			IncomingMessage pooledIncomingMessage;
			if (this.pooledIncomingMessages.Count == 0)
			{
				pooledIncomingMessage = new IncomingMessage(new byte[this.MaxMessageSize]);
			}
			else
			{
				pooledIncomingMessage = this.pooledIncomingMessages.Dequeue();
			}
			this.nextIncomingMessages.Enqueue(pooledIncomingMessage);
			pooledIncomingMessage.Clear();
			return pooledIncomingMessage;
		}

		/// <summary>
		/// Returns a message that can be used to send data.
		/// </summary>
		public OutgoingMessage CreateOutgoingMessage()
		{
			this.outgoingMessage.Reset();
			return this.outgoingMessage;
		}

		/// <summary>
		/// Clears this buffer to start from scratch (clears all incoming messages).
		/// </summary>
		public void Clear()
		{
			while (this.nextIncomingMessages.Count > 0)
			{
				IncomingMessage incomingMessage = this.nextIncomingMessages.Dequeue();
				incomingMessage.Clear();
				this.pooledIncomingMessages.Enqueue(incomingMessage);
			}
		}

		#endregion Methods
	}
}
