using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Entmoot.Framework.MonoGame.UnitTests
{
	[TestFixture]
	public class Collision2DTests
	{
		#region Tests

		[Test]
		public void CollideBoxBox_UnitDir_NoCollide()
		{
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(-100, 0), 1, 1), Vector2.Zero, new Box2D(Vector2.Zero, 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(-100, 0), 1, 1), Vector2.UnitX, new Box2D(Vector2.Zero, 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(-100, 0), 1, 1), -Vector2.UnitX, new Box2D(Vector2.Zero, 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(-100, 0), 1, 1), Vector2.UnitY, new Box2D(Vector2.Zero, 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(-100, 0), 1, 1), -Vector2.UnitY, new Box2D(Vector2.Zero, 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_UnitDir_Collide()
		{
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(-100, 0), 1, 1), Vector2.UnitX * 100, new Box2D(Vector2.Zero, 20, 20), 0, out _));
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, 0), 1, 1), -Vector2.UnitX * 100, new Box2D(Vector2.Zero, 20, 20), 0, out _));
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, -100), 1, 1), Vector2.UnitY * 100, new Box2D(Vector2.Zero, 20, 20), 0, out _));
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, 100), 1, 1), -Vector2.UnitY * 100, new Box2D(Vector2.Zero, 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_CollideTime()
		{
			float collideTime;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(-20, 0), 1, 1), Vector2.UnitX * 20, new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(20, 0), 1, 1), -Vector2.UnitX * 20, new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, -20), 1, 1), Vector2.UnitY * 20, new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, 20), 1, 1), -Vector2.UnitY * 20, new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
		}

		[Test]
		public void CollideBoxBox_CollideTime_Diagonal()
		{
			float collideTime;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(-20, -20), 1, 1), new Vector2(20, 20), new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(20, -20), 1, 1), new Vector2(-20, 20), new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(-20, 20), 1, 1), new Vector2(20, -20), new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(20, 20), 1, 1), new Vector2(-20, -20), new Box2D(Vector2.Zero, 20, 20), 0, out collideTime));
			Assert.AreEqual(0.475, collideTime, 0.0001);
		}

		[Test]
		public void CollideBoxBox_EdgesStartTouching_NoCollide()
		{
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), Vector2.Zero, new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(1, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(1, 1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(0, 1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(1, -1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(0, -1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));

			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), Vector2.Zero, new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(1, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(1, 1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(0, 1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(1, -1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(0, -1), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_EdgesStartTouching_Collide()
		{
			float collideTime;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(-1, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(-1, 1), new Box2D(new Vector2(100, 0), 20, 20), 0, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 0), 2, 2), new Vector2(-1, -1), new Box2D(new Vector2(100, 0), 20, 20), 0, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);

			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(-1, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(-1, 1), new Box2D(new Vector2(100, 0), 20, 20), 0, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 10), 2, 2), new Vector2(-1, -1), new Box2D(new Vector2(100, 0), 20, 20), 0, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
		}

		[Test]
		public void CollideBoxBox_EdgesSlidePast_NoCollide()
		{
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, 100), 2, 2), new Vector2(0, -200), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111, -100), 2, 2), new Vector2(0, 200), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, 11), 2, 2), new Vector2(200, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(200, 11), 2, 2), new Vector2(-200, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_EdgesClipVerySmall_Collide()
		{
			const float clipAmount = 0.001f;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - clipAmount, 100), 2, 2), new Vector2(0, -200), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - clipAmount, -100), 2, 2), new Vector2(0, 200), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, 11 - clipAmount), 2, 2), new Vector2(200, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(200, 11 - clipAmount), 2, 2), new Vector2(-200, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_StopAtEdges_NoCollide()
		{
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, 10), 2, 2), new Vector2(89, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(200, 10), 2, 2), new Vector2(-89, 0), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_CornersSlidePast_NoCollide()
		{
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(122, 0), 2, 2), new Vector2(-20, 20), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(122, 0), 2, 2), new Vector2(-20, -20), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(78, 0), 2, 2), new Vector2(20, 20), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(78, 0), 2, 2), new Vector2(20, -20), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_CornersClipVerySmall_Collide()
		{
			const float clipAmount = 0.001f;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(122 - clipAmount, 0), 2, 2), new Vector2(-20, -20), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(122, 0 + clipAmount), 2, 2), new Vector2(-20, -20), new Box2D(new Vector2(100, 0), 20, 20), 0, out _));
		}

		[Test]
		public void CollideBoxBox_SlightlyPenetrating_NoCollide()
		{
			const float interpenetrationCorrection = 0.0001f;
			const float penetrationDepth = 0.00001f;
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), Vector2.Zero, new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(1, 0), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(1, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(0, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(1, -1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(0, -1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));

			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), Vector2.Zero, new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(1, 0), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(1, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(0, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(1, -1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
			Assert.IsFalse(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(0, -1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out _));
		}

		[Test]
		public void CollideBoxBox_SlightlyPenetrating_Collide()
		{
			const float interpenetrationCorrection = 0.0001f;
			const float penetrationDepth = 0.00001f;
			float collideTime;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(-1, 0), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(-1, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(-1, -1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);

			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(-1, 0), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(-1, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 10), 2, 2), new Vector2(-1, -1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection, out collideTime));
			Assert.AreEqual(0.0, collideTime, 0.0001);
		}

		[Test]
		public void CollideBoxBox_ReturnedSide_UnitDir()
		{
			Vector2 collisionNormal;
			Vector2 collisionEdge;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, 0), 2, 2), new Vector2(100, 0), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(-1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(200, 0), 2, 2), new Vector2(-100, 0), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, 100), 2, 2), new Vector2(0, -100), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, 1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, -100), 2, 2), new Vector2(0, 100), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, -1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
		}

		[Test]
		public void CollideBoxBox_ReturnedSide_XYComponents()
		{
			Vector2 collisionNormal;
			Vector2 collisionEdge;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(0, 0), 2, 2), new Vector2(100, 1), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(-1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(200, 0), 2, 2), new Vector2(-100, 1), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, 100), 2, 2), new Vector2(1, -100), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, 1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, -100), 2, 2), new Vector2(1, 100), new Box2D(new Vector2(100, 0), 20, 20), 0,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, -1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
		}

		[Test]
		public void CollideBoxBox_ReturnedSide_SemiPenetrating_UnitDir()
		{
			const float interpenetrationCorrection = 0.0001f;
			const float penetrationDepth = 0.00001f;
			Vector2 collisionNormal;
			Vector2 collisionEdge;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(89 + penetrationDepth, 0), 2, 2), new Vector2(1, 0), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(-1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(-1, 0), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, 11 - penetrationDepth), 2, 2), new Vector2(0, -1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, 1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, -11 + penetrationDepth), 2, 2), new Vector2(0, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, -1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
		}

		[Test]
		public void CollideBoxBox_ReturnedSide_SemiPenetrating_XYComponents()
		{
			const float interpenetrationCorrection = 0.0001f;
			const float penetrationDepth = 0.00001f;
			Vector2 collisionNormal;
			Vector2 collisionEdge;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(89 + penetrationDepth, 0), 2, 2), new Vector2(100, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(-1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(111 - penetrationDepth, 0), 2, 2), new Vector2(-100, 1), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(1, 0), collisionNormal);
			Assert.AreEqual(new Vector2(0, 1), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, 11 - penetrationDepth), 2, 2), new Vector2(1, -100), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, 1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(100, -11 + penetrationDepth), 2, 2), new Vector2(1, 100), new Box2D(new Vector2(100, 0), 20, 20), interpenetrationCorrection,
				out _, out collisionNormal, out collisionEdge));
			Assert.AreEqual(new Vector2(0, -1), collisionNormal);
			Assert.AreEqual(new Vector2(1, 0), collisionEdge);
		}

		[Test]
		public void CollideBoxBox_Rectangles_Collide()
		{
			float collideTime;
			Vector2 collisionNormal;
			Vector2 collisionEdge;
			Assert.IsTrue(Collision2D.CollideBoxBox(new Box2D(new Vector2(-310, 175), 5, 20), new Vector2(25, -50), new Box2D(new Vector2(-300, 140), 50, 1), 0,
				out collideTime, out collisionNormal, out collisionEdge));
			Assert.AreEqual(0.49, collideTime, 0.00001);
			Assert.AreEqual(Vector2.UnitY, collisionNormal);
			Assert.AreEqual(Vector2.UnitX, collisionEdge);
		}

		#endregion Tests
	}
}
