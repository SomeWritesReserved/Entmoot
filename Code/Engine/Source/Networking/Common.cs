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

	public interface INetworkConnection
	{
		#region Methods

		byte[] GetNextIncomingPacket();
		void SendPacket(byte[] packet);

		#endregion Methods
	}

	/// <summary>
	/// An interface that defines the actual data that a client command will send to the server.
	/// </summary>
	public interface ICommandData
	{
		#region Methods

		/// <summary>
		/// Writes the state of this command data to a binary source.
		/// </summary>
		void Serialize(BinaryWriter binaryWriter);

		/// <summary>
		/// Reads and overwrites the current state of this command data from a binary source.
		/// </summary>
		void Deserialize(BinaryReader binaryReader);

		/// <summary>
		/// Applies this command data to a given entity (whatever that may mean for the type of command).
		/// </summary>
		void ApplyToEntity(Entity entity);

		#endregion Methods
	}

	/// <summary>
	/// Represents a command from a client sent to and processed by the server (and used in client-side prediction).
	/// </summary>
	/// <typeparam name="TCommandData">The type of data that will be sent to the server as a command.</typeparam>
	public class ClientCommand<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Properties

		/// <summary>
		/// Gets the maximum number of clinet commands that will be sent to the server, per single update.
		/// </summary>
		public static int MaxClientCommandsPerUpdate { get { return 30; } }

		/// <summary>
		/// Gets the frame tick on the client that this command was taken at (which may be ahead of <see cref="RenderedTick"/>).
		/// </summary>
		public int ClientFrameTick { get; private set; } = -1;

		/// <summary>
		/// Gets the frame tick for what the client is rendering when this command was taken (which may be behind <see cref="ClientFrameTick"/>).
		/// </summary>
		public int RenderedTick { get; private set; } = -1;

		/// <summary>
		/// Gets the starting frame tick that the client is using to interpolate to the rendered frame (if interpolation was active);
		/// </summary>
		public int InterpolationStartTick { get; private set; } = -1;

		/// <summary>
		/// Gets the ending frame tick that the client is using to interpolate to the rendered frame (if interpolation was active);
		/// </summary>
		public int InterpolationEndTick { get; private set; } = -1;

		/// <summary>
		/// Gets the entity that the client was commanding when this command was taken (which the server may ignore if the client is out-of-date
		/// with what entity it should be commanding).
		/// </summary>
		public int CommandingEntityID { get; private set; } = -1;

		private TCommandData commandData;
		/// <summary>
		/// Gets the packaged data that the client command will send to the server.
		/// </summary>
		/// <remarks>
		/// This has a manually declared backing field (since its a struct) which you should always use, if not then the property getter creates a copy
		/// of the backing field which breaks deserialization (data gets deserialized into the copy, not into the backing field).
		/// </remarks>
		public TCommandData CommandData { get { return this.commandData; } }

		/// <summary>
		/// Gets whether or not this client command has been loaded with data.
		/// </summary>
		public bool HasData { get { return (this.ClientFrameTick >= 0); } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates this entity snapshot to be a new snapshot at a new point in time, given the new server frame tick and new entities.
		/// </summary>
		public void UpdateFrom(int clientFrameTick, int renderedTick, int interpolationStartTick, int interpolationEndTick, int commandingEntityID, TCommandData commandData)
		{
			this.ClientFrameTick = clientFrameTick;
			this.RenderedTick = renderedTick;
			this.InterpolationStartTick = interpolationStartTick;
			this.InterpolationEndTick = interpolationEndTick;
			this.CommandingEntityID = commandingEntityID;
			this.commandData = commandData;
		}

		/// <summary>
		/// Writes the state of this command to a binary source.
		/// </summary>
		public void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.ClientFrameTick);
			binaryWriter.Write(this.RenderedTick);
			binaryWriter.Write(this.InterpolationStartTick);
			binaryWriter.Write(this.InterpolationEndTick);
			binaryWriter.Write(this.CommandingEntityID);
			this.commandData.Serialize(binaryWriter);
		}

		/// <summary>
		/// Reads and overwrites the current state of this command from a binary source.
		/// </summary>
		public void Deserialize(BinaryReader binaryReader)
		{
			this.ClientFrameTick = binaryReader.ReadInt32();
			this.RenderedTick = binaryReader.ReadInt32();
			this.InterpolationStartTick = binaryReader.ReadInt32();
			this.InterpolationEndTick = binaryReader.ReadInt32();
			this.CommandingEntityID = binaryReader.ReadInt32();
			this.commandData.Deserialize(binaryReader);
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
