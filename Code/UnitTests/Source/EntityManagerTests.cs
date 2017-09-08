using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using NUnit.Framework;

namespace Entmoot.UnitTests
{
	[TestFixture]
	public class EntityManagerTests
	{
		#region Tests

		[Test]
		public void EntityGet_NoEntityExists()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out Entity noEntity));
			Assert.AreEqual(default(Entity), noEntity);
		}

		[Test]
		public void EntityCreate()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity));
			Assert.AreEqual(0, newEntity.ID);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity));
			Assert.AreEqual(newEntity, fetchedEntity);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityCreate_Multiple()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0));
			Assert.AreEqual(newEntity0, fetchedEntity0);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1));
			Assert.AreEqual(newEntity1, fetchedEntity1);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityCreate_MultipleFillCapacity()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0));
			Assert.AreEqual(newEntity0, fetchedEntity0);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1));
			Assert.AreEqual(newEntity1, fetchedEntity1);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out Entity fetchedEntity2));
			Assert.AreEqual(newEntity2, fetchedEntity2);
		}

		[Test]
		public void EntityCreate_MultipleFillCapacity_OverMultipleTicks()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0a));
			Assert.AreEqual(newEntity0, fetchedEntity0a);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0b));
			Assert.AreEqual(newEntity0, fetchedEntity0b);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1b));
			Assert.AreEqual(newEntity1, fetchedEntity1b);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0c));
			Assert.AreEqual(newEntity0, fetchedEntity0c);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1c));
			Assert.AreEqual(newEntity1, fetchedEntity1c);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out Entity fetchedEntity2c));
			Assert.AreEqual(newEntity2, fetchedEntity2c);
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0d));
			Assert.AreEqual(newEntity0, fetchedEntity0d);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1d));
			Assert.AreEqual(newEntity1, fetchedEntity1d);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out Entity fetchedEntity2d));
			Assert.AreEqual(newEntity2, fetchedEntity2d);
		}

		[Test]
		public void EntityCreate_TooMany()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsFalse(entitySystemManager.EntityArray.TryCreateEntity(out Entity noEntity));
			Assert.AreEqual(default(Entity), noEntity);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0));
			Assert.AreEqual(newEntity0, fetchedEntity0);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1));
			Assert.AreEqual(newEntity1, fetchedEntity1);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out Entity fetchedEntity2));
			Assert.AreEqual(newEntity2, fetchedEntity2);
		}

		[Test]
		public void EntityCreate_TooMany_OverMultipleTicks()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0a));
			Assert.AreEqual(newEntity0, fetchedEntity0a);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0b));
			Assert.AreEqual(newEntity0, fetchedEntity0b);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1b));
			Assert.AreEqual(newEntity1, fetchedEntity1b);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsFalse(entitySystemManager.EntityArray.TryCreateEntity(out Entity noEntityc));
			Assert.AreEqual(default(Entity), noEntityc);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0c));
			Assert.AreEqual(newEntity0, fetchedEntity0c);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1c));
			Assert.AreEqual(newEntity1, fetchedEntity1c);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out Entity fetchedEntity2c));
			Assert.AreEqual(newEntity2, fetchedEntity2c);
			entitySystemManager.Update();
			Assert.IsFalse(entitySystemManager.EntityArray.TryCreateEntity(out Entity noEntityd));
			Assert.AreEqual(default(Entity), noEntityd);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0d));
			Assert.AreEqual(newEntity0, fetchedEntity0d);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1d));
			Assert.AreEqual(newEntity1, fetchedEntity1d);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out Entity fetchedEntity2d));
			Assert.AreEqual(newEntity2, fetchedEntity2d);
		}

		[Test]
		public void EntityRemove()
		{
		}

		[Test]
		public void EntityRemoveRedundant()
		{
		}

		[Test]
		public void EntityCreateAndRemoveInSameTick()
		{
		}

		#endregion Tests
	}
}
