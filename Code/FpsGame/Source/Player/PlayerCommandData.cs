using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	public struct PlayerCommandData : ICommandData
	{
		#region Fields

		public const float MoveImpulseAccelerationAmount = 75.0f;

		public PlayerInputFlags PlayerInput;
		public Vector2 LookAngles;

		#endregion Fields

		#region Methods

		public void Serialize(IWriter writer)
		{
			writer.Write((short)this.PlayerInput);
			writer.Write(this.LookAngles.X);
			writer.Write(this.LookAngles.Y);
		}

		public void Deserialize(IReader reader)
		{
			this.PlayerInput = (PlayerInputFlags)reader.ReadInt16();
			this.LookAngles.X = reader.ReadSingle();
			this.LookAngles.Y = reader.ReadSingle();
		}

		public void ApplyToEntity(Entity entity)
		{
			if (!entity.HasComponent<SpatialComponent>()) { return; }
			if (!entity.HasComponent<PhysicsComponent>()) { return; }

			Vector3 movement = Vector3.Zero;
			if ((this.PlayerInput & PlayerInputFlags.MoveForward) == PlayerInputFlags.MoveForward) { movement += Vector3.Forward; }
			if ((this.PlayerInput & PlayerInputFlags.MoveBackward) == PlayerInputFlags.MoveBackward) { movement += Vector3.Backward; }
			if ((this.PlayerInput & PlayerInputFlags.MoveLeft) == PlayerInputFlags.MoveLeft) { movement += Vector3.Left; }
			if ((this.PlayerInput & PlayerInputFlags.MoveRight) == PlayerInputFlags.MoveRight) { movement += Vector3.Right; }

			if (movement != Vector3.Zero)
			{
				movement.Normalize();
				Quaternion lookMoveRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, this.LookAngles.X);
				Vector3.Transform(ref movement, ref lookMoveRotation, out movement);
			}

			ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();
			ref PhysicsComponent physicsComponent = ref entity.GetComponent<PhysicsComponent>();
			physicsComponent.Acceleration += movement * PlayerCommandData.MoveImpulseAccelerationAmount;
			spatialComponent.Orientation = Quaternion.CreateFromYawPitchRoll(this.LookAngles.X, this.LookAngles.Y, 0.0f);
		}

		#endregion Methods
	}
}
