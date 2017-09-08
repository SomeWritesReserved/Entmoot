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
		public void EntityCreate()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(2, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity));
			Assert.AreEqual(0, newEntity.ID);
			// Todo: makes sure before the update the entity isn't active yet (for this and all tests)
			entitySystemManager.Update();
			// Todo: make sure after an update the entity is active (for this and all tests)
		}

		[Test]
		public void EntityCreateMultiple()
		{
			// Todo: make sure after each update that the entity collection is in the order we expect
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(2, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(0, newEntity1.ID);
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(1, newEntity2.ID);
			entitySystemManager.Update();
			// Todo:
		}

		[Test]
		public void EntityCreateMultipleInSameTick()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(2, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(0, newEntity1.ID);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(1, newEntity2.ID);
			entitySystemManager.Update();
			//todo:
		}

		[Test]
		public void EntityCreateMultiple_TooMany()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(2, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			entitySystemManager.Update();
			Assert.IsFalse(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity3));
		}

		[Test]
		public void EntityCreateMultiple_TooManyInSameTick()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(2, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			entitySystemManager.Update();
			Assert.IsFalse(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity3));
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
