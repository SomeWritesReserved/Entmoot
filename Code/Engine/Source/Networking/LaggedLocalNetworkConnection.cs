using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a connection between two local endpoints that communicate directly through memory, but introduces predictable simulated lag.
	/// </summary>
	/// <remarks>This is not thread safe, it should only be used in a synchronous and sequential manner.
	/// This is designed to not generate garbage between either endpoint connections.</remarks>
	public class LaggedLocalNetworkConnection : INetworkConnection
	{
		#region Fields

		/// <summary>The outgoing message to use for serialization and sending (there is only one since the use case is to request exactly one, use it, then return it).</summary>
		private readonly OutgoingMessage outgoingMessage;
		/// <summary>The queue of messages that have arrived and will be returned next.</summary>
		private readonly Queue<LaggedMessage> nextIncomingMessages;
		/// <summary>The queue of unused messages that will be used as a pool for incoming messages that could arrive next (to reduce GC pressure).</summary>
		private readonly Queue<LaggedMessage> pooledIncomingMessages;

		/// <summary>The corresponding network connection that represents the opposite endpoint.</summary>
		private LaggedLocalNetworkConnection pairedNetworkConnection;

		/// <summary>
		/// The stopwatch to use to measure simulated lag before allowing next messages to get through. This can be problematic though because when debugging the timer keeps
		/// going when the game is paused which breaks the simulated lag once you resume (all the lagged messages will come in immediately). A better approach would be based
		/// on game ticks but this layer of code doesn't know anything about the game or its ticks.
		/// </summary>
		private readonly Stopwatch stopwatch;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public LaggedLocalNetworkConnection(int maxMessageSize, double simulatedLatency)
		{
			this.MaxMessageSize = maxMessageSize;
			this.SimulatedLatency = simulatedLatency;
			this.stopwatch = Stopwatch.StartNew();

			const int initialQueueCapacity = 4;
			this.outgoingMessage = new OutgoingMessage(new byte[this.MaxMessageSize]);
			this.nextIncomingMessages = new Queue<LaggedMessage>(initialQueueCapacity);
			this.pooledIncomingMessages = new Queue<LaggedMessage>(initialQueueCapacity);
			for (int i = 0; i < initialQueueCapacity; i++)
			{
				this.pooledIncomingMessages.Enqueue(new LaggedMessage(new IncomingMessage(new byte[this.MaxMessageSize])));
			}
		}

		/// <summary>
		/// Constructor for creating a linked pair of network connections.
		/// </summary>
		private LaggedLocalNetworkConnection(LaggedLocalNetworkConnection pairedNetworkConnection) : this(pairedNetworkConnection.MaxMessageSize, pairedNetworkConnection.SimulatedLatency)
		{
			this.pairedNetworkConnection = pairedNetworkConnection;
			this.stopwatch = pairedNetworkConnection.stopwatch;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets that maximum size in bytes for a single message.
		/// </summary>
		public int MaxMessageSize { get; }

		/// <summary>
		/// Gets whether or not this local connection is actually connected to another endpoint.
		/// </summary>
		public bool IsConnected { get { return this.pairedNetworkConnection != null; } }

		/// <summary>
		/// Gets or sets the amount of time it takes to send a packet from endpoint to endpoint, measured in milliseconds.
		/// </summary>
		public double SimulatedLatency { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns a paired <see cref="LaggedLocalNetworkConnection"/> that represents the other endpoint.
		/// </summary>
		public LaggedLocalNetworkConnection GetPairedNetworkConnection()
		{
			if (this.pairedNetworkConnection == null) { this.pairedNetworkConnection = new LaggedLocalNetworkConnection(this); }
			return this.pairedNetworkConnection;
		}

		/// <summary>
		/// Returns the next message that is coming in over the network. Returns null if no message is ready.
		/// </summary>
		public IncomingMessage GetNextIncomingMessage()
		{
			if (this.nextIncomingMessages.Count == 0) { return null; }

			double now = this.stopwatch.Elapsed.TotalMilliseconds;
			if (this.nextIncomingMessages.Peek().LaggedArrivalTime > now) { return null; }

			LaggedMessage nextLaggedMessage = this.nextIncomingMessages.Dequeue();
			this.pooledIncomingMessages.Enqueue(nextLaggedMessage);
			nextLaggedMessage.IncomingMessage.Reset();
			return nextLaggedMessage.IncomingMessage;
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
			LaggedMessage nextLaggedMessage = this.pairedNetworkConnection.getMessageToAddToIncomingQueue();

			double now = this.stopwatch.Elapsed.TotalMilliseconds;
			nextLaggedMessage.LaggedArrivalTime = now + this.SimulatedLatency;
			nextLaggedMessage.IncomingMessage.CopyFrom(outgoingMessage);
		}

		/// <summary>
		/// Returns a message that will be added to the queue of messages that have arrived. This will always return a
		/// message and the message is immediately added to the queue. The consumer must fill in the data of the message.
		/// </summary>
		private LaggedMessage getMessageToAddToIncomingQueue()
		{
			LaggedMessage pooledLaggedMessage;
			if (this.pooledIncomingMessages.Count == 0)
			{
				pooledLaggedMessage = new LaggedMessage(new IncomingMessage(new byte[this.MaxMessageSize]));
			}
			else
			{
				pooledLaggedMessage = this.pooledIncomingMessages.Dequeue();
			}
			this.nextIncomingMessages.Enqueue(pooledLaggedMessage);
			pooledLaggedMessage.IncomingMessage.Clear();
			return pooledLaggedMessage;
		}

		#endregion Methods

		#region Nested Types

		private class LaggedMessage
		{
			#region Constructors

			public LaggedMessage(IncomingMessage incomingMessage)
			{
				this.IncomingMessage = incomingMessage;
			}

			#endregion Constructors

			#region Properties

			public double LaggedArrivalTime { get; set; }

			public IncomingMessage IncomingMessage { get; }

			#endregion Properties
		}

		#endregion Nested Types
	}
}
