using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using NUnit.Framework;

namespace Entmoot.UnitTests
{
	/// <summary>
	/// Future tests:
	/// </summary>
	[TestFixture]
	public class EntitySystemManagerTests
	{
		#region Tests

		[Test]
		public void EntityGet_NoEntityExists()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsFalse(entityArray.TryGetEntity(0, out Entity noEntity));
			Assert.AreEqual(Entity.NoEntity, noEntity);
			Assert.AreEqual(-1, noEntity.ID);
			Assert.AreEqual(-1, Entity.NoEntity.ID);
		}

		[Test]
		public void EntityCreate()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity));
			Assert.AreEqual(0, newEntity.ID);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity));
			Assert.AreEqual(newEntity, fetchedEntity);
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityCreate_Multiple()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0));
			Assert.AreEqual(newEntity0, fetchedEntity0);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1));
			Assert.AreEqual(newEntity1, fetchedEntity1);
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityCreate_MultipleFillCapacity()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0));
			Assert.AreEqual(newEntity0, fetchedEntity0);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1));
			Assert.AreEqual(newEntity1, fetchedEntity1);
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity fetchedEntity2));
			Assert.AreEqual(newEntity2, fetchedEntity2);
		}

		[Test]
		public void EntityCreate_MultipleFillCapacity_OverMultipleTicks()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0a));
			Assert.AreEqual(newEntity0, fetchedEntity0a);
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0b));
			Assert.AreEqual(newEntity0, fetchedEntity0b);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1b));
			Assert.AreEqual(newEntity1, fetchedEntity1b);
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0c));
			Assert.AreEqual(newEntity0, fetchedEntity0c);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1c));
			Assert.AreEqual(newEntity1, fetchedEntity1c);
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity fetchedEntity2c));
			Assert.AreEqual(newEntity2, fetchedEntity2c);
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0d));
			Assert.AreEqual(newEntity0, fetchedEntity0d);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1d));
			Assert.AreEqual(newEntity1, fetchedEntity1d);
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity fetchedEntity2d));
			Assert.AreEqual(newEntity2, fetchedEntity2d);
		}

		[Test]
		public void EntityCreate_TooMany()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsFalse(entityArray.TryCreateEntity(out Entity noEntity));
			Assert.AreEqual(Entity.NoEntity, noEntity);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0));
			Assert.AreEqual(newEntity0, fetchedEntity0);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1));
			Assert.AreEqual(newEntity1, fetchedEntity1);
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity fetchedEntity2));
			Assert.AreEqual(newEntity2, fetchedEntity2);
		}

		[Test]
		public void EntityCreate_TooMany_OverMultipleTicks()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.AreEqual(0, newEntity0.ID);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.AreEqual(1, newEntity1.ID);
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0a));
			Assert.AreEqual(newEntity0, fetchedEntity0a);
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			Assert.AreEqual(2, newEntity2.ID);
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0b));
			Assert.AreEqual(newEntity0, fetchedEntity0b);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1b));
			Assert.AreEqual(newEntity1, fetchedEntity1b);
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsFalse(entityArray.TryCreateEntity(out Entity noEntityc));
			Assert.AreEqual(Entity.NoEntity, noEntityc);
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0c));
			Assert.AreEqual(newEntity0, fetchedEntity0c);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1c));
			Assert.AreEqual(newEntity1, fetchedEntity1c);
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity fetchedEntity2c));
			Assert.AreEqual(newEntity2, fetchedEntity2c);
			entityArray.EndUpdate();
			Assert.IsFalse(entityArray.TryCreateEntity(out Entity noEntityd));
			Assert.AreEqual(Entity.NoEntity, noEntityd);
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity fetchedEntity0d));
			Assert.AreEqual(newEntity0, fetchedEntity0d);
			Assert.IsTrue(entityArray.TryGetEntity(1, out Entity fetchedEntity1d));
			Assert.AreEqual(newEntity1, fetchedEntity1d);
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity fetchedEntity2d));
			Assert.AreEqual(newEntity2, fetchedEntity2d);
		}

		[Test]
		public void EntityRemove()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			entityArray.EndUpdate();
			entityArray.RemoveEntity(newEntity1);
			Assert.IsTrue(entityArray.TryGetEntity(0, out _));
			Assert.IsTrue(entityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityRemove_InSameTick()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			entityArray.RemoveEntity(newEntity1);
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityRemove_NoEntityExists()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			entityArray.RemoveEntity(newEntity1);
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entityArray.TryGetEntity(2, out _));
			entityArray.RemoveEntity(newEntity1);
			Assert.IsTrue(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void EntityRemove_RemoveMultipleTimesInSameTick()
		{
			EntityArray entityArray = new EntityArray(3, new ComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity0));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity1));
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntity2));
			entityArray.RemoveEntity(newEntity0);
			entityArray.RemoveEntity(newEntity0);
			entityArray.EndUpdate();
			entityArray.RemoveEntity(newEntity2);
			entityArray.RemoveEntity(newEntity2);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsTrue(entityArray.TryGetEntity(1, out _));
			Assert.IsTrue(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			entityArray.RemoveEntity(newEntity1);
			entityArray.RemoveEntity(newEntity1);
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsTrue(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
			entityArray.EndUpdate();
			Assert.IsFalse(entityArray.TryGetEntity(0, out _));
			Assert.IsFalse(entityArray.TryGetEntity(1, out _));
			Assert.IsFalse(entityArray.TryGetEntity(2, out _));
		}

		[Test]
		public void Components_ResetOnEntityCreate()
		{
			EntityArray entityArray = new EntityArray(1, EntitySystemManagerTests.createComponentsDefinition());
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntityA));
			ref PositionComponent2D positionComponentA = ref entityArray.GetComponentArray<PositionComponent2D>().AddComponent(newEntityA);
			ref HealthComponent healthComponentA = ref entityArray.GetComponentArray<HealthComponent>().AddComponent(newEntityA);
			ref StringComponent stringComponentA = ref entityArray.GetComponentArray<StringComponent>().AddComponent(newEntityA);
			Assert.IsTrue(entityArray.GetComponentArray<PositionComponent2D>().HasComponent(newEntityA));
			Assert.IsTrue(entityArray.GetComponentArray<HealthComponent>().HasComponent(newEntityA));
			Assert.IsTrue(entityArray.GetComponentArray<StringComponent>().HasComponent(newEntityA));
			positionComponentA.PositionX = 10;
			positionComponentA.PositionY = 14.44f;
			healthComponentA.HealthAmount = 99;
			stringComponentA.StringValue = "!!!!";
			entityArray.RemoveEntity(newEntityA);
			entityArray.EndUpdate();
			Assert.IsTrue(entityArray.TryCreateEntity(out Entity newEntityB));
			ref PositionComponent2D positionComponentB = ref entityArray.GetComponentArray<PositionComponent2D>().GetComponent(newEntityB);
			ref HealthComponent healthComponentB = ref entityArray.GetComponentArray<HealthComponent>().GetComponent(newEntityB);
			ref StringComponent stringComponentB = ref entityArray.GetComponentArray<StringComponent>().GetComponent(newEntityB);
			Assert.IsFalse(entityArray.GetComponentArray<PositionComponent2D>().HasComponent(newEntityB));
			Assert.IsFalse(entityArray.GetComponentArray<HealthComponent>().HasComponent(newEntityB));
			Assert.IsFalse(entityArray.GetComponentArray<StringComponent>().HasComponent(newEntityB));
			Assert.AreEqual(0, positionComponentB.PositionX);
			Assert.AreEqual(0, positionComponentB.PositionY);
			Assert.AreEqual(0, healthComponentB.HealthAmount);
			Assert.IsNull(stringComponentB.StringValue);
		}

		[Test]
		public void EntityArray_CopyTo_Empty()
		{
			EntityArray sourceEntityArray = EntitySystemManagerTests.createStandardEntityArray();
			EntityArray destinationEntityArray = new EntityArray(3, EntitySystemManagerTests.createComponentsDefinition());
			sourceEntityArray.CopyTo(destinationEntityArray);
			EntitySystemManagerTests.assertStandardEntityArray(destinationEntityArray);
		}

		[Test]
		public void EntityArray_CopyTo_Overwrite()
		{
			EntityArray sourceEntityArray = EntitySystemManagerTests.createStandardEntityArray();
			EntityArray destinationEntityArray = new EntityArray(3, EntitySystemManagerTests.createComponentsDefinition());
			destinationEntityArray.BeginUpdate();
			destinationEntityArray.TryCreateEntity(out Entity entity0);
			destinationEntityArray.TryCreateEntity(out Entity entity1);
			destinationEntityArray.TryCreateEntity(out Entity entity2);
			entity0.AddComponent<PositionComponent2D>().PositionX = 901;
			entity0.AddComponent<PositionComponent2D>().PositionY = 902;
			entity1.AddComponent<PositionComponent2D>().PositionX = 911;
			entity1.AddComponent<PositionComponent2D>().PositionY = 912;
			entity2.AddComponent<PositionComponent2D>().PositionX = 921;
			entity2.AddComponent<PositionComponent2D>().PositionY = 922;
			entity0.AddComponent<HealthComponent>().HealthAmount = 200;
			entity1.AddComponent<HealthComponent>().HealthAmount = 201;
			entity2.AddComponent<HealthComponent>().HealthAmount = 202;
			entity0.AddComponent<StringComponent>().StringValue = "z0";
			entity1.AddComponent<StringComponent>().StringValue = "z1";
			entity2.AddComponent<StringComponent>().StringValue = "z2";
			destinationEntityArray.RemoveEntity(entity0);
			destinationEntityArray.EndUpdate();
			sourceEntityArray.CopyTo(destinationEntityArray);
			EntitySystemManagerTests.assertStandardEntityArray(destinationEntityArray);
		}

		[Test]
		public void EntityArray_Serialize_Empty()
		{
			EntityArray sourceEntityArray = EntitySystemManagerTests.createStandardEntityArray();
			EntityArray destinationEntityArray = new EntityArray(3, EntitySystemManagerTests.createComponentsDefinition());
			byte[] serializedBytes;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					sourceEntityArray.Serialize(binaryWriter);
					serializedBytes = memoryStream.ToArray();
				}
			}
			using (MemoryStream memoryStream = new MemoryStream(serializedBytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					destinationEntityArray.Deserialize(binaryReader);
				}
			}
			EntitySystemManagerTests.assertStandardEntityArray(destinationEntityArray);
		}

		[Test]
		public void EntityArray_Serialize_Overwrite()
		{
			EntityArray sourceEntityArray = EntitySystemManagerTests.createStandardEntityArray();
			EntityArray destinationEntityArray = new EntityArray(3, EntitySystemManagerTests.createComponentsDefinition());
			destinationEntityArray.BeginUpdate();
			destinationEntityArray.TryCreateEntity(out Entity entity0);
			destinationEntityArray.TryCreateEntity(out Entity entity1);
			destinationEntityArray.TryCreateEntity(out Entity entity2);
			entity0.AddComponent<PositionComponent2D>().PositionX = 901;
			entity0.AddComponent<PositionComponent2D>().PositionY = 902;
			entity1.AddComponent<PositionComponent2D>().PositionX = 911;
			entity1.AddComponent<PositionComponent2D>().PositionY = 912;
			entity2.AddComponent<PositionComponent2D>().PositionX = 921;
			entity2.AddComponent<PositionComponent2D>().PositionY = 922;
			entity0.AddComponent<HealthComponent>().HealthAmount = 200;
			entity1.AddComponent<HealthComponent>().HealthAmount = 201;
			entity2.AddComponent<HealthComponent>().HealthAmount = 202;
			entity0.AddComponent<StringComponent>().StringValue = "z0";
			entity1.AddComponent<StringComponent>().StringValue = "z1";
			entity2.AddComponent<StringComponent>().StringValue = "z2";
			destinationEntityArray.RemoveEntity(entity0);
			destinationEntityArray.EndUpdate();
			byte[] serializedBytes;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					sourceEntityArray.Serialize(binaryWriter);
					serializedBytes = memoryStream.ToArray();
				}
			}
			using (MemoryStream memoryStream = new MemoryStream(serializedBytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					destinationEntityArray.Deserialize(binaryReader);
					Assert.AreEqual(memoryStream.Length, memoryStream.Position);
				}
			}
			EntitySystemManagerTests.assertStandardEntityArray(destinationEntityArray);
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
			entityArray.BeginUpdate();
			entityArray.TryCreateEntity(out Entity entity0);
			entityArray.TryCreateEntity(out Entity entity1);
			entityArray.TryCreateEntity(out Entity entity2);
			entity0.AddComponent<PositionComponent2D>().PositionX = 60.5f;
			entity0.AddComponent<PositionComponent2D>().PositionY = 60.9f;
			entity1.AddComponent<PositionComponent2D>().PositionX = 61.5f;
			entity1.AddComponent<PositionComponent2D>().PositionY = 61.9f;
			entity2.AddComponent<PositionComponent2D>().PositionX = 62.5f;
			entity2.AddComponent<PositionComponent2D>().PositionY = 62.9f;
			//entity0.AddComponent<HealthComponent>().HealthAmount = 70;
			entity1.AddComponent<HealthComponent>().HealthAmount = 71;
			entity2.AddComponent<HealthComponent>().HealthAmount = 72;
			entity0.AddComponent<StringComponent>().StringValue = "entity0";
			entity1.AddComponent<StringComponent>().StringValue = "entity1";
			//entity2.AddComponent<StringComponent>().StringValue = "entity2";
			entityArray.RemoveEntity(entity1);
			entityArray.EndUpdate();
			EntitySystemManagerTests.assertStandardEntityArray(entityArray);
			return entityArray;
		}

		private static void assertStandardEntityArray(EntityArray entityArray)
		{
			Assert.IsTrue(entityArray.TryGetEntity(0, out Entity entity0));
			Assert.IsFalse(entityArray.TryGetEntity(1, out Entity entity1));
			Assert.IsTrue(entityArray.TryGetEntity(2, out Entity entity2));
			Assert.IsTrue(entity0.HasComponent<PositionComponent2D>());
			Assert.IsTrue(entity2.HasComponent<PositionComponent2D>());
			Assert.IsFalse(entity0.HasComponent<HealthComponent>());
			Assert.IsTrue(entity2.HasComponent<HealthComponent>());
			Assert.IsTrue(entity0.HasComponent<StringComponent>());
			Assert.IsFalse(entity2.HasComponent<StringComponent>());
			Assert.AreEqual(60.5f, entity0.GetComponent<PositionComponent2D>().PositionX);
			Assert.AreEqual(60.9f, entity0.GetComponent<PositionComponent2D>().PositionY);
			Assert.AreEqual(62.5f, entity2.GetComponent<PositionComponent2D>().PositionX);
			Assert.AreEqual(62.9f, entity2.GetComponent<PositionComponent2D>().PositionY);
			Assert.AreEqual(72, entity2.GetComponent<HealthComponent>().HealthAmount);
			Assert.AreEqual("entity0", entity0.GetComponent<StringComponent>().StringValue);
		}

		#endregion Helpers

		#region Nested Types

		public struct PositionComponent2D : IComponent<PositionComponent2D>
		{
			#region Fields

			public float PositionX;
			public float PositionY;

			#endregion Fields

			#region Methods

			public void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.PositionX);
				binaryWriter.Write(this.PositionY);
			}

			public void Deserialize(BinaryReader binaryReader)
			{
				this.PositionX = binaryReader.ReadSingle();
				this.PositionY = binaryReader.ReadSingle();
			}

			#endregion Methods
		}

		public struct HealthComponent : IComponent<HealthComponent>
		{
			#region Fields

			public int HealthAmount;

			#endregion Fields

			#region Methods

			public void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.HealthAmount);
			}

			public void Deserialize(BinaryReader binaryReader)
			{
				this.HealthAmount = binaryReader.ReadInt32();
			}

			#endregion Methods
		}

		public struct StringComponent : IComponent<StringComponent>
		{
			#region Fields

			public string StringValue;

			#endregion Fields

			#region Methods

			public void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.StringValue ?? "");
			}

			public void Deserialize(BinaryReader binaryReader)
			{
				this.StringValue = binaryReader.ReadString();
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
