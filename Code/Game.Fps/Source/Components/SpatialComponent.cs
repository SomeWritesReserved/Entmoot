using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Entmoot.Framework.MonoGame;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Fps
{
	/// <summary>
	/// Represents an entity's ability to take up space in the world.
	/// </summary>
	public struct SpatialComponent : IComponent<SpatialComponent>
	{
		#region Fields

		/// <summary>
		/// The position of this entity's origin in the world. The Y component of the origin is always the bottom of the height's extents.
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// The rotation of this entity (in radians), pitch then yaw.
		/// </summary>
		public Vector2 Rotation;

		/// <summary>
		/// The width, height, and depth extents that this entity takes up in space, defining its axis-aligned bounding box.
		/// The X and Z components define the half extents of the width and depth while the Y component defines the full extents of the height from the bottom.
		/// </summary>
		public Vector3 Extents;

		/// <summary>
		/// The color to render this entity.
		/// </summary>
		public Color Color;

		#endregion Fields

		#region Methods

		public bool Equals(SpatialComponent other)
		{
			return (this.Position.Equals(other.Position) &&
				this.Rotation.Equals(other.Rotation) &&
				this.Extents.Equals(other.Extents) &&
				this.Color.Equals(other.Color));
		}

		public void Interpolate(SpatialComponent otherA, SpatialComponent otherB, float amount)
		{
			Vector3.Lerp(ref otherA.Position, ref otherB.Position, amount, out this.Position);
			Vector2.Lerp(ref otherA.Rotation, ref otherB.Rotation, amount, out this.Rotation);
			this.Extents = otherB.Extents;
			this.Color = otherB.Color;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Position.Z);
			writer.Write(this.Rotation.X);
			writer.Write(this.Rotation.Y);
			writer.Write(this.Extents.X);
			writer.Write(this.Extents.Y);
			writer.Write(this.Extents.Z);
			writer.Write(this.Color.PackedValue);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Position.Z = reader.ReadSingle();
			this.Rotation.X = reader.ReadSingle();
			this.Rotation.Y = reader.ReadSingle();
			this.Extents.X = reader.ReadSingle();
			this.Extents.Y = reader.ReadSingle();
			this.Extents.Z = reader.ReadSingle();
			this.Color = new Color(reader.ReadUInt32());
		}

		public void ResetToDefaults()
		{
			this.Position = Vector3.Zero;
			this.Rotation = Vector2.Zero;
			this.Extents = Vector3.Zero;
			this.Color = Color.WhiteSmoke;
		}

		#endregion Methods
	}
}
