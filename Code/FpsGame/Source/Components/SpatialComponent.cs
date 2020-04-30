using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	public struct SpatialComponent : IComponent<SpatialComponent>
	{
		#region Fields

		public Vector3 Position;
		public Quaternion Orientation;

		#endregion Fields

		#region Methods

		public bool Equals(SpatialComponent other)
		{
			return (this.Position.Equals(other.Position) &&
				this.Orientation.Equals(other.Orientation));
		}

		public void Interpolate(SpatialComponent otherA, SpatialComponent otherB, float amount)
		{
			Vector3.Lerp(ref otherA.Position, ref otherB.Position, amount, out this.Position);
			Quaternion.Slerp(ref otherA.Orientation, ref otherB.Orientation, amount, out this.Orientation);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Position.Z);
			writer.Write(this.Orientation.X);
			writer.Write(this.Orientation.Y);
			writer.Write(this.Orientation.Z);
			writer.Write(this.Orientation.W);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Position.Z = reader.ReadSingle();
			this.Orientation.X = reader.ReadSingle();
			this.Orientation.Y = reader.ReadSingle();
			this.Orientation.Z = reader.ReadSingle();
			this.Orientation.W = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this.Position = Vector3.Zero;
			this.Orientation = Quaternion.Identity;
		}

		#endregion Methods
	}
}
