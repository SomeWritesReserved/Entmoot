using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Entmoot.Framework.MonoGame;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Sideswipe
{
	/// <summary>
	/// Represents an entity's ability to take up space in the world.
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

		/// <summary>
		/// Whether or not this entity is solid in the world, meaning other things will collide with this entity.
		/// </summary>
		public bool IsSolid;

		#endregion Fields

		#region Properties

		public Box2D Box2D => new Box2D(this.Position, this.Extents.X, this.Extents.Y);

		#endregion Properties

		#region Methods

		public bool Equals(SpatialComponent other)
		{
			return (this.Position.Equals(other.Position) &&
				this.Rotation.Equals(other.Rotation) &&
				this.Extents.Equals(other.Extents) &&
				this.IsSolid.Equals(other.IsSolid));
		}

		public void Interpolate(SpatialComponent otherA, SpatialComponent otherB, float amount)
		{
			Vector2.Lerp(ref otherA.Position, ref otherB.Position, amount, out this.Position);
			this.Rotation = MathHelper.Lerp(otherA.Rotation, otherB.Rotation, amount);
			Vector2.Lerp(ref otherA.Extents, ref otherB.Extents, amount, out this.Extents);
			this.IsSolid = otherB.IsSolid;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Rotation);
			writer.Write(this.Extents.X);
			writer.Write(this.Extents.Y);
			writer.Write(this.IsSolid);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Rotation = reader.ReadSingle();
			this.Extents.X = reader.ReadSingle();
			this.Extents.Y = reader.ReadSingle();
			this.IsSolid = reader.ReadBoolean();
		}

		public void ResetToDefaults()
		{
			this.Position = Vector2.Zero;
			this.Rotation = 0;
			this.Extents = Vector2.Zero;
			this.IsSolid = false;
		}

		#endregion Methods
	}
}
