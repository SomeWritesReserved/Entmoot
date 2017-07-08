using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public class StateSnapshot
	{
		#region Fields

		public int FrameTick;
		public Entity[] Entities;

		#endregion Fields

		#region Methods

		public static StateSnapshot DeserializePacket(byte[] packet)
		{
			StateSnapshot stateSnapshot = new StateSnapshot();
			using (MemoryStream memoryStream = new MemoryStream(packet, 0, packet.Length, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					stateSnapshot.FrameTick = binaryReader.ReadInt32();
					List<Entity> entities = new List<Entity>(16);
					while (memoryStream.Position < memoryStream.Length)
					{
						entities.Add(new Entity()
						{
							Position = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
						});
					}
					stateSnapshot.Entities = entities.ToArray();
				}
			}
			return stateSnapshot;
		}

		public byte[] SerializePacket()
		{
			byte[] packet = new byte[sizeof(int) + sizeof(float) * 3 * this.Entities.Length];
			using (MemoryStream memoryStream = new MemoryStream(packet, 0, packet.Length, true))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.FrameTick);
					foreach (Entity entity in this.Entities)
					{
						binaryWriter.Write(entity.Position.X);
						binaryWriter.Write(entity.Position.Y);
						binaryWriter.Write(entity.Position.Z);
					}
				}
			}
			return packet;
		}

		#endregion Methods
	}

	public interface INetworkConnection
	{
		#region Properties

		bool HasIncomingPackets { get; }

		#endregion Properties

		#region Methods

		byte[] GetNextIncomingPacket();

		void SendPacket(byte[] packet);

		#endregion Methods
	}
}
