using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Entmoot.Framework.MonoGame
{
	public static class Collision3D
	{
		#region Methods

		public static bool CollideBoxBox(Box3D boxA, Vector3 velocityA, Box3D boxB, float interpenetrationCorrection, out float collisionTime)
		{
			return Collision3D.CollideBoxBox(boxA, velocityA, boxB, interpenetrationCorrection,
				out collisionTime, out _, out _, out _);
		}

		public static bool CollideBoxBox(Box3D boxA, Vector3 velocityA, Box3D boxB, float interpenetrationCorrection,
			out float collisionTime, out Vector3 collisionNormal, out Vector3 collisionEdgeUp, out Vector3 collisionEdgeSide)
		{
			Box3D expandedBoxB = boxB.ExpandBy(boxA);

			Vector3 boxACenter = boxA.Center;
			Vector3 velocityAInverse = new Vector3(1.0f / velocityA.X, 1.0f / velocityA.Y, 1.0f / velocityA.Z);

			float timeX1 = (expandedBoxB.Min.X - boxACenter.X) * velocityAInverse.X;
			float timeX2 = (expandedBoxB.Max.X - boxACenter.X) * velocityAInverse.X;
			float timeY1 = (expandedBoxB.Min.Y - boxACenter.Y) * velocityAInverse.Y;
			float timeY2 = (expandedBoxB.Max.Y - boxACenter.Y) * velocityAInverse.Y;
			float timeZ1 = (expandedBoxB.Min.Z - boxACenter.Z) * velocityAInverse.Z;
			float timeZ2 = (expandedBoxB.Max.Z - boxACenter.Z) * velocityAInverse.Z;

			float firstCollideTimeX = Math.Min(timeX1, timeX2);
			float lastCollideTimeX = Math.Max(timeX1, timeX2);
			float firstCollideTimeY = Math.Min(timeY1, timeY2);
			float lastCollideTimeY = Math.Max(timeY1, timeY2);
			float firstCollideTimeZ = Math.Min(timeZ1, timeZ2);
			float lastCollideTimeZ = Math.Max(timeZ1, timeZ2);
			float firstCollideTime = Math.Max(Math.Max(firstCollideTimeX, firstCollideTimeY), firstCollideTimeZ);
			float lastCollideTime = Math.Min(Math.Min(lastCollideTimeX, lastCollideTimeY), lastCollideTimeZ);

			collisionTime = firstCollideTime;
			if (firstCollideTime == timeX1)
			{
				collisionNormal = -Vector3.UnitX;
				collisionEdgeUp = Vector3.UnitY;
				collisionEdgeSide = Vector3.UnitZ;
			}
			else if (firstCollideTime == timeX2)
			{
				collisionNormal = Vector3.UnitX;
				collisionEdgeUp = Vector3.UnitY;
				collisionEdgeSide = Vector3.UnitZ;
			}
			else if (firstCollideTime == timeY1)
			{
				collisionNormal = -Vector3.UnitY;
				collisionEdgeUp = Vector3.UnitX;
				collisionEdgeSide = Vector3.UnitZ;
			}
			else if (firstCollideTime == timeY2)
			{
				collisionNormal = Vector3.UnitY;
				collisionEdgeUp = Vector3.UnitX;
				collisionEdgeSide = Vector3.UnitZ;
			}
			else if (firstCollideTime == timeZ1)
			{
				collisionNormal = -Vector3.UnitZ;
				collisionEdgeUp = Vector3.UnitY;
				collisionEdgeSide = Vector3.UnitX;
			}
			else
			{
				collisionNormal = Vector3.UnitZ;
				collisionEdgeUp = Vector3.UnitY;
				collisionEdgeSide = Vector3.UnitX;
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
