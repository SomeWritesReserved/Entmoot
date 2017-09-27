using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an ordered list of three numbers or a three dimensional quantity.
	/// </summary>
	[DebuggerDisplay("({X}, {Y}, {Z})")]
	public struct Vec3 : IEquatable<Vec3>
	{
		#region Fields

		/// <summary>The X axis component of this vector (or the first value).</summary>
		public float X;

		/// <summary>The Y axis component of this vector (or the second value).</summary>
		public float Y;

		/// <summary>The Z axis component of this vector (or the third value).</summary>
		public float Z;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Vec3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Returns a new vector that is an interpolated value between two other vectors, interpolated by the given amount.
		/// </summary>
		public static Vec3 Interpolate(Vec3 vectorA, Vec3 vectorB, float amount)
		{
			return new Vec3(vectorA.X + (vectorB.X - vectorA.X) * amount,
				vectorA.Y + (vectorB.Y - vectorA.Y) * amount,
				vectorA.Z + (vectorB.Z - vectorA.Z) * amount);
		}

		/// <summary>
		/// Returns whether two vectors are equal (same).
		/// </summary>
		public static bool operator ==(Vec3 a, Vec3 b)
		{
			return (a.X == b.X &&
				a.Y == b.Y &&
				a.Z == b.Z);
		}

		/// <summary>
		/// Returns whether two vectors are not equal (different).
		/// </summary>
		public static bool operator !=(Vec3 a, Vec3 b)
		{
			return (a.X != b.X ||
				a.Y != b.Y ||
				a.Z != b.Z);
		}
		
		/// <summary>
		/// Returns whether this vector is equal to another object.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (!(obj is Vec3)) { return false; }
			return this.Equals((Vec3)obj);
		}
		
		/// <summary>
		/// Returns whether this vector is equal to another vector.
		/// </summary>
		public bool Equals(Vec3 other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns the hashcode for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		/// <summary>
		/// Returns the string representation for this vector.
		/// </summary>
		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.X, this.Y, this.Z);
		}

		#endregion Methods
	}
}
