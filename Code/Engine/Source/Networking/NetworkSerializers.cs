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
			clientNetworkConnection.SendPacket(ServerUpdateSerializer.Serialize(entitySnapshot, latestClientTickReceived, clientCommandingEntityID));
		}

		/// <summary>
		/// Returns a serialized byte array for the given server update (entity snapshot and client-specific data).
		/// </summary>
		public static byte[] Serialize(EntitySnapshot entitySnapshot, int latestClientTickReceived, int clientCommandingEntityID)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(latestClientTickReceived);
					binaryWriter.Write(clientCommandingEntityID);
					entitySnapshot.Serialize(binaryWriter);
					return memoryStream.ToArray();
				}
			}
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given byte array.
		/// </summary>
		public static void Deserialize(byte[] packet, EntitySnapshot entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID)
		{
			using (MemoryStream memoryStream = new MemoryStream(packet))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					latestClientTickAcknowledgedByServer = binaryReader.ReadInt32();
					clientCommandingEntityID = binaryReader.ReadInt32();
					entitySnapshot.Deserialize(binaryReader);
				}
			}
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given byte array, but only if the byte array represents a newer server update.
		/// Returns true if the entity snapshot was actually updated from the byte array.
		/// </summary>
		public static bool DeserializeIfNewer(byte[] packet, EntitySnapshot entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID)
		{
			using (MemoryStream memoryStream = new MemoryStream(packet))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					latestClientTickAcknowledgedByServer = binaryReader.ReadInt32();
					clientCommandingEntityID = binaryReader.ReadInt32();
					return entitySnapshot.DeserializeIfNewer(binaryReader);
				}
			}
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
		public static void Send(INetworkConnection serverNetworkConnection, IEnumerable<ClientCommand<TCommandData>> clientCommands, int latestServerTickReceived)
		{
			serverNetworkConnection.SendPacket(ClientUpdateSerializer<TCommandData>.Serialize(clientCommands, latestServerTickReceived));
		}

		/// <summary>
		/// Returns a serialized byte array for the given client update (client commands).
		/// </summary>
		public static byte[] Serialize(IEnumerable<ClientCommand<TCommandData>> clientCommands, int latestServerTickReceived)
		{
			// Todo: only write the commands that have data and are newer than what the server already acknowledged (LatestFrameTickAcknowledgedByServer)
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(latestServerTickReceived);
					binaryWriter.Write((byte)clientCommands.Count());
					foreach (ClientCommand<TCommandData> clientCommand in clientCommands)
					{
						clientCommand.Serialize(binaryWriter);
					}
					return memoryStream.ToArray();
				}
			}
		}

		/// <summary>
		/// Deserializes a client update (client commands) based on the given byte array, overwriting the given array of commands with what was
		/// receieved. Returns the number of commands received (which may be less than given array's length).
		/// </summary>
		public static int Deserialize(byte[] packet, ClientCommand<TCommandData>[] clientCommands, out int latestFrameTickAcknowledgedByClient)
		{
			using (MemoryStream memoryStream = new MemoryStream(packet))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					latestFrameTickAcknowledgedByClient = binaryReader.ReadInt32();
					byte numberOfCommands = binaryReader.ReadByte();
					for (int i = 0; i < numberOfCommands; i++)
					{
						clientCommands[i].Deserialize(binaryReader);
					}
					return numberOfCommands;
				}
			}
		}

		#endregion Methods
	}
}
