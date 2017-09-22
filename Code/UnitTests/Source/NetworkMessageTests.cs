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
	public class NetworkMessageTests
	{
		#region Tests

		[Test]
		public void OutgoingAndIncoming_Byte()
		{
			byte[] data = new byte[256];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			for (byte value = byte.MinValue; value < byte.MaxValue; value++)
			{
				outgoingMessage.Write(value);
			}
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (byte value = byte.MinValue; value < byte.MaxValue; value++)
			{
				Assert.AreEqual(value, incomingMessage.ReadByte());
			}
		}

		[Test]
		public void OutgoingAndIncoming_Int16()
		{
			byte[] data = new byte[257 * sizeof(short)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			for (short value = short.MinValue; value < short.MaxValue; value += 255)
			{
				outgoingMessage.Write(value);
			}
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (short value = short.MinValue; value < short.MaxValue; value += 255)
			{
				Assert.AreEqual(value, incomingMessage.ReadInt16());
			}
		}

		[Test]
		public void OutgoingAndIncoming_UInt16()
		{
			byte[] data = new byte[257 * sizeof(ushort)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			for (ushort value = ushort.MinValue; value < ushort.MaxValue; value += 255)
			{
				outgoingMessage.Write(value);
			}
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (ushort value = ushort.MinValue; value < ushort.MaxValue; value += 255)
			{
				Assert.AreEqual(value, incomingMessage.ReadUInt16());
			}
		}

		[Test]
		public void OutgoingAndIncoming_Int32()
		{
			byte[] data = new byte[257 * sizeof(int)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			for (int value = int.MinValue; value < int.MaxValue; value += 16711935)
			{
				outgoingMessage.Write(value);
			}
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (int value = int.MinValue; value < int.MaxValue; value += 16711935)
			{
				Assert.AreEqual(value, incomingMessage.ReadInt32());
			}
		}

		[Test]
		public void OutgoingAndIncoming_UInt32()
		{
			byte[] data = new byte[257 * sizeof(uint)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			for (uint value = uint.MinValue; value < uint.MaxValue; value += 16711935)
			{
				outgoingMessage.Write(value);
			}
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (uint value = uint.MinValue; value < uint.MaxValue; value += 16711935)
			{
				Assert.AreEqual(value, incomingMessage.ReadUInt32());
			}
		}

		[Test]
		public void OutgoingAndIncoming_Single()
		{
			byte[] data = new byte[15 * sizeof(float)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			outgoingMessage.Write(0.0f);
			outgoingMessage.Write(-0.0f);
			outgoingMessage.Write(10.0f);
			outgoingMessage.Write(-10.0f);
			outgoingMessage.Write(6546540.3654654f);
			outgoingMessage.Write(-957.548498f);
			outgoingMessage.Write(-99999.87888888888f);
			outgoingMessage.Write(0.00001f);
			outgoingMessage.Write(-0.2f);
			outgoingMessage.Write(Single.MinValue);
			outgoingMessage.Write(Single.MaxValue);
			outgoingMessage.Write(Single.NaN);
			outgoingMessage.Write(Single.NegativeInfinity);
			outgoingMessage.Write(Single.PositiveInfinity);
			outgoingMessage.Write(Single.Epsilon);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			Assert.AreEqual(0.0f, incomingMessage.ReadSingle());
			Assert.AreEqual(-0.0f, incomingMessage.ReadSingle());
			Assert.AreEqual(10.0f, incomingMessage.ReadSingle());
			Assert.AreEqual(-10.0f, incomingMessage.ReadSingle());
			Assert.AreEqual(6546540.3654654f, incomingMessage.ReadSingle());
			Assert.AreEqual(-957.548498f, incomingMessage.ReadSingle());
			Assert.AreEqual(-99999.87888888888f, incomingMessage.ReadSingle());
			Assert.AreEqual(0.00001f, incomingMessage.ReadSingle());
			Assert.AreEqual(-0.2f, incomingMessage.ReadSingle());
			Assert.AreEqual(Single.MinValue, incomingMessage.ReadSingle());
			Assert.AreEqual(Single.MaxValue, incomingMessage.ReadSingle());
			Assert.AreEqual(Single.NaN, incomingMessage.ReadSingle());
			Assert.AreEqual(Single.NegativeInfinity, incomingMessage.ReadSingle());
			Assert.AreEqual(Single.PositiveInfinity, incomingMessage.ReadSingle());
			Assert.AreEqual(Single.Epsilon, incomingMessage.ReadSingle());
		}

		[Test]
		public void OutgoingAndIncoming_Boolean()
		{
			byte[] data = new byte[256];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			for (byte value = byte.MinValue; value < byte.MaxValue; value++)
			{
				outgoingMessage.Write(((value % 2) == 0));
			}
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (byte value = byte.MinValue; value < byte.MaxValue; value++)
			{
				Assert.AreEqual(((value % 2) == 0), incomingMessage.ReadBoolean());
			}
		}

		#endregion Tests
	}
}
