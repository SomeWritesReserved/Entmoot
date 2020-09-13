using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Entmoot.Framework.MonoGame
{
	public struct Box2D : IEquatable<Box2D>
	{
		#region Fields

		public readonly Vector2 Min;
		public readonly Vector2 Max;

		#endregion Fields

		#region Constructors

		public Box2D(Vector2 min, Vector2 max)
		{
			this.Min = min;
			this.Max = max;
		}

		public Box2D(Vector2 center, float sizeX, float sizeY)
		{
			Vector2 halfExtents = new Vector2(sizeX * 0.5f, sizeY * 0.5f);
			this.Min = center - halfExtents;
			this.Max = center + halfExtents;
		}

		#endregion Constructors

		#region Methods

		public Vector2 Center => this.Min + this.Size * 0.5f;

		public Vector2 Size => this.Max - this.Min;

		#endregion Methods

		#region Methods

		public override int GetHashCode()
		{
			return this.Min.GetHashCode() ^ this.Max.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Box2D)
			{
				return this.Equals((Box2D)obj);
			}
			return false;
		}

		public bool Equals(Box2D other)
		{
			return this.Min == other.Min &&
				this.Max == other.Max;
		}

		public static bool operator ==(Box2D a, Box2D b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Box2D a, Box2D b)
		{
			return !a.Equals(b);
		}

		public Box2D ExpandBy(Box2D other)
		{
			Vector2 otherHalfExtents = other.Size * 0.5f;
			return new Box2D(this.Min - otherHalfExtents, this.Max + otherHalfExtents);
		}

		#endregion Methods
	}
}
