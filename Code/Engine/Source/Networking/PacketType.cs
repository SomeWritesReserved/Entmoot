using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Defines a specific type of packet sent for a specific reason.
	/// </summary>
	public enum PacketType : byte
	{
		/// <summary>No packet type defined (invalid).</summary>
		None = 0,
		/// <summary>A packet that contains typical game update data (to be processed by the underlying game).</summary>
		GameUpdate,
		/// <summary>A packet that represents a client's attempt to connect.</summary>
		ClientConnectRequest,
		/// <summary>A packet that is the server's response to a client's attempt to connect.</summary>
		ServerConnectResponse,
		/// <summary>A packet that is the client acknwoledging the server's response, confirming and finalizing the client's connection</summary>
		ClientConnectFinalize,
	}

	/// <summary>
	/// Defines a more specific detail or reason for a given <see cref="PacketType"/>.
	/// </summary>
	public enum PacketTypeDetail : byte
	{
		/// <summary></summary>
		None = 0,
		/// <summary>A <see cref="PacketType.GameUpdate"/> packet that is specifically coming from the server (to be received by a client).</summary>
		GameUpdateFromServer,
		/// <summary>A <see cref="PacketType.GameUpdate"/> packet that is specifically coming from a client (to be received by the server).</summary>
		GameUpdateFromClient,
		/// <summary>A <see cref="PacketType.ServerConnectResponse"/> packet where the server is accepting the client's attempt to connect.</summary>
		ConnectResponseAccept,
		/// <summary>A <see cref="PacketType.ServerConnectResponse"/> packet where the server rejects a client's connection due to an application ID mismatch.</summary>
		ConnectResponseRejectAppMismatch,
		/// <summary>A <see cref="PacketType.ServerConnectResponse"/> packet where the server rejects a client's connection due to the server being full (all client slots are taken).</summary>
		ConnectResponseRejectServerFull,
	}
}
