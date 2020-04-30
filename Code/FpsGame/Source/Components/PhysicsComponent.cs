using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	public struct PhysicsComponent : IComponent<PhysicsComponent>
	{
		#region Fields


		public Vector3 Velocity;
		public Vector3 Acceleration;

		#endregion Fields

		#region Methods

		public bool Equals(PhysicsComponent other)
		{
			return (this.Velocity.Equals(other.Velocity) &&
				this.Acceleration.Equals(other.Acceleration));
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
}
