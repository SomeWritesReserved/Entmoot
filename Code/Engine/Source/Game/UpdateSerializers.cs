using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// A helper class for serializing and deserializing server updates (entity snapshot and client-specific data) to clients.
	/// </summary>
	public static class ServerUpdateSerializer
	{
		#region Methods

		/// <summary>
		/// Serializes the given server update (entity snapshot and client-specific data) and immediately sends a packet.
		/// </summary>
		public static void Send(INetworkConnection clientNetworkConnection, EntitySnapshot entitySnapshot, int latestClientTickReceived, int clientCommandingEntityID)
		{
			OutgoingMessage outgoingMessage = clientNetworkConnection.GetOutgoingMessageToSend();
			ServerUpdateSerializer.Serialize(outgoingMessage, entitySnapshot, latestClientTickReceived, clientCommandingEntityID);
			clientNetworkConnection.SendMessage(outgoingMessage);
		}

		/// <summary>
		/// Serializes the given server update (entity snapshot and client-specific data) to the given writer.
		/// </summary>
		public static void Serialize(IWriter writer, EntitySnapshot entitySnapshot, int latestClientTickReceived, int clientCommandingEntityID)
		{
			writer.Write(latestClientTickReceived);
			writer.Write(clientCommandingEntityID);
			entitySnapshot.Serialize(writer);
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given reader.
		/// </summary>
		public static void Deserialize(IReader reader, EntitySnapshot entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID)
		{
			latestClientTickAcknowledgedByServer = reader.ReadInt32();
			clientCommandingEntityID = reader.ReadInt32();
			entitySnapshot.Deserialize(reader);
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given reader, but only if the reader contains a newer server update.
		/// Returns true if the entity snapshot was actually updated from the reader.
		/// </summary>
		public static bool DeserializeIfNewer(IReader reader, EntitySnapshot entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID)
		{
			latestClientTickAcknowledgedByServer = reader.ReadInt32();
			clientCommandingEntityID = reader.ReadInt32();
			return entitySnapshot.DeserializeIfNewer(reader);
		}

		#endregion Methods
	}

	/// <summary>
	/// A helper class for serializing and deserializing client updates (client commands) to the server.
	/// </summary>
	/// <typeparam name="TCommandData">The type of data that will be sent to the server as a command.</typeparam>
	public static class ClientUpdateSerializer<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Methods

		/// <summary>
		/// Serializes the given client update (client commands) and immediately sends a packet.
		/// </summary>
		public static void Send(INetworkConnection serverNetworkConnection, IEnumerable<ClientCommand<TCommandData>> clientCommands, int latestServerTickReceived, int latestFrameTickAcknowledgedByServer)
		{
			OutgoingMessage outgoingMessage = serverNetworkConnection.GetOutgoingMessageToSend();
			ClientUpdateSerializer<TCommandData>.Serialize(outgoingMessage, clientCommands, latestServerTickReceived, latestFrameTickAcknowledgedByServer);
			serverNetworkConnection.SendMessage(outgoingMessage);
		}

		/// <summary>
		/// Serializes the given client update (client commands) to the given writer.
		/// </summary>
		public static void Serialize(IWriter writer, IEnumerable<ClientCommand<TCommandData>> clientCommands, int latestServerTickReceived, int latestFrameTickAcknowledgedByServer)
		{
			writer.Write(latestServerTickReceived);

			// Remember where the command count should go, we'll overwrite it later once we know the real count
			int positionOfCommandCount = writer.Length;
			writer.Write((byte)0);

			byte numberOfCommands = 0;
			foreach (ClientCommand<TCommandData> clientCommand in clientCommands)
			{
				// Don't send the server commands its already received and acknowledged
				if (clientCommand.ClientFrameTick <= latestFrameTickAcknowledgedByServer) { continue; }

				clientCommand.Serialize(writer);
				numberOfCommands++;
			}
			writer.WriteAt(positionOfCommandCount, numberOfCommands);
		}

		/// <summary>
		/// Deserializes a client update (client commands) based on the given reader, overwriting the given array of commands with what was
		/// read. Returns the number of commands received (which may be less than the given array's length).
		/// </summary>
		public static int Deserialize(IReader reader, ClientCommand<TCommandData>[] clientCommands, out int latestFrameTickAcknowledgedByClient)
		{
			latestFrameTickAcknowledgedByClient = reader.ReadInt32();
			byte numberOfCommands = reader.ReadByte();
			for (int i = 0; i < numberOfCommands; i++)
			{
				clientCommands[i].Deserialize(reader);
			}
			return numberOfCommands;
		}

		#endregion Methods
	}
}
