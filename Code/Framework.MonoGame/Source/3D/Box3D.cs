using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Entmoot.Framework.MonoGame
{
	public struct Box3D : IEquatable<Box3D>
	{
		#region Fields

		public readonly Vector3 Min;
		public readonly Vector3 Max;

		#endregion Fields

		#region Constructors

		public Box3D(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		public Box3D(Vector3 center, float sizeX, float sizeY, float sizeZ)
		{
			Vector3 halfExtents = new Vector3(sizeX * 0.5f, sizeY * 0.5f, sizeZ * 0.5f);
			this.Min = center - halfExtents;
			this.Max = center + halfExtents;
		}

		#endregion Constructors

		#region Properties

		public Vector3 Center => this.Min + this.Size * 0.5f;

		public Vector3 Size => this.Max - this.Min;

		#endregion Properties

		#region Methods

		public override int GetHashCode()
		{
			return this.Min.GetHashCode() ^ this.Max.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Box3D)
			{
				return this.Equals((Box3D)obj);
			}
			return false;
		}

		public bool Equals(Box3D other)
		{
			return this.Min == other.Min &&
				this.Max == other.Max;
		}

		public static bool operator ==(Box3D a, Box3D b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Box3D a, Box3D b)
		{
			return !a.Equals(b);
		}

		public Box3D ExpandBy(Box3D other)
		{
			Vector3 otherHalfExtents = other.Size * 0.5f;
			return new Box3D(this.Min - otherHalfExtents, this.Max + otherHalfExtents);
		}

		#endregion Methods
	}
}
