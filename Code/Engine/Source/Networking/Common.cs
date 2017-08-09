﻿using System;
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

		public int ServerFrameTick = -1;
		public int AcknowledgedClientTick = -1;
		public int ClientOwnedEntity = -1;
		public Entity[] Entities;

		#endregion Fields

		#region Methods

		public static StateSnapshot Clone(StateSnapshot stateSnapshot)
		{
			Entity[] interpolatedEntities = new Entity[stateSnapshot.Entities.Length];
			foreach (int entityIndex in Enumerable.Range(0, interpolatedEntities.Length))
			{
				interpolatedEntities[entityIndex] = new Entity()
				{
					Position = stateSnapshot.Entities[entityIndex].Position,
				};
			}
			return new StateSnapshot()
			{
				ServerFrameTick = stateSnapshot.ServerFrameTick,
				Entities = interpolatedEntities,
			};
		}

		public static StateSnapshot Interpolate(StateSnapshot stateSnapshotA, StateSnapshot stateSnapshotB, int frameTick)
		{
			float amount = ((float)frameTick - stateSnapshotA.ServerFrameTick) / ((float)stateSnapshotB.ServerFrameTick - stateSnapshotA.ServerFrameTick);

			Entity[] interpolatedEntities = new Entity[stateSnapshotA.Entities.Length];
			foreach (int entityIndex in Enumerable.Range(0, interpolatedEntities.Length))
			{
				interpolatedEntities[entityIndex] = new Entity()
				{
					Position = Vector3.Interpolate(stateSnapshotA.Entities[entityIndex].Position, stateSnapshotB.Entities[entityIndex].Position, amount),
				};
			}
			return new StateSnapshot()
			{
				ServerFrameTick = frameTick,
				Entities = interpolatedEntities,
			};
		}

		public static StateSnapshot DeserializePacket(byte[] packet)
		{
			StateSnapshot stateSnapshot = new StateSnapshot();
			using (MemoryStream memoryStream = new MemoryStream(packet, 0, packet.Length, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					stateSnapshot.ServerFrameTick = binaryReader.ReadInt32();
					stateSnapshot.AcknowledgedClientTick = binaryReader.ReadInt32();
					stateSnapshot.ClientOwnedEntity = binaryReader.ReadInt32();
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
			byte[] packet = new byte[sizeof(int) + sizeof(int) + sizeof(int) + sizeof(float) * 3 * this.Entities.Length];
			using (MemoryStream memoryStream = new MemoryStream(packet, 0, packet.Length, true))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.ServerFrameTick);
					binaryWriter.Write(this.AcknowledgedClientTick);
					binaryWriter.Write(this.ClientOwnedEntity);
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
		#region Methods

		byte[] GetNextIncomingPacket();

		void SendPacket(byte[] packet);

		#endregion Methods
	}

	public interface ISystem
	{
		#region Properties

		int FrameTick { get; }

		#endregion Properties
	}

	public class ClientCommand
	{
		#region Fields

		public int ClientFrameTick = -1;
		public int AcknowledgedServerTick = -1;
		public int InterpolationStartTick = -1;
		public int InterpolationEndTick = -1;
		public int RenderedFrameTick = -1;
		public int CommandingEntity = -1;
		public CommandKeys CommandKeys;

		#endregion Fields

		#region Methods

		public static ClientCommand[] DeserializePacket(byte[] packet)
		{
			using (MemoryStream memoryStream = new MemoryStream(packet, 0, packet.Length, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					List<ClientCommand> clientCommands = new List<ClientCommand>(32);
					while (memoryStream.Position < memoryStream.Length)
					{
						clientCommands.Add(new ClientCommand()
						{
							ClientFrameTick = binaryReader.ReadInt32(),
							AcknowledgedServerTick = binaryReader.ReadInt32(),
							InterpolationStartTick = binaryReader.ReadInt32(),
							InterpolationEndTick = binaryReader.ReadInt32(),
							RenderedFrameTick = binaryReader.ReadInt32(),
							CommandingEntity = binaryReader.ReadInt32(),
							CommandKeys = (CommandKeys)binaryReader.ReadByte(),
						});
					}
					return clientCommands.ToArray();
				}
			}
		}

		public static byte[] SerializeCommands(ClientCommand[] clientCommands)
		{
			byte[] packet = new byte[(sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(byte)) * clientCommands.Length];
			using (MemoryStream memoryStream = new MemoryStream(packet, 0, packet.Length, true))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					foreach (ClientCommand clientCommand in clientCommands)
					{
						binaryWriter.Write(clientCommand.ClientFrameTick);
						binaryWriter.Write(clientCommand.AcknowledgedServerTick);
						binaryWriter.Write(clientCommand.InterpolationStartTick);
						binaryWriter.Write(clientCommand.InterpolationEndTick);
						binaryWriter.Write(clientCommand.RenderedFrameTick);
						binaryWriter.Write(clientCommand.CommandingEntity);
						binaryWriter.Write((byte)clientCommand.CommandKeys);
					}
				}
			}
			return packet;
		}

		public void RunOnEntity(Entity entity)
		{
			if ((this.CommandKeys & CommandKeys.MoveForward) != 0) { entity.Position.Y -= 5; }
			if ((this.CommandKeys & CommandKeys.MoveBackward) != 0) { entity.Position.Y += 5; }
			if ((this.CommandKeys & CommandKeys.MoveLeft) != 0) { entity.Position.X -= 5; }
			if ((this.CommandKeys & CommandKeys.MoveRight) != 0) { entity.Position.X += 5; }
		}

		#endregion Methods
	}

	public enum CommandKeys : byte
	{
		None = 0,
		MoveForward = 1,
		MoveBackward = 2,
		MoveLeft = 4,
		MoveRight = 8,
		Seat1 = 16,
		Seat2 = 32,
		Fire = 64,
	}
}
