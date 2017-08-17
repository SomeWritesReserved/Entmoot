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
			EntityManager entityManager = new EntityManager(2, new IEntitySystem[0]);
			Entity newEntity = entityManager.CreateEntity<Entity>();
			Assert.NotNull(newEntity);
			Assert.AreEqual(0, newEntity.ID);
			Assert.AreEqual(EntityState.Creating, newEntity.EntityState);
			Assert.AreEqual(0, entityManager.Entities.Count);
			entityManager.Update();
			Assert.AreEqual(EntityState.Active, newEntity.EntityState);
			Assert.AreEqual(newEntity, entityManager.Entities[0]);
		}

		[Test]
		public void EntityCreateMultiple()
		{
			EntityManager entityManager = new EntityManager(2, new IEntitySystem[0]);
			Entity newEntity1 = entityManager.CreateEntity<Entity>();
			Assert.NotNull(newEntity1);
			Assert.AreEqual(0, newEntity1.ID);
			Assert.AreEqual(EntityState.Creating, newEntity1.EntityState);
			Assert.AreEqual(0, entityManager.Entities.Count);
			entityManager.Update();
			Assert.AreEqual(EntityState.Active, newEntity1.EntityState);
			Assert.AreEqual(1, entityManager.Entities.Count);
			Assert.AreEqual(newEntity1, entityManager.Entities[0]);
			Entity newEntity2 = entityManager.CreateEntity<Entity>();
			Assert.NotNull(newEntity2);
			Assert.AreEqual(1, newEntity2.ID);
			Assert.AreEqual(EntityState.Creating, newEntity2.EntityState);
			Assert.AreEqual(1, entityManager.Entities.Count);
			entityManager.Update();
			Assert.AreEqual(EntityState.Active, newEntity1.EntityState);
			Assert.AreEqual(EntityState.Active, newEntity2.EntityState);
			Assert.AreEqual(2, entityManager.Entities.Count);
			Assert.AreEqual(newEntity1, entityManager.Entities[0]);
			Assert.AreEqual(newEntity2, entityManager.Entities[1]);
		}

		[Test]
		public void EntityCreateMultipleInSameTick()
		{
			EntityManager entityManager = new EntityManager(2, new IEntitySystem[0]);
			Entity newEntity1 = entityManager.CreateEntity<Entity>();
			Assert.NotNull(newEntity1);
			Assert.AreEqual(0, newEntity1.ID);
			Assert.AreEqual(EntityState.Creating, newEntity1.EntityState);
			Assert.AreEqual(0, entityManager.Entities.Count);
			Entity newEntity2 = entityManager.CreateEntity<Entity>();
			Assert.NotNull(newEntity2);
			Assert.AreEqual(1, newEntity2.ID);
			Assert.AreEqual(EntityState.Creating, newEntity2.EntityState);
			Assert.AreEqual(0, entityManager.Entities.Count);
			entityManager.Update();
			Assert.AreEqual(EntityState.Active, newEntity1.EntityState);
			Assert.AreEqual(newEntity1, entityManager.Entities[0]);
			Assert.AreEqual(EntityState.Active, newEntity2.EntityState);
			Assert.AreEqual(newEntity2, entityManager.Entities[1]);
		}

		[Test]
		public void EntityCreateMultiple_TooMany()
		{
			EntityManager entityManager = new EntityManager(2, new IEntitySystem[0]);
			Assert.AreEqual(0, entityManager.CreateEntity<Entity>().ID);
			entityManager.Update();
			Assert.AreEqual(1, entityManager.CreateEntity<Entity>().ID);
			entityManager.Update();
			Assert.IsNull(entityManager.CreateEntity<Entity>());
		}

		[Test]
		public void EntityCreateMultiple_TooManyInSameTick()
		{
			EntityManager entityManager = new EntityManager(2, new IEntitySystem[0]);
			Assert.AreEqual(0, entityManager.CreateEntity<Entity>().ID);
			Assert.AreEqual(1, entityManager.CreateEntity<Entity>().ID);
			Assert.IsNull(entityManager.CreateEntity<Entity>());
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
