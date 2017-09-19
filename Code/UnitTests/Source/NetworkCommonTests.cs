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
	public class NetworkCommonTests
	{
		#region Tests

		[Test]
		public void ServerUpdateSerializer_Serialization_Empty()
		{
			byte[] packet;
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(66, entityArray);
				packet = ServerUpdateSerializer.Serialize(entitySnapshot, 63, 13);
			}
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				ServerUpdateSerializer.Deserialize(packet, entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID);
				Assert.AreEqual(63, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(13, clientCommandingEntityID);
				Assert.AreEqual(66, entitySnapshot.ServerFrameTick);
				EntityTests.AssertStandardEntityArray(entitySnapshot.EntityArray);
			}
		}

		[Test]
		public void ServerUpdateSerializer_Serialization_Overwrite()
		{
			byte[] packet;
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(68, entitySnapshot.EntityArray);
				packet = ServerUpdateSerializer.Serialize(entitySnapshot, 73, 17);
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				ServerUpdateSerializer.Deserialize(packet, entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID);
				Assert.AreEqual(73, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(17, clientCommandingEntityID);
				Assert.AreEqual(68, entitySnapshot.ServerFrameTick);
				Assert.IsFalse(entitySnapshot.EntityArray.TryGetEntity(0, out _));
				Assert.IsFalse(entitySnapshot.EntityArray.TryGetEntity(1, out _));
				Assert.IsFalse(entitySnapshot.EntityArray.TryGetEntity(2, out _));
			}
		}

		[Test]
		public void ServerUpdateSerializer_DeserializeIfNewer_NotNewer()
		{
			byte[] packet;
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(55, entitySnapshot.EntityArray);
				packet = ServerUpdateSerializer.Serialize(entitySnapshot, 73, 17);
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				Assert.IsFalse(ServerUpdateSerializer.DeserializeIfNewer(packet, entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID));
				Assert.AreEqual(73, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(17, clientCommandingEntityID);
				Assert.AreEqual(65, entitySnapshot.ServerFrameTick);
				EntityTests.AssertStandardEntityArray(entitySnapshot.EntityArray);
			}
		}

		[Test]
		public void ServerUpdateSerializer_DeserializeIfNewer_IsNewer()
		{
			byte[] packet;
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(68, entitySnapshot.EntityArray);
				packet = ServerUpdateSerializer.Serialize(entitySnapshot, 73, 17);
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				Assert.IsTrue(ServerUpdateSerializer.DeserializeIfNewer(packet, entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID));
				Assert.AreEqual(73, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(17, clientCommandingEntityID);
				Assert.AreEqual(68, entitySnapshot.ServerFrameTick);
				Assert.IsFalse(entitySnapshot.EntityArray.TryGetEntity(0, out _));
				Assert.IsFalse(entitySnapshot.EntityArray.TryGetEntity(1, out _));
				Assert.IsFalse(entitySnapshot.EntityArray.TryGetEntity(2, out _));
			}
		}

		#endregion Tests
	}
}
