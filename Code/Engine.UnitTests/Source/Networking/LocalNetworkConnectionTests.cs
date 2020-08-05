using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Entmoot.Engine.UnitTests
{
	[TestFixture]
	public class LocalNetworkConnectionTests
	{
		#region Tests

		[Test]
		public void LocalNetworkConnection_OneMessage()
		{
			LocalNetworkConnection localNetworkConnectionA = new LocalNetworkConnection(1300);
			LocalNetworkConnection localNetworkConnectionB = localNetworkConnectionA.GetPairedNetworkConnection();
			{
				OutgoingMessage outgoingMessage = localNetworkConnectionA.GetOutgoingMessageToSend();
				Assert.AreEqual(1300, outgoingMessage.MessageData.Length);
				outgoingMessage.Write(true);
				outgoingMessage.Write(123);
				outgoingMessage.Write("yes");
				Assert.AreEqual(9, outgoingMessage.Length);
				localNetworkConnectionA.SendMessage(outgoingMessage);
			}
			{
				IncomingMessage incomingMessage = localNetworkConnectionB.GetNextIncomingMessage();
				Assert.IsNotNull(incomingMessage);
				Assert.AreEqual(1300, incomingMessage.MessageData.Length);
				Assert.AreEqual(9, incomingMessage.Length);
				Assert.AreEqual(0, incomingMessage.Position);
				Assert.AreEqual(true, incomingMessage.ReadBoolean());
				Assert.AreEqual(1, incomingMessage.Position);
				Assert.AreEqual(8, incomingMessage.BytesLeft);
				Assert.AreEqual(123, incomingMessage.ReadInt32());
				Assert.AreEqual(5, incomingMessage.Position);
				Assert.AreEqual(4, incomingMessage.BytesLeft);
				Assert.AreEqual("yes", incomingMessage.ReadString());
				Assert.AreEqual(9, incomingMessage.Position);
				Assert.AreEqual(0, incomingMessage.BytesLeft);
				Assert.IsNull(localNetworkConnectionB.GetNextIncomingMessage());
			}
		}

		[Test]
		public void LocalNetworkConnection_TwoMessages()
		{
			LocalNetworkConnection localNetworkConnectionA = new LocalNetworkConnection(1300);
			LocalNetworkConnection localNetworkConnectionB = localNetworkConnectionA.GetPairedNetworkConnection();
			{
				OutgoingMessage outgoingMessage = localNetworkConnectionA.GetOutgoingMessageToSend();
				Assert.AreEqual(1300, outgoingMessage.MessageData.Length);
				outgoingMessage.Write(true);
				outgoingMessage.Write(100.1f);
				outgoingMessage.Write("yes");
				localNetworkConnectionA.SendMessage(outgoingMessage);
			}
			{
				OutgoingMessage outgoingMessage = localNetworkConnectionA.GetOutgoingMessageToSend();
				Assert.AreEqual(1300, outgoingMessage.MessageData.Length);
				outgoingMessage.Write(false);
				outgoingMessage.Write(200.2f);
				outgoingMessage.Write("no");
				outgoingMessage.Write(123);
				localNetworkConnectionA.SendMessage(outgoingMessage);
			}
			{
				IncomingMessage incomingMessage = localNetworkConnectionB.GetNextIncomingMessage();
				Assert.IsNotNull(incomingMessage);
				Assert.AreEqual(1300, incomingMessage.MessageData.Length);
				Assert.AreEqual(true, incomingMessage.ReadBoolean());
				Assert.AreEqual(100.1f, incomingMessage.ReadSingle());
				Assert.AreEqual("yes", incomingMessage.ReadString());
			}
			{
				IncomingMessage incomingMessage = localNetworkConnectionB.GetNextIncomingMessage();
				Assert.IsNotNull(incomingMessage);
				Assert.AreEqual(1300, incomingMessage.MessageData.Length);
				Assert.AreEqual(false, incomingMessage.ReadBoolean());
				Assert.AreEqual(200.2f, incomingMessage.ReadSingle());
				Assert.AreEqual("no", incomingMessage.ReadString());
				Assert.AreEqual(123, incomingMessage.ReadInt32());
				Assert.IsNull(localNetworkConnectionB.GetNextIncomingMessage());
			}
		}

		[Test]
		public void LocalNetworkConnection_ManyMessages_Sequential()
		{
			LocalNetworkConnection localNetworkConnectionA = new LocalNetworkConnection(1300);
			LocalNetworkConnection localNetworkConnectionB = localNetworkConnectionA.GetPairedNetworkConnection();
			for (int i = 0; i < 20; i++)
			{
				OutgoingMessage outgoingMessage = localNetworkConnectionA.GetOutgoingMessageToSend();
				outgoingMessage.Write(((i % 2) == 0));
				outgoingMessage.Write(i);
				outgoingMessage.Write(i / 2.0f);
				outgoingMessage.Write(i.ToString());
				localNetworkConnectionA.SendMessage(outgoingMessage);

				IncomingMessage incomingMessage = localNetworkConnectionB.GetNextIncomingMessage();
				Assert.IsNotNull(incomingMessage);
				Assert.AreEqual(0, incomingMessage.Position);
				Assert.AreEqual(((i % 2) == 0), incomingMessage.ReadBoolean());
				Assert.AreEqual(i, incomingMessage.ReadInt32());
				Assert.AreEqual(i / 2.0f, incomingMessage.ReadSingle());
				Assert.AreEqual(i.ToString(), incomingMessage.ReadString());
				Assert.IsNull(localNetworkConnectionB.GetNextIncomingMessage());
			}
		}

		[Test]
		public void LocalNetworkConnection_ManyMessages_Queued()
		{
			LocalNetworkConnection localNetworkConnectionA = new LocalNetworkConnection(1300);
			LocalNetworkConnection localNetworkConnectionB = localNetworkConnectionA.GetPairedNetworkConnection();
			for (int i = 0; i < 20; i++)
			{
				OutgoingMessage outgoingMessage = localNetworkConnectionA.GetOutgoingMessageToSend();
				outgoingMessage.Write(((i % 2) == 0));
				outgoingMessage.Write(i);
				outgoingMessage.Write(i / 2.0f);
				outgoingMessage.Write(i.ToString());
				localNetworkConnectionA.SendMessage(outgoingMessage);
			}
			for (int i = 0; i < 20; i++)
			{
				IncomingMessage incomingMessage = localNetworkConnectionB.GetNextIncomingMessage();
				Assert.IsNotNull(incomingMessage);
				Assert.AreEqual(0, incomingMessage.Position);
				Assert.AreEqual(((i % 2) == 0), incomingMessage.ReadBoolean());
				Assert.AreEqual(i, incomingMessage.ReadInt32());
				Assert.AreEqual(i / 2.0f, incomingMessage.ReadSingle());
				Assert.AreEqual(i.ToString(), incomingMessage.ReadString());
			}
			Assert.IsNull(localNetworkConnectionB.GetNextIncomingMessage());
		}

		#endregion Tests
	}
}
