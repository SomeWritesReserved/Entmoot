using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Entmoot.Framework.MonoGame
{
	public static class Collision2D
	{
		#region Methods

		public static bool CollideBoxBox(Box2D boxA, Vector2 velocityA, Box2D boxB, float interpenetrationCorrection, out float collisionTime)
		{
			return Collision2D.CollideBoxBox(boxA, velocityA, boxB, interpenetrationCorrection,
				out collisionTime, out _, out _);
		}

		public static bool CollideBoxBox(Box2D boxA, Vector2 velocityA, Box2D boxB, float interpenetrationCorrection,
			out float collisionTime, out Vector2 collisionNormal, out Vector2 collisionEdge)
		{
			Box2D expandedBoxB = boxB.ExpandBy(boxA);

			Vector2 boxACenter = boxA.Center;
			Vector2 velocityAInverse = new Vector2(1.0f / velocityA.X, 1.0f / velocityA.Y);

			float timeX1 = (expandedBoxB.Min.X - boxACenter.X) * velocityAInverse.X;
			float timeX2 = (expandedBoxB.Max.X - boxACenter.X) * velocityAInverse.X;
			float timeY1 = (expandedBoxB.Min.Y - boxACenter.Y) * velocityAInverse.Y;
			float timeY2 = (expandedBoxB.Max.Y - boxACenter.Y) * velocityAInverse.Y;

			float firstCollideTimeX = Math.Min(timeX1, timeX2);
			float lastCollideTimeX = Math.Max(timeX1, timeX2);
			float firstCollideTimeY = Math.Min(timeY1, timeY2);
			float lastCollideTimeY = Math.Max(timeY1, timeY2);
			float firstCollideTime = Math.Max(firstCollideTimeX, firstCollideTimeY);
			float lastCollideTime = Math.Min(lastCollideTimeX, lastCollideTimeY);

			collisionTime = firstCollideTime;
			if (firstCollideTime == timeX1)
			{
				collisionNormal = -Vector2.UnitX;
				collisionEdge = Vector2.UnitY;
			}
			else if (firstCollideTime == timeX2)
			{
				collisionNormal = Vector2.UnitX;
				collisionEdge = Vector2.UnitY;
			}
			else if (firstCollideTime == timeY1)
			{
				collisionNormal = -Vector2.UnitY;
				collisionEdge = Vector2.UnitX;
			}
			else
			{
				collisionNormal = Vector2.UnitY;
				collisionEdge = Vector2.UnitX;
			}

			if (firstCollideTime < 0 && firstCollideTime > -interpenetrationCorrection)
			{
				// This accounts for floating point imprecision where a box might be moved to meet another box
				// but now actually slightly interpenetrates the box because the true position cannot be represented.
				// If boxes are slightly interpenetrating this treats them as if they are just touching.
				firstCollideTime = 0;
			}

			return lastCollideTime > Math.Max(firstCollideTime, 0.0f) && firstCollideTime >= 0 && firstCollideTime < 1.0f;
		}

		#endregion Methods
	}
}
