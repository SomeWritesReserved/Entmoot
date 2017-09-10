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

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates this snapshot to be a new snapshot at a new point in time, given the new server frame tick and new entities.
		/// </summary>
		public void UpdateFrom(int serverFrameTick, EntityArray entityArray)
		{
			this.ServerFrameTick = serverFrameTick;
			entityArray.CopyTo(this.EntityArray);
		}

		/// <summary>
		/// Writes this snapshot's data to a binary source.
		/// </summary>
		public void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.ServerFrameTick);
			this.EntityArray.Serialize(binaryWriter);
		}

		/// <summary>
		/// Reads and overwrites this snapshot with data from a binary source.
		/// </summary>
		public void Deserialize(BinaryReader binaryReader)
		{
			this.ServerFrameTick = binaryReader.ReadInt32();
			this.EntityArray.Deserialize(binaryReader);
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
