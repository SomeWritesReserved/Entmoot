using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.TestGame3D
{
	public struct SpatialComponent : IComponent<SpatialComponent>
	{
		#region Fields

		public Vector3 Position;
		public Quaternion Rotation;

		#endregion Fields

		#region Methods

		public bool Equals(SpatialComponent other)
		{
			return this.Position == other.Position && this.Rotation == other.Rotation;
		}

		public void Interpolate(SpatialComponent otherA, SpatialComponent otherB, float amount)
		{
			Vector3.Lerp(ref otherA.Position, ref otherB.Position, amount, out this.Position);
			Quaternion.Slerp(ref otherA.Rotation, ref otherB.Rotation, amount, out this.Rotation);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Position.Z);
			writer.Write(this.Rotation.X);
			writer.Write(this.Rotation.Y);
			writer.Write(this.Rotation.Z);
			writer.Write(this.Rotation.W);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Position.Z = reader.ReadSingle();
			this.Rotation.X = reader.ReadSingle();
			this.Rotation.Y = reader.ReadSingle();
			this.Rotation.Z = reader.ReadSingle();
			this.Rotation.W = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Position = Vector3.Zero;
			this.Rotation = Quaternion.Identity;
		}

		#endregion Methods
	}

	public struct PhysicsComponent : IComponent<PhysicsComponent>
	{
		#region Fields

		public Vector3 Velocity;
		public Vector3 Acceleration;

		#endregion Fields

		#region Methods

		public bool Equals(PhysicsComponent other)
		{
			return Vector3.Equals(this.Velocity, other.Velocity) &&
				Vector3.Equals(this.Acceleration, other.Acceleration);
		}

		public void Interpolate(PhysicsComponent otherA, PhysicsComponent otherB, float amount)
		{
			Vector3.Lerp(ref otherA.Velocity, ref otherB.Velocity, amount, out this.Velocity);
			Vector3.Lerp(ref otherA.Acceleration, ref otherB.Acceleration, amount, out this.Acceleration);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Velocity.X);
			writer.Write(this.Velocity.Y);
			writer.Write(this.Velocity.Z);
			writer.Write(this.Acceleration.X);
			writer.Write(this.Acceleration.Y);
			writer.Write(this.Acceleration.Z);
		}

		public void Deserialize(IReader reader)
		{
			this.Velocity.X = reader.ReadSingle();
			this.Velocity.Y = reader.ReadSingle();
			this.Velocity.Z = reader.ReadSingle();
			this.Acceleration.X = reader.ReadSingle();
			this.Acceleration.Y = reader.ReadSingle();
			this.Acceleration.Z = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Velocity = Vector3.Zero;
			this.Acceleration = Vector3.Zero;
		}

		#endregion Methods
	}

	public struct PlayerMovementComponent : IComponent<PlayerMovementComponent>
	{
		#region Fields

		public Vector3 Velocity;
		public Vector3 Acceleration;

		#endregion Fields

		#region Methods

		public bool Equals(PlayerMovementComponent other)
		{
			return Vector3.Equals(this.Velocity, other.Velocity) &&
				Vector3.Equals(this.Acceleration, other.Acceleration);
		}

		public void Interpolate(PlayerMovementComponent otherA, PlayerMovementComponent otherB, float amount)
		{
			Vector3.Lerp(ref otherA.Velocity, ref otherB.Velocity, amount, out this.Velocity);
			Vector3.Lerp(ref otherA.Acceleration, ref otherB.Acceleration, amount, out this.Acceleration);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Velocity.X);
			writer.Write(this.Velocity.Y);
			writer.Write(this.Velocity.Z);
			writer.Write(this.Acceleration.X);
			writer.Write(this.Acceleration.Y);
			writer.Write(this.Acceleration.Z);
		}

		public void Deserialize(IReader reader)
		{
			this.Velocity.X = reader.ReadSingle();
			this.Velocity.Y = reader.ReadSingle();
			this.Velocity.Z = reader.ReadSingle();
			this.Acceleration.X = reader.ReadSingle();
			this.Acceleration.Y = reader.ReadSingle();
			this.Acceleration.Z = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Velocity = Vector3.Zero;
			this.Acceleration = Vector3.Zero;
		}

		#endregion Methods
	}

	public struct ColorComponent : IComponent<ColorComponent>
	{
		#region Fields

		public Color Color;

		#endregion Fields

		#region Methods

		public bool Equals(ColorComponent other)
		{
			return this.Color == other.Color;
		}

		public void Interpolate(ColorComponent otherA, ColorComponent otherB, float amount)
		{
			this.Color = Color.Lerp(otherA.Color, otherB.Color, amount);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Color.R);
			writer.Write(this.Color.G);
			writer.Write(this.Color.B);
		}

		public void Deserialize(IReader reader)
		{
			this.Color.R = reader.ReadByte();
			this.Color.G = reader.ReadByte();
			this.Color.B = reader.ReadByte();
		}

		public void ResetToDefaults()
		{
			this.Color = Color.White;
		}

		#endregion Methods
	}
}
