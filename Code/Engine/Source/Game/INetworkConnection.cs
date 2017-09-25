using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a potential connection between two endpoints over the network.
	/// </summary>
	public interface INetworkConnection
	{
		#region Properties

		/// <summary>
		/// Gets whether or not this network connection is actually connected to another endpoint.
		/// </summary>
		bool IsConnected { get; }

		#endregion Properties

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
}
