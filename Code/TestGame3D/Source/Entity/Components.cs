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
}
