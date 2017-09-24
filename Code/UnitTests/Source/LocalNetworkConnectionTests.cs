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
				localNetworkConnectionA.SendMessage(outgoingMessage);
			}
			{
				IncomingMessage incomingMessage = localNetworkConnectionB.GetNextIncomingMessage();
				Assert.IsNotNull(incomingMessage);
				Assert.AreEqual(1300, incomingMessage.MessageData.Length);
				Assert.AreEqual(true, incomingMessage.ReadBoolean());
				Assert.AreEqual(123, incomingMessage.ReadInt32());
				Assert.AreEqual("yes", incomingMessage.ReadString());
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

		#endregion Tests
	}
}
