using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Entmoot.Engine.UnitTests
{
	[TestFixture]
	public class UpdateSerializersTests
	{
		#region Tests

		[Test]
		public void ServerUpdateSerializer_Serialization_Empty()
		{
			byte[] serializedData = new byte[1024];
			int messageLength = 0;
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(66, entityArray);
				OutgoingMessage outgoingMessage = new OutgoingMessage(serializedData);
				ServerUpdateSerializer.Serialize(outgoingMessage, null, entitySnapshot, 63, 13);
				serializedData = outgoingMessage.ToArray();
				messageLength = outgoingMessage.Length;
			}
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				IncomingMessage incomingMessage = new IncomingMessage(serializedData);
				incomingMessage.Length = messageLength;
				ServerUpdateSerializer.Deserialize(incomingMessage, new EntitySnapshot[0], entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID);
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
				ServerUpdateSerializer.Serialize(outgoingMessage, null, entitySnapshot, 73, 17);
				serializedData = outgoingMessage.ToArray();
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				ServerUpdateSerializer.Deserialize(new IncomingMessage(serializedData), new EntitySnapshot[0], entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID);
				Assert.AreEqual(73, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(17, clientCommandingEntityID);
				Assert.AreEqual(68, entitySnapshot.ServerFrameTick);
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
				ServerUpdateSerializer.Serialize(outgoingMessage, null, entitySnapshot, 73, 17);
				serializedData = outgoingMessage.ToArray();
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				Assert.IsFalse(ServerUpdateSerializer.DeserializeIfNewer(new IncomingMessage(serializedData), new EntitySnapshot[0], entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID));
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
			int messageLength;
			{
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(68, entitySnapshot.EntityArray);
				OutgoingMessage outgoingMessage = new OutgoingMessage(serializedData);
				ServerUpdateSerializer.Serialize(outgoingMessage, null, entitySnapshot, 73, 17);
				serializedData = outgoingMessage.ToArray();
				messageLength = outgoingMessage.Length;
			}
			{
				EntityArray entityArray = EntityTests.CreateStandardEntityArray();
				EntitySnapshot entitySnapshot = new EntitySnapshot(3, EntityTests.CreateComponentsDefinition());
				entitySnapshot.Update(65, entityArray);
				IncomingMessage incomingMessage = new IncomingMessage(serializedData);
				incomingMessage.Length = messageLength;
				Assert.IsTrue(ServerUpdateSerializer.DeserializeIfNewer(incomingMessage, new EntitySnapshot[0], entitySnapshot, out int latestClientTickAcknowledgedByServer, out int clientCommandingEntityID));
				Assert.AreEqual(73, latestClientTickAcknowledgedByServer);
				Assert.AreEqual(17, clientCommandingEntityID);
				Assert.AreEqual(68, entitySnapshot.ServerFrameTick);
			}
		}

		#endregion Tests
	}
}
