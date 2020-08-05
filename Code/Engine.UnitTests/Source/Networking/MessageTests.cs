using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Entmoot.Engine.UnitTests
{
	[TestFixture]
	public class NetworkMessageTests
	{
		#region Tests

		[Test]
		public void OutgoingMessage_Reset()
		{
			byte[] data = new byte[256];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			outgoingMessage.Write(1234.555f);
			outgoingMessage.Write(1200);
			Assert.AreEqual(8, outgoingMessage.Length);
			outgoingMessage.Reset();
			Assert.AreEqual(0, outgoingMessage.Length);
		}

		[Test]
		public void OutgoingAndIncoming_Byte()
		{
			byte[] data = new byte[256];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			for (int value = byte.MinValue; value <= byte.MaxValue; value++)
			{
				outgoingMessage.Write((byte)value);
			}
			Assert.AreEqual(data.Length, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (int value = byte.MinValue; value < byte.MaxValue; value++)
			{
				Assert.AreEqual((byte)value, incomingMessage.ReadByte());
			}
		}

		[Test]
		public void OutgoingAndIncoming_Int16()
		{
			byte[] data = new byte[258 * sizeof(short)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			for (int value = short.MinValue; value <= short.MaxValue; value += 255)
			{
				outgoingMessage.Write((short)value);
			}
			Assert.AreEqual(data.Length, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (int value = short.MinValue; value <= short.MaxValue; value += 255)
			{
				Assert.AreEqual((short)value, incomingMessage.ReadInt16());
			}
		}

		[Test]
		public void OutgoingAndIncoming_UInt16()
		{
			byte[] data = new byte[258 * sizeof(ushort)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			for (uint value = ushort.MinValue; value <= ushort.MaxValue; value += 255)
			{
				outgoingMessage.Write((ushort)value);
			}
			Assert.AreEqual(data.Length, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (uint value = ushort.MinValue; value <= ushort.MaxValue; value += 255)
			{
				Assert.AreEqual((ushort)value, incomingMessage.ReadUInt16());
			}
		}

		[Test]
		public void OutgoingAndIncoming_Int32()
		{
			byte[] data = new byte[258 * sizeof(int)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			for (long value = int.MinValue; value <= int.MaxValue; value += 16711935)
			{
				outgoingMessage.Write((int)value);
			}
			Assert.AreEqual(data.Length, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (long value = int.MinValue; value <= int.MaxValue; value += 16711935)
			{
				Assert.AreEqual((int)value, incomingMessage.ReadInt32());
			}
		}

		[Test]
		public void OutgoingAndIncoming_UInt32()
		{
			byte[] data = new byte[258 * sizeof(uint)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			for (ulong value = uint.MinValue; value <= uint.MaxValue; value += 16711935)
			{
				outgoingMessage.Write((uint)value);
			}
			Assert.AreEqual(data.Length, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (ulong value = uint.MinValue; value <= uint.MaxValue; value += 16711935)
			{
				Assert.AreEqual((uint)value, incomingMessage.ReadUInt32());
			}
		}

		[Test]
		public void OutgoingAndIncoming_Single()
		{
			byte[] data = new byte[15 * sizeof(float)];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
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
			Assert.AreEqual(data.Length, outgoingMessage.Length);
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
			Assert.AreEqual(0, outgoingMessage.Length);
			for (int value = byte.MinValue; value <= byte.MaxValue; value++)
			{
				outgoingMessage.Write(((value % 2) == 0));
			}
			Assert.AreEqual(data.Length, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			for (int value = byte.MinValue; value <= byte.MaxValue; value++)
			{
				Assert.AreEqual(((value % 2) == 0), incomingMessage.ReadBoolean());
			}
		}

		[Test]
		public void OutgoingAndIncoming_String()
		{
			byte[] data = new byte[512];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			outgoingMessage.Write(string.Empty);
			outgoingMessage.Write("");
			outgoingMessage.Write("yyyyyyyyyes");
			outgoingMessage.Write("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t");
			Assert.AreEqual(112, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			Assert.AreEqual(string.Empty, incomingMessage.ReadString());
			Assert.AreEqual("", incomingMessage.ReadString());
			Assert.AreEqual("yyyyyyyyyes", incomingMessage.ReadString());
			Assert.AreEqual("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t", incomingMessage.ReadString());
		}

		[Test]
		public void OutgoingAndIncoming_StringBuilder()
		{
			byte[] data = new byte[512];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			outgoingMessage.Write(new StringBuilder());
			outgoingMessage.Write(new StringBuilder(""));
			outgoingMessage.Write(new StringBuilder("yyyyyyyyyes"));
			outgoingMessage.Write(new StringBuilder("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t"));
			Assert.AreEqual(112, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual(string.Empty, stringBuilder.ToString());
			}
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("", stringBuilder.ToString());
			}
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("yyyyyyyyyes", stringBuilder.ToString());
			}
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t", stringBuilder.ToString());
			}
		}

		[Test]
		public void OutgoingAndIncoming_StringBuilderAppend()
		{
			byte[] data = new byte[512];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			outgoingMessage.Write(new StringBuilder());
			outgoingMessage.Write(new StringBuilder(""));
			outgoingMessage.Write(new StringBuilder("yyyyyyyyyes"));
			outgoingMessage.Write(new StringBuilder("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t"));
			Assert.AreEqual(112, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			StringBuilder stringBuilder = new StringBuilder(16);
			{
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual(string.Empty, stringBuilder.ToString());
			}
			{
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("", stringBuilder.ToString());
			}
			{
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("yyyyyyyyyes", stringBuilder.ToString());
			}
			{
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("yyyyyyyyyes" + "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t", stringBuilder.ToString());
			}
		}

		[Test]
		public void OutgoingAndIncoming_StringBuilderToString()
		{
			byte[] data = new byte[512];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			outgoingMessage.Write(new StringBuilder());
			outgoingMessage.Write(new StringBuilder(""));
			outgoingMessage.Write(new StringBuilder("yyyyyyyyyes"));
			outgoingMessage.Write(new StringBuilder("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t"));
			Assert.AreEqual(112, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			Assert.AreEqual(string.Empty, incomingMessage.ReadString());
			Assert.AreEqual("", incomingMessage.ReadString());
			Assert.AreEqual("yyyyyyyyyes", incomingMessage.ReadString());
			Assert.AreEqual("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t", incomingMessage.ReadString());
		}

		[Test]
		public void OutgoingAndIncoming_StringToStringBuilder()
		{
			byte[] data = new byte[512];
			OutgoingMessage outgoingMessage = new OutgoingMessage(data);
			Assert.AreEqual(0, outgoingMessage.Length);
			outgoingMessage.Write(string.Empty);
			outgoingMessage.Write("");
			outgoingMessage.Write("yyyyyyyyyes");
			outgoingMessage.Write("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t");
			Assert.AreEqual(112, outgoingMessage.Length);
			IncomingMessage incomingMessage = new IncomingMessage(data);
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual(string.Empty, stringBuilder.ToString());
			}
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("", stringBuilder.ToString());
			}
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("yyyyyyyyyes", stringBuilder.ToString());
			}
			{
				StringBuilder stringBuilder = new StringBuilder(16);
				incomingMessage.ReadString(stringBuilder);
				Assert.AreEqual("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[{]}\\|;:'\"/?.>,<\r\n\t", stringBuilder.ToString());
			}
		}

		#endregion Tests
	}
}
