using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Fps
{
	/// <summary>
	/// Represents an entity's ability to move around in the world.
	/// </summary>
	public struct MovementComponent : IComponent<MovementComponent>
	{
		#region Fields

		/// <summary>
		/// The velocity of this entity in the world, with the center of travel being the entity's position.
		/// </summary>
		public Vector3 Velocity;

		/// <summary>
		/// The acceleration of this entity in the world, with the center of travel being the entity's position.
		/// </summary>
		//public Vector3 Acceleration;

		#endregion Fields

		#region Methods

		public bool Equals(MovementComponent other)
		{
			return this.Velocity.Equals(other.Velocity);
		}

		public void Interpolate(MovementComponent otherA, MovementComponent otherB, float amount)
		{
			Vector3.Lerp(ref otherA.Velocity, ref otherB.Velocity, amount, out this.Velocity);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Velocity.X);
			writer.Write(this.Velocity.Y);
			writer.Write(this.Velocity.Z);
		}

		public void Deserialize(IReader reader)
		{
			this.Velocity.X = reader.ReadSingle();
			this.Velocity.Y = reader.ReadSingle();
			this.Velocity.Z = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Velocity = Vector3.Zero;
		}

		#endregion Methods
	}
}
