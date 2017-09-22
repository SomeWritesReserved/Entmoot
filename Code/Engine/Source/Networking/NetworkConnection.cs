﻿using System;
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
		/// Returns the next packet that is coming in over the network. Returns null if no packet is ready.
		/// </summary>
		byte[] GetNextIncomingPacket();

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
