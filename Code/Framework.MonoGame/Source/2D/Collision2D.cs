//#define COLLIDE_VIA_3D_ON_X

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
#if COLLIDE_VIA_3D_ON_X
			Box3D boxA_3d = new Box3D(new Vector3(0, boxA.Min.X, boxA.Min.Y), new Vector3(1, boxA.Max.X, boxA.Max.Y));
			Vector3 velocityA_3d = new Vector3(0, velocityA.X, velocityA.Y);
			Box3D boxB_3d = new Box3D(new Vector3(0, boxB.Min.X, boxB.Min.Y), new Vector3(1, boxB.Max.X, boxB.Max.Y));

			bool returnValue = Collision3D.CollideBoxBox(boxA_3d, velocityA_3d, boxB_3d, interpenetrationCorrection,
				out collisionTime, out Vector3 collisionNormal_3d, out Vector3 collisionEdgeUp_3d, out Vector3 collisionEdgeSide_3d);

			collisionNormal = new Vector2(collisionNormal_3d.Y, collisionNormal_3d.Z);
			collisionEdge = new Vector2(collisionEdgeUp_3d.Y, collisionEdgeUp_3d.X);
			return returnValue;
#elif COLLIDE_VIA_3D_ON_Z
			Box3D boxA_3d = new Box3D(new Vector3(boxA.Min.X, boxA.Min.Y, 0), new Vector3(boxA.Max.X, boxA.Max.Y, 1));
			Vector3 velocityA_3d = new Vector3(velocityA.X, velocityA.Y, 0);
			Box3D boxB_3d = new Box3D(new Vector3(boxB.Min.X, boxB.Min.Y, 0), new Vector3(boxB.Max.X, boxB.Max.Y, 1));

			bool returnValue = Collision3D.CollideBoxBox(boxA_3d, velocityA_3d, boxB_3d, interpenetrationCorrection,
				out collisionTime, out Vector3 collisionNormal_3d, out Vector3 collisionEdgeUp_3d, out Vector3 collisionEdgeSide_3d);

			collisionNormal = new Vector2(collisionNormal_3d.X, collisionNormal_3d.Y);
			collisionEdge = new Vector2(collisionEdgeUp_3d.X, collisionEdgeUp_3d.Y);
			return returnValue;
#else
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
#endif
		}

		#endregion Methods
	}
}
