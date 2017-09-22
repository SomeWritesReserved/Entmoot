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
			byte[] serializedData = new byte[1024];
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(66, entityArray);
				OutgoingMessage outgoingMessage = new OutgoingMessage(serializedData);
				ServerUpdateSerializer.Serialize(outgoingMessage, entitySnapshot, 63, 13);
				serializedData = outgoingMessage.ToArray();
			}
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				ServerUpdateSerializer.Deserialize(new IncomingMessage(serializedData), entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID);
				Assert.AreEqual(63, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(13, clientCommandingEntityID);
				Assert.AreEqual(66, entitySnapshot.ServerFrameTick);
				EntityTests.AssertStandardEntityArray(entitySnapshot.EntityArray);
			}
		}

		[Test]
		public void ServerUpdateSerializer_Serialization_Overwrite()
		{
			byte[] serializedData = new byte[1024];
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(68, entitySnapshot.EntityArray);
				OutgoingMessage outgoingMessage = new OutgoingMessage(serializedData);
				ServerUpdateSerializer.Serialize(outgoingMessage, entitySnapshot, 73, 17);
				serializedData = outgoingMessage.ToArray();
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				ServerUpdateSerializer.Deserialize(new IncomingMessage(serializedData), entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID);
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
			byte[] serializedData = new byte[1024];
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(55, entitySnapshot.EntityArray);
				OutgoingMessage outgoingMessage = new OutgoingMessage(serializedData);
				ServerUpdateSerializer.Serialize(outgoingMessage, entitySnapshot, 73, 17);
				serializedData = outgoingMessage.ToArray();
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				Assert.IsFalse(ServerUpdateSerializer.DeserializeIfNewer(new IncomingMessage(serializedData), entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID));
				Assert.AreEqual(73, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(17, clientCommandingEntityID);
				Assert.AreEqual(65, entitySnapshot.ServerFrameTick);
				EntityTests.AssertStandardEntityArray(entitySnapshot.EntityArray);
			}
		}

		[Test]
		public void ServerUpdateSerializer_DeserializeIfNewer_IsNewer()
		{
			byte[] serializedData = new byte[1024];
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(68, entitySnapshot.EntityArray);
				OutgoingMessage outgoingMessage = new OutgoingMessage(serializedData);
				ServerUpdateSerializer.Serialize(outgoingMessage, entitySnapshot, 73, 17);
				serializedData = outgoingMessage.ToArray();
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				Assert.IsTrue(ServerUpdateSerializer.DeserializeIfNewer(new IncomingMessage(serializedData), entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID));
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
