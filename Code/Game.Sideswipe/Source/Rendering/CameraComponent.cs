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
	/// Represents an entity's ability to have a camera view (meaning if you were to render the screen with respect to this entity's point of view,
	/// you would use its <see cref="CameraComponent"/>).
	/// </summary>
	public struct CameraComponent : IComponent<CameraComponent>
	{
		#region Fields

		/// <summary>
		/// The position of the center of the camera's view in the world.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// The width and height extents that the screen can see in the world.
		/// </summary>
		public Vector2 Extents;

		#endregion Fields

		#region Methods

		public bool Equals(CameraComponent other)
		{
			return (this.Position.Equals(other.Position) &&
				this.Extents.Equals(other.Extents));
		}

		public void Interpolate(CameraComponent otherA, CameraComponent otherB, float amount)
		{
			Vector2.Lerp(ref otherA.Position, ref otherB.Position, amount, out this.Position);
			Vector2.Lerp(ref otherA.Extents, ref otherB.Extents, amount, out this.Extents);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Extents.X);
			writer.Write(this.Extents.Y);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Extents.X = reader.ReadSingle();
			this.Extents.Y = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Position = Vector2.Zero;
			this.Extents = Vector2.Zero;
		}

		#endregion Methods
	}
}
