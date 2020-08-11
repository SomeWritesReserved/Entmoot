using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Zombtown
{
	public struct SpatialComponent : IComponent<SpatialComponent>
	{
		#region Fields

		public Vector2 Position;
		public float Rotation;
		public float Radius;

		#endregion Fields

		#region Methods

		public bool Equals(SpatialComponent other)
		{
			return (this.Position.Equals(other.Position) &&
				this.Rotation.Equals(other.Rotation) &&
				this.Radius.Equals(other.Radius));
		}

		public void Interpolate(SpatialComponent otherA, SpatialComponent otherB, float amount)
		{
			Vector2.Lerp(ref otherA.Position, ref otherB.Position, amount, out this.Position);
			this.Rotation = MathHelper.Lerp(otherA.Rotation, otherB.Rotation, amount);
			this.Radius = otherB.Radius;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Rotation);
			writer.Write(this.Radius);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Rotation = reader.ReadSingle();
			this.Radius = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Position = Vector2.Zero;
			this.Rotation = 0;
			this.Radius = 0;
		}

		#endregion Methods
	}
}
