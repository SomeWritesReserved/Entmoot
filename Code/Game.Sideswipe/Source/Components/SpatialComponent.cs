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
	/// Represents a entity that takes up space in the world.
	/// </summary>
	public struct SpatialComponent : IComponent<SpatialComponent>
	{
		#region Fields

		/// <summary>
		/// The position of this entity in the world, with the center of the entity being the center of the sprite and the center of <see cref="Extents"/>.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// The rotation of this entity (in radians) about the center of the sprite.
		/// </summary>
		public float Rotation;

		/// <summary>
		/// The width and height extents that this entity takes up in space.
		/// </summary>
		public Vector2 Extents;

		#endregion Fields

		#region Methods

		public bool Equals(SpatialComponent other)
		{
			return (this.Position.Equals(other.Position) &&
				this.Rotation.Equals(other.Rotation) &&
				this.Extents.Equals(other.Extents));
		}

		public void Interpolate(SpatialComponent otherA, SpatialComponent otherB, float amount)
		{
			Vector2.Lerp(ref otherA.Position, ref otherB.Position, amount, out this.Position);
			this.Rotation = MathHelper.Lerp(otherA.Rotation, otherB.Rotation, amount);
			Vector2.Lerp(ref otherA.Extents, ref otherB.Extents, amount, out this.Extents);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Rotation);
			writer.Write(this.Extents.X);
			writer.Write(this.Extents.Y);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Rotation = reader.ReadSingle();
			this.Extents.X = reader.ReadSingle();
			this.Extents.Y = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Position = Vector2.Zero;
			this.Rotation = 0;
			this.Extents = Vector2.Zero;
		}

		#endregion Methods
	}
}
