using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Sideswipe
{
	/// <summary>
	/// Represents an entity's ability to move around in the world.
	/// </summary>
	public struct PhysicsComponent : IComponent<PhysicsComponent>
	{
		#region Fields

		/// <summary>
		/// The velocity of this entity in the world, with the center of travel being the center of the sprite and the center of <see cref="SpatialComponent.Extents"/>.
		/// </summary>
		public Vector2 Velocity;

		/// <summary>
		/// The acceleration of this entity in the world, with the center of travel being the center of the sprite and the center of <see cref="SpatialComponent.Extents"/>.
		/// </summary>
		public Vector2 Acceleration;

		/// <summary>
		/// The number of frames this entity has been off the ground.
		/// </summary>
		public int OffGroundCount;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Whether or not this entity is on the "ground" (on top of some other entity).
		/// </summary>
		public bool IsOnGround => this.OffGroundCount == 0;

		#endregion Properties

		#region Methods

		public bool Equals(PhysicsComponent other)
		{
			return (this.Velocity.Equals(other.Velocity) &&
				this.Acceleration.Equals(other.Acceleration) &&
				this.OffGroundCount == other.OffGroundCount);
		}

		public void Interpolate(PhysicsComponent otherA, PhysicsComponent otherB, float amount)
		{
			Vector2.Lerp(ref otherA.Velocity, ref otherB.Velocity, amount, out this.Velocity);
			Vector2.Lerp(ref otherA.Acceleration, ref otherB.Acceleration, amount, out this.Acceleration);
			this.OffGroundCount = otherB.OffGroundCount;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Velocity.X);
			writer.Write(this.Velocity.Y);
			writer.Write(this.Acceleration.X);
			writer.Write(this.Acceleration.Y);
			writer.Write(this.OffGroundCount);
		}

		public void Deserialize(IReader reader)
		{
			this.Velocity.X = reader.ReadSingle();
			this.Velocity.Y = reader.ReadSingle();
			this.Acceleration.X = reader.ReadSingle();
			this.Acceleration.Y = reader.ReadSingle();
			this.OffGroundCount = reader.ReadInt32();
		}

		public void ResetToDefaults()
		{
			this.Velocity = Vector2.Zero;
			this.Acceleration = Vector2.Zero;
			this.OffGroundCount = 0;
		}

		#endregion Methods
	}
}
