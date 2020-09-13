using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Entmoot.Framework.MonoGame.UnitTests
{
	[TestFixture]
	public class Box2DTests
	{
		#region Tests

		[Test]
		public void Constructor_MinMax()
		{
			Box2D box = new Box2D(new Vector2(-100, 75), new Vector2(-50, 85));
			Assert.AreEqual(new Vector2(-100, 75), box.Min);
			Assert.AreEqual(new Vector2(-50, 85), box.Max);
			Assert.AreEqual(new Vector2(-75, 80), box.Center);
			Assert.AreEqual(new Vector2(50, 10), box.Size);
		}

		[Test]
		public void Constructor_CenterSize()
		{
			Box2D box = new Box2D(new Vector2(-75, 80), 50, 10);
			Assert.AreEqual(new Vector2(-100, 75), box.Min);
			Assert.AreEqual(new Vector2(-50, 85), box.Max);
			Assert.AreEqual(new Vector2(-75, 80), box.Center);
			Assert.AreEqual(new Vector2(50, 10), box.Size);
		}

		[Test]
		public void IEquatable_Equals()
		{
			#pragma warning disable 1718
			Box2D boxA = new Box2D(new Vector2(-75, 80), 50, 10);
			Box2D boxB = new Box2D(new Vector2(-100, 75), new Vector2(-50, 85));
			Assert.IsTrue(boxA.Equals(boxA));
			Assert.IsTrue(boxA.Equals(boxB));
			Assert.IsTrue(boxB.Equals(boxA));
			Assert.IsTrue(boxA == boxA);
			Assert.IsTrue(boxA == boxB);
			Assert.IsTrue(boxB == boxA);
			Assert.IsFalse(boxA != boxA);
			Assert.IsFalse(boxA != boxB);
			Assert.IsFalse(boxB != boxA);
			Assert.AreEqual(boxA.GetHashCode(), boxB.GetHashCode());
			#pragma warning restore 1718
		}

		[Test]
		public void IEquatable_NotEquals()
		{
			Box2D boxA = new Box2D(new Vector2(-75, 80), 50, 9);
			Box2D boxB = new Box2D(new Vector2(-100, 75), new Vector2(-50, 85));
			Assert.IsTrue(boxA.Equals(boxA));
			Assert.IsFalse(boxA.Equals(boxB));
			Assert.IsFalse(boxB.Equals(boxA));
			Assert.IsFalse(boxA == boxB);
			Assert.IsFalse(boxB == boxA);
			Assert.IsTrue(boxA != boxB);
			Assert.IsTrue(boxB != boxA);
			Assert.AreNotEqual(boxA.GetHashCode(), boxB.GetHashCode());
		}

		[Test]
		public void ExpandBy()
		{
			Box2D boxA = new Box2D(new Vector2(-75, 80), 50, 10);
			Box2D boxB = new Box2D(new Vector2(350, -30), new Vector2(370, 0));

			Box2D expandedByA = boxB.ExpandBy(boxA);
			Assert.AreEqual(new Vector2(325, -35), expandedByA.Min);
			Assert.AreEqual(new Vector2(395, 5), expandedByA.Max);
			Assert.AreEqual(new Vector2(360, -15), expandedByA.Center);
			Assert.AreEqual(boxA.Size + boxB.Size, expandedByA.Size);
			Assert.AreEqual(new Vector2(70, 40), expandedByA.Size);

			Box2D expandedByB = boxA.ExpandBy(boxB);
			Assert.AreEqual(new Vector2(-110, 60), expandedByB.Min);
			Assert.AreEqual(new Vector2(-40, 100), expandedByB.Max);
			Assert.AreEqual(new Vector2(-75, 80), expandedByB.Center);
			Assert.AreEqual(boxA.Size + boxB.Size, expandedByB.Size);
			Assert.AreEqual(new Vector2(70, 40), expandedByB.Size);
		}

		#endregion Tests
	}
}
