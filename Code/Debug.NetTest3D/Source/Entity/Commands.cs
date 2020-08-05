using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.Debug.NetTest3D
{
	[Flags]
	public enum Commands : short
	{
		None,
		MoveForward = 1,
		MoveBackward = 2,
		MoveLeft = 4,
		MoveRight = 8,
		Jump = 16,
		Attack = 32,
	}

	public struct CommandData : ICommandData
	{
		#region Fields

		public const float MoveImpulse = 75.0f;

		public Commands Commands;
		public Vector2 LookAngles;

		#endregion Fields

		#region Methods

		public void Serialize(IWriter writer)
		{
			writer.Write((short)this.Commands);
			writer.Write(this.LookAngles.X);
			writer.Write(this.LookAngles.Y);
		}

		public void Deserialize(IReader reader)
		{
			this.Commands = (Commands)reader.ReadInt16();
			this.LookAngles.X = reader.ReadSingle();
			this.LookAngles.Y = reader.ReadSingle();
		}

		public void ApplyToEntity(Entity entity)
		{
			if (!entity.HasComponent<SpatialComponent>()) { return; }
			if (!entity.HasComponent<PhysicsComponent>()) { return; }

			Vector3 movement = Vector3.Zero;
			if ((this.Commands & Commands.MoveForward) != 0) { movement += Vector3.Forward; }
			if ((this.Commands & Commands.MoveBackward) != 0) { movement += Vector3.Backward; }
			if ((this.Commands & Commands.MoveLeft) != 0) { movement += Vector3.Left; }
			if ((this.Commands & Commands.MoveRight) != 0) { movement += Vector3.Right; }

			if (movement != Vector3.Zero)
			{
				movement.Normalize();
				Quaternion lookMoveRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, this.LookAngles.X);
				Vector3.Transform(ref movement, ref lookMoveRotation, out movement);
			}

			ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();
			ref PhysicsComponent physicsComponent = ref entity.GetComponent<PhysicsComponent>();
			physicsComponent.Acceleration += movement * CommandData.MoveImpulse;
			spatialComponent.Rotation = Quaternion.CreateFromYawPitchRoll(this.LookAngles.X, this.LookAngles.Y, 0.0f);
		}

		#endregion Methods
	}
}
