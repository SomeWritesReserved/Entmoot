using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using NUnit.Framework;

namespace Entmoot.UnitTests
{
	/// <summary>
	/// Future tests:
	/// -Adding, removing, editing components. Adding components multiple times don't reset state. Entity component methods same as EntityArray?
	/// -GetComponentArray()
	/// -CopyTo()
	/// </summary>
	[TestFixture]
	public class EntitySystemManagerTests
	{
		#region Tests

		[Test]
		public void EntityGet_NoEntityExists()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out Entity noEntity));
			Assert.AreEqual(Entity.NoEntity, noEntity);
			Assert.AreEqual(-1, noEntity.ID);
			Assert.AreEqual(-1, Entity.NoEntity.ID);
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
		public void EntityCreate_ResetsComponents()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(1, EntitySystemManagerTests.createComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntityA));
			ref PositionComponent2D positionComponentA = ref entitySystemManager.EntityArray.GetComponentArray<PositionComponent2D>().AddComponent(newEntityA);
			ref HealthComponent healthComponentA = ref entitySystemManager.EntityArray.GetComponentArray<HealthComponent>().AddComponent(newEntityA);
			ref StringComponent stringComponentA = ref entitySystemManager.EntityArray.GetComponentArray<StringComponent>().AddComponent(newEntityA);
			Assert.IsTrue(entitySystemManager.EntityArray.GetComponentArray<PositionComponent2D>().HasComponent(newEntityA));
			Assert.IsTrue(entitySystemManager.EntityArray.GetComponentArray<HealthComponent>().HasComponent(newEntityA));
			Assert.IsTrue(entitySystemManager.EntityArray.GetComponentArray<StringComponent>().HasComponent(newEntityA));
			positionComponentA.PositionX = 10;
			positionComponentA.PositionY = 14.44f;
			healthComponentA.HealthAmount = 99;
			stringComponentA.StringValue = "!!!!";
			entitySystemManager.EntityArray.RemoveEntity(newEntityA);
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntityB));
			ref PositionComponent2D positionComponentB = ref entitySystemManager.EntityArray.GetComponentArray<PositionComponent2D>().GetComponent(newEntityB);
			ref HealthComponent healthComponentB = ref entitySystemManager.EntityArray.GetComponentArray<HealthComponent>().GetComponent(newEntityB);
			ref StringComponent stringComponentB = ref entitySystemManager.EntityArray.GetComponentArray<StringComponent>().GetComponent(newEntityB);
			Assert.IsFalse(entitySystemManager.EntityArray.GetComponentArray<PositionComponent2D>().HasComponent(newEntityB));
			Assert.IsFalse(entitySystemManager.EntityArray.GetComponentArray<HealthComponent>().HasComponent(newEntityB));
			Assert.IsFalse(entitySystemManager.EntityArray.GetComponentArray<StringComponent>().HasComponent(newEntityB));
			Assert.AreEqual(0, positionComponentB.PositionX);
			Assert.AreEqual(0, positionComponentB.PositionY);
			Assert.AreEqual(0, healthComponentB.HealthAmount);
			Assert.IsNull(stringComponentB.StringValue);
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
			Assert.AreEqual(Entity.NoEntity, noEntity);
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
			Assert.AreEqual(Entity.NoEntity, noEntityc);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out Entity fetchedEntity0c));
			Assert.AreEqual(newEntity0, fetchedEntity0c);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out Entity fetchedEntity1c));
			Assert.AreEqual(newEntity1, fetchedEntity1c);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out Entity fetchedEntity2c));
			Assert.AreEqual(newEntity2, fetchedEntity2c);
			entitySystemManager.Update();
			Assert.IsFalse(entitySystemManager.EntityArray.TryCreateEntity(out Entity noEntityd));
			Assert.AreEqual(Entity.NoEntity, noEntityd);
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
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			entitySystemManager.Update();
			entitySystemManager.EntityArray.RemoveEntity(newEntity1);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityRemove_InSameTick()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			entitySystemManager.EntityArray.RemoveEntity(newEntity1);
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityRemove_NoEntityExists()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			entitySystemManager.EntityArray.RemoveEntity(newEntity1);
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.EntityArray.RemoveEntity(newEntity1);
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityRemove_RemoveMultipleTimesInSameTick()
		{
			EntitySystemManager entitySystemManager = new EntitySystemManager(new EntityArray(3, new ComponentsDefinition()), new ISystem[0]);
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entitySystemManager.EntityArray.TryCreateEntity(out Entity newEntity2));
			entitySystemManager.EntityArray.RemoveEntity(newEntity0);
			entitySystemManager.EntityArray.RemoveEntity(newEntity0);
			entitySystemManager.Update();
			entitySystemManager.EntityArray.RemoveEntity(newEntity2);
			entitySystemManager.EntityArray.RemoveEntity(newEntity2);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			entitySystemManager.EntityArray.RemoveEntity(newEntity1);
			entitySystemManager.EntityArray.RemoveEntity(newEntity1);
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsTrue(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
			entitySystemManager.Update();
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entitySystemManager.EntityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityArray_CopyTo()
		{
		}

		#endregion Tests

		#region Helpers

		private static ComponentsDefinition createComponentsDefinition()
		{
			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<PositionComponent2D>();
			componentsDefinition.RegisterComponentType<HealthComponent>();
			componentsDefinition.RegisterComponentType<StringComponent>();
			return componentsDefinition;
		}

		private static EntityArray createStandardEntityArray()
		{
			EntityArray entityArray = new EntityArray(3, EntitySystemManagerTests.createComponentsDefinition());
			entityArray.TryCreateEntity(out Entity entity0);
			entityArray.TryCreateEntity(out Entity entity1);
			entityArray.TryCreateEntity(out Entity entity2);
			entity0.AddComponent<PositionComponent2D>().PositionX = 60.5f;
			entity0.AddComponent<PositionComponent2D>().PositionX = 60.9f;
			entity1.AddComponent<PositionComponent2D>().PositionX = 61.5f;
			entity1.AddComponent<PositionComponent2D>().PositionX = 61.9f;
			entity1.AddComponent<PositionComponent2D>().PositionX = 62.5f;
			entity1.AddComponent<PositionComponent2D>().PositionX = 62.9f;
			//entity0.AddComponent<HealthComponent>().HealthAmount = 70;
			entity1.AddComponent<HealthComponent>().HealthAmount = 71;
			entity2.AddComponent<HealthComponent>().HealthAmount = 72;
			entity0.AddComponent<StringComponent>().StringValue = "entity0";
			entity1.AddComponent<StringComponent>().StringValue = "entity1";
			//entity2.AddComponent<StringComponent>().StringValue = "entity2";
			entityArray.RemoveEntity(entity1);
			EntitySystemManagerTests.assertStandardEntityArray(entityArray);
			return entityArray;
		}

		private static void assertStandardEntityArray(EntityArray entityArray)
		{
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity entity0));
			Assert.IsFalse(entityArray.TryGetEntity(1, out Entity entity1));
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity entity2));
			Assert.IsTrue(entity0.HasComponent<PositionComponent2D>());
			Assert.IsTrue(entity1.HasComponent<PositionComponent2D>());
			Assert.IsTrue(entity2.HasComponent<PositionComponent2D>());
		}

		#endregion Helpers

		#region Nested Types

		public struct PositionComponent2D : IComponent<PositionComponent2D>
		{
			public float PositionX;
			public float PositionY;
		}

		public struct HealthComponent : IComponent<HealthComponent>
		{
			public int HealthAmount;
		}

		public struct StringComponent : IComponent<StringComponent>
		{
			public string StringValue;
		}

		#endregion Nested Types
	}
}
