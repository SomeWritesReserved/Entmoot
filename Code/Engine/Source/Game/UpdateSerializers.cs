﻿using System;
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
		/// Serializes the given server update (entity snapshot and client-specific data) and immediately sends a packet, only writing data that has changed from a previous snapshot..
		/// </summary>
		public static void Send(INetworkConnection clientNetworkConnection, EntitySnapshot previousEntitySnapshot, EntitySnapshot latestEntitySnapshot, int latestClientTickReceived, int clientCommandingEntityID)
		{
			OutgoingMessage outgoingMessage = clientNetworkConnection.GetOutgoingMessageToSend();
			ServerUpdateSerializer.Serialize(outgoingMessage, previousEntitySnapshot, latestEntitySnapshot, latestClientTickReceived, clientCommandingEntityID);
			clientNetworkConnection.SendMessage(outgoingMessage);
		}

		/// <summary>
		/// Serializes the given server update (entity snapshot and client-specific data) to the given writer, only writing data that has changed from a previous snapshot.
		/// </summary>
		public static void Serialize(IWriter writer, EntitySnapshot previousEntitySnapshot, EntitySnapshot latestEntitySnapshot, int latestClientTickReceived, int clientCommandingEntityID)
		{
			Log<LogServerUpdateSerialization>.StartNew();
			Log<LogServerUpdateSerialization>.Data.SerializationTime.Start();
			writer.Write(latestClientTickReceived);
			writer.Write(clientCommandingEntityID);
			writer.Write((previousEntitySnapshot == null) ? -1 : previousEntitySnapshot.ServerFrameTick);
			latestEntitySnapshot.Serialize(previousEntitySnapshot, writer);
			Log<LogServerUpdateSerialization>.Data.SerializationTime.Stop();
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given reader, basing incoming data on a previous snapshot's data.
		/// </summary>
		public static void Deserialize(IReader reader, EntitySnapshot[] previousEntitySnapshots, EntitySnapshot latestEntitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID)
		{
			latestClientTickAcknowledgedByServer = reader.ReadInt32();
			clientCommandingEntityID = reader.ReadInt32();
			int previousServerFrameTick = reader.ReadInt32();
			EntitySnapshot previousEntitySnapshot = null;
			foreach (EntitySnapshot entitySnapshot in previousEntitySnapshots)
			{
				if (entitySnapshot.ServerFrameTick == previousServerFrameTick)
				{
					previousEntitySnapshot = entitySnapshot;
					break;
				}
			}
			latestEntitySnapshot.Deserialize(previousEntitySnapshot, reader);
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given reader, basing incoming data on a previous snapshot's data.
		/// Only overwrites if the reader contains a newer server update. Returns true if the entity snapshot was actually updated from the reader.
		/// </summary>
		public static bool DeserializeIfNewer(IReader reader, EntitySnapshot[] previousEntitySnapshots, EntitySnapshot latestEntitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID)
		{
			Log<LogServerUpdateDeserialization>.StartNew();
			Log<LogServerUpdateDeserialization>.Data.DeserializationTime.Start();
			latestClientTickAcknowledgedByServer = reader.ReadInt32();
			clientCommandingEntityID = reader.ReadInt32();
			int previousServerFrameTick = reader.ReadInt32();
			EntitySnapshot previousEntitySnapshot = null;
			foreach (EntitySnapshot entitySnapshot in previousEntitySnapshots)
			{
				if (entitySnapshot.ServerFrameTick == previousServerFrameTick)
				{
					previousEntitySnapshot = entitySnapshot;
					break;
				}
			}
			bool result = latestEntitySnapshot.DeserializeIfNewer(previousEntitySnapshot, reader);
			Log<LogServerUpdateDeserialization>.Data.DeserializationTime.Stop();
			return result;
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
		public static void Send(INetworkConnection serverNetworkConnection, Queue<ClientCommand<TCommandData>> clientCommands, int latestServerTickReceived, int latestFrameTickAcknowledgedByServer)
		{
			OutgoingMessage outgoingMessage = serverNetworkConnection.GetOutgoingMessageToSend();
			ClientUpdateSerializer<TCommandData>.Serialize(outgoingMessage, clientCommands, latestServerTickReceived, latestFrameTickAcknowledgedByServer);
			serverNetworkConnection.SendMessage(outgoingMessage);
		}

		/// <summary>
		/// Serializes the given client update (client commands) to the given writer.
		/// </summary>
		public static void Serialize(IWriter writer, Queue<ClientCommand<TCommandData>> clientCommands, int latestServerTickReceived, int latestFrameTickAcknowledgedByServer)
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

			Log<LogGameClient>.Data.SentCommands += numberOfCommands;
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

	/// <summary>
	/// Log data for ServerUpdateSerializer serialization.
	/// </summary>
	public struct LogServerUpdateSerialization
	{
		#region Fields

		/// <summary>The amount of time it takes for the server to fully serialize a server update.</summary>
		public LogTimer SerializationTime;

		#endregion Fields
	}

	/// <summary>
	/// Log data for ServerUpdateSerializer deserialization.
	/// </summary>
	public struct LogServerUpdateDeserialization
	{
		#region Fields

		/// <summary>The amount of time it takes for the client to fully deserialize a server update.</summary>
		public LogTimer DeserializationTime;

		#endregion Fields
	}
}
