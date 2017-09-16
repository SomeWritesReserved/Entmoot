using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a snapshot in time of entity state on the server.
	/// </summary>
	public class EntitySnapshot
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntitySnapshot(int entityCapacity, ComponentsDefinition componentsDefinition)
		{
			this.ServerFrameTick = -1;
			this.EntityArray = new EntityArray(entityCapacity, componentsDefinition);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the frame tick on the server that this snapshot was taken at.
		/// </summary>
		public int ServerFrameTick { get; private set; }

		/// <summary>
		/// Gets the array of entities as they existed at the point in time at <see cref="ServerFrameTick"/>.
		/// </summary>
		public EntityArray EntityArray { get; }

		/// <summary>
		/// Gets whether or not this entity snapshot has been loaded with data.
		/// </summary>
		public bool HasData { get { return (this.ServerFrameTick >= 0); } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates this entity snapshot to be the same as the given other snapshot.
		/// </summary>
		public void CopyFrom(EntitySnapshot other)
		{
			this.UpdateFrom(other.ServerFrameTick, other.EntityArray);
		}

		/// <summary>
		/// Updates this entity snapshot to be a new snapshot at a new point in time, given the new server frame tick and new entities.
		/// </summary>
		public void UpdateFrom(int serverFrameTick, EntityArray other)
		{
			this.ServerFrameTick = serverFrameTick;
			other.CopyTo(this.EntityArray);
		}

		/// <summary>
		/// Updates this entity snapshot to be a new snapshot that is an interpolated state between two other snapshots.
		/// </summary>
		public void Interpolate(EntitySnapshot otherA, EntitySnapshot otherB, int interpolationFrameTick, int serverFrameTick)
		{
			float amount = ((float)interpolationFrameTick - otherA.ServerFrameTick) / ((float)otherB.ServerFrameTick - otherA.ServerFrameTick);
			this.EntityArray.Interpolate(otherA.EntityArray, otherB.EntityArray, amount);
			this.ServerFrameTick = serverFrameTick;
		}

		/// <summary>
		/// Writes this entity snapshot's data to a binary source.
		/// </summary>
		public void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.ServerFrameTick);
			this.EntityArray.Serialize(binaryWriter);
		}

		/// <summary>
		/// Reads and overwrites this entity snapshot with data from a binary source.
		/// </summary>
		public void Deserialize(BinaryReader binaryReader)
		{
			this.ServerFrameTick = binaryReader.ReadInt32();
			this.EntityArray.Deserialize(binaryReader);
		}

		/// <summary>
		/// Reads and overwrites this entity snapshot with data from a binary source, but only if the binary source represents a newer snapshot.
		/// Returns true if this snapshot was actually updated from the deserialied binary source.
		/// </summary>
		public bool DeserializeIfNewer(BinaryReader binaryReader)
		{
			int newServerFrameTick = binaryReader.ReadInt32();
			if (newServerFrameTick <= this.ServerFrameTick) { return false; }

			this.ServerFrameTick = newServerFrameTick;
			this.EntityArray.Deserialize(binaryReader);
			return true;
		}

		#endregion Methods
	}

	/// <summary>
	/// A helper class for serializing and deserializing server updates (entity snapshot and client-specific data) to clients.
	/// </summary>
	public static class ServerUpdateSerializer
	{
		#region Methods

		/// <summary>
		/// Serializes the given server update (entity snapshot and client-specific data) and immediately sends a packet.
		/// </summary>
		public static void Serialize(INetworkConnection clientNetworkConnection, EntitySnapshot entitySnapshot, int latestClientTickReceived, int clientCommandingEntity)
		{
			clientNetworkConnection.SendPacket(ServerUpdateSerializer.Serialize(entitySnapshot, latestClientTickReceived, clientCommandingEntity));
		}

		/// <summary>
		/// Returns a serialized byte array for the given server update (entity snapshot and client-specific data).
		/// </summary>
		public static byte[] Serialize(EntitySnapshot entitySnapshot, int latestClientTickReceived, int clientCommandingEntity)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(latestClientTickReceived);
					binaryWriter.Write(clientCommandingEntity);
					entitySnapshot.Serialize(binaryWriter);
					return memoryStream.ToArray();
				}
			}
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given byte array.
		/// </summary>
		public static void Deserialize(byte[] packet, EntitySnapshot entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntity)
		{
			using (MemoryStream memoryStream = new MemoryStream(packet))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					latestClientTickAcknowledgedByServer = binaryReader.ReadInt32();
					clientCommandingEntity = binaryReader.ReadInt32();
					entitySnapshot.Deserialize(binaryReader);
				}
			}
		}

		/// <summary>
		/// Deserializes a server update (entity snapshot and client-specific data) based on the given byte array, but only if the byte array represents a newer server update.
		/// Returns true if the entity snapshot was actually updated from the byte array.
		/// </summary>
		public static bool DeserializeIfNewer(byte[] packet, EntitySnapshot entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntity)
		{
			using (MemoryStream memoryStream = new MemoryStream(packet))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					latestClientTickAcknowledgedByServer = binaryReader.ReadInt32();
					clientCommandingEntity = binaryReader.ReadInt32();
					return entitySnapshot.DeserializeIfNewer(binaryReader);
				}
			}
		}

		#endregion Methods
	}

	public interface INetworkConnection
	{
		#region Methods

		byte[] GetNextIncomingPacket();
		void SendPacket(byte[] packet);

		#endregion Methods
	}

	public interface ICommandData
	{
		#region Methods

		void DeserializeData(BinaryReader binaryReader);
		void SerializeData(BinaryWriter binaryWriter);

		void ApplyToEntity(Entity entity);

		#endregion Methods
	}

	public class ClientCommand<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Fields

		public int ClientFrameTick = -1;
		public int AcknowledgedServerTick = -1;
		public int InterpolationStartTick = -1;
		public int InterpolationEndTick = -1;
		public int RenderedFrameTick = -1;
		public int CommandingEntity = -1;
		public TCommandData CommandData = default(TCommandData);

		#endregion Fields

		#region Methods

		public static ClientCommand<TCommandData>[] DeserializePacket(byte[] packet)
		{
			using (MemoryStream memoryStream = new MemoryStream(packet, 0, packet.Length, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					List<ClientCommand<TCommandData>> clientCommands = new List<ClientCommand<TCommandData>>(32);
					while (memoryStream.Position < memoryStream.Length)
					{
						ClientCommand<TCommandData> clientCommand = new ClientCommand<TCommandData>()
						{
							ClientFrameTick = binaryReader.ReadInt32(),
							AcknowledgedServerTick = binaryReader.ReadInt32(),
							InterpolationStartTick = binaryReader.ReadInt32(),
							InterpolationEndTick = binaryReader.ReadInt32(),
							RenderedFrameTick = binaryReader.ReadInt32(),
							CommandingEntity = binaryReader.ReadInt32(),
						};
						clientCommand.CommandData.DeserializeData(binaryReader);
						clientCommands.Add(clientCommand);
					}
					return clientCommands.ToArray();
				}
			}
		}

		public static byte[] SerializeCommands(ClientCommand<TCommandData>[] clientCommands)
		{
			using (MemoryStream memoryStream = new MemoryStream(256))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					foreach (ClientCommand<TCommandData> clientCommand in clientCommands)
					{
						binaryWriter.Write(clientCommand.ClientFrameTick);
						binaryWriter.Write(clientCommand.AcknowledgedServerTick);
						binaryWriter.Write(clientCommand.InterpolationStartTick);
						binaryWriter.Write(clientCommand.InterpolationEndTick);
						binaryWriter.Write(clientCommand.RenderedFrameTick);
						binaryWriter.Write(clientCommand.CommandingEntity);
						clientCommand.CommandData.SerializeData(binaryWriter);
					}
				}
				return memoryStream.ToArray();
			}
		}

		#endregion Methods
	}
}
