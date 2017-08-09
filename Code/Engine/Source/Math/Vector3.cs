using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	[DebuggerDisplay("({X}, {Y}, {Z})")]
	public struct Vector3 : IEquatable<Vector3>
	{
		#region Fields

		public float X;
		public float Y;
		public float Z;

		#endregion Fields

		#region Constructors

		public Vector3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		#endregion Constructors

		#region Methods

		public static bool CloseTo(Vector3 vectorA, Vector3 vectorB, float distance)
		{
			return (Math.Abs(vectorA.X - vectorB.X) < distance) &&
				(Math.Abs(vectorA.Y - vectorB.Y) < distance) &&
				(Math.Abs(vectorA.Z - vectorB.Z) < distance);
		}

		public static Vector3 Interpolate(Vector3 vectorA, Vector3 vectorB, float amount)
		{
			return new Vector3(vectorA.X + (vectorB.X - vectorA.X) * amount,
				vectorA.Y + (vectorB.Y - vectorA.Y) * amount,
				vectorA.Z + (vectorB.Z - vectorA.Z) * amount);
		}

		public static bool operator ==(Vector3 a, Vector3 b)
		{
			return (a.X == b.X &&
				a.Y == b.Y &&
				a.Z == b.Z);
		}

		public static bool operator !=(Vector3 a, Vector3 b)
		{
			return (a.X != b.X ||
				a.Y != b.Y ||
				a.Z != b.Z);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Vector3)) { return false; }
			return this.Equals((Vector3)obj);
		}

		public bool Equals(Vector3 other)
		{
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.X, this.Y, this.Z);
		}

		#endregion Methods
	}
}
