using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Entmoot.Framework.MonoGame.UnitTests
{
	[TestFixture]
	public class Box3DTests
	{
		#region Tests

		[Test]
		public void Constructor_MinMax()
		{
			Box3D box = new Box3D(new Vector3(-100, 75, -35), new Vector3(-50, 85, 15));
			Assert.AreEqual(new Vector3(-100, 75, -35), box.Min);
			Assert.AreEqual(new Vector3(-50, 85, 15), box.Max);
			Assert.AreEqual(new Vector3(-75, 80, -10), box.Center);
			Assert.AreEqual(new Vector3(50, 10, 50), box.Size);
		}

		[Test]
		public void Constructor_CenterSize()
		{
			Box3D box = new Box3D(new Vector3(-75, 80, 20), 50, 10, 40);
			Assert.AreEqual(new Vector3(-100, 75, 0), box.Min);
			Assert.AreEqual(new Vector3(-50, 85, 40), box.Max);
			Assert.AreEqual(new Vector3(-75, 80, 20), box.Center);
			Assert.AreEqual(new Vector3(50, 10, 40), box.Size);
		}

		[Test]
		public void IEquatable_Equals()
		{
			#pragma warning disable 1718
			Box3D boxA = new Box3D(new Vector3(-75, 80, -20), 50, 10, 40);
			Box3D boxB = new Box3D(new Vector3(-100, 75, -40), new Vector3(-50, 85, 0));
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
			Box3D boxA = new Box3D(new Vector3(-75, 80, -20), 50, 9, 40);
			Box3D boxB = new Box3D(new Vector3(-100, 75, -40), new Vector3(-50, 85, 0));
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
			Box3D boxA = new Box3D(new Vector3(-75, 80, -20), 50, 10, 40);
			Box3D boxB = new Box3D(new Vector3(350, -30, -2), new Vector3(370, 0, 2));

			Box3D expandedByA = boxB.ExpandBy(boxA);
			Assert.AreEqual(new Vector3(325, -35, -22), expandedByA.Min);
			Assert.AreEqual(new Vector3(395, 5, 22), expandedByA.Max);
			Assert.AreEqual(new Vector3(360, -15, 0), expandedByA.Center);
			Assert.AreEqual(boxA.Size + boxB.Size, expandedByA.Size);
			Assert.AreEqual(new Vector3(70, 40, 44), expandedByA.Size);

			Box3D expandedByB = boxA.ExpandBy(boxB);
			Assert.AreEqual(new Vector3(-110, 60, -42), expandedByB.Min);
			Assert.AreEqual(new Vector3(-40, 100, 2), expandedByB.Max);
			Assert.AreEqual(new Vector3(-75, 80, -20), expandedByB.Center);
			Assert.AreEqual(boxA.Size + boxB.Size, expandedByB.Size);
			Assert.AreEqual(new Vector3(70, 40, 44), expandedByB.Size);
		}

		#endregion Tests
	}
}
