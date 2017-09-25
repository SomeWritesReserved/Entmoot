using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a message that can be read from which has come in from another endpoint.
	/// </summary>
	public class IncomingMessage : IReader
	{
		#region Fields

		/// <summary>The current location to write data to.</summary>
		private int dataIndex = 0;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public IncomingMessage(byte[] messageData)
		{
			this.MessageData = messageData;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets that raw underlying byte array that is the message data.
		/// </summary>
		public byte[] MessageData { get; }

		/// <summary>
		/// Gets the length of this message (the amount of data that has been written to it).
		/// </summary>
		public int Length { get; private set; }

		/// <summary>
		/// Gets the current position of the read head (showing which data will be read next).
		/// </summary>
		public int Position { get { return this.dataIndex; } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Resets this message so begins reading from the beginning.
		/// </summary>
		public void Reset()
		{
			this.dataIndex = 0;
			this.Length = 0;
		}

		/// <summary>
		/// Copies data from an outgoing message to this message (simulating its data arriving as an incoming message).
		/// </summary>
		public void CopyFrom(OutgoingMessage outgoingMessage)
		{
			this.Reset();
			this.Length = outgoingMessage.Length;
			Array.Copy(outgoingMessage.MessageData, this.MessageData, this.Length);
		}

		/// <summary>
		/// Copies data from a binary source to this message.
		/// </summary>
		public void CopyFrom(byte[] data, int numberOfBytes)
		{
			this.Reset();
			this.Length = numberOfBytes;
			Array.Copy(data, this.MessageData, this.Length);
		}

		/// <summary>
		/// Reads an unsigned byte from the message.
		/// </summary>
		public byte ReadByte()
		{
			byte value = this.MessageData[this.dataIndex];
			this.dataIndex += sizeof(byte);
			return value;
		}

		/// <summary>
		/// Reads a signed 16-bit integer from the message.
		/// </summary>
		public short ReadInt16()
		{
			short value = (short)((this.MessageData[this.dataIndex + 0] << 8) |
				this.MessageData[this.dataIndex + 1]);
			this.dataIndex += sizeof(short);
			return value;
		}

		/// <summary>
		/// Reads an unsigned 16-bit integer from the message.
		/// </summary>
		public ushort ReadUInt16()
		{
			ushort value = (ushort)((this.MessageData[this.dataIndex + 0] << 8) |
				this.MessageData[this.dataIndex + 1]);
			this.dataIndex += sizeof(ushort);
			return value;
		}

		/// <summary>
		/// Reads a signed 32-bit integer from the message.
		/// </summary>
		public int ReadInt32()
		{
			int value = (int)((this.MessageData[this.dataIndex + 0] << 24) |
				(this.MessageData[this.dataIndex + 1] << 16) |
				(this.MessageData[this.dataIndex + 2] << 8) |
				this.MessageData[this.dataIndex + 3]);
			this.dataIndex += sizeof(int);
			return value;
		}

		/// <summary>
		/// Reads an unsigned 32-bit integer from the message.
		/// </summary>
		public uint ReadUInt32()
		{
			uint value = (uint)((this.MessageData[this.dataIndex + 0] << 24) |
				(this.MessageData[this.dataIndex + 1] << 16) |
				(this.MessageData[this.dataIndex + 2] << 8) |
				this.MessageData[this.dataIndex + 3]);
			this.dataIndex += sizeof(uint);
			return value;
		}

		/// <summary>
		/// Reads a 32-bit floating point number from the message.
		/// </summary>
		public float ReadSingle()
		{
			SingleToUIntUnion singleToUIntUnion = new SingleToUIntUnion();
			singleToUIntUnion.UIntValue = this.ReadUInt32();
			return singleToUIntUnion.SingleValue;
		}

		/// <summary>
		/// Reads a boolean value from the message.
		/// </summary>
		public bool ReadBoolean()
		{
			return (this.ReadByte() == 1);
		}

		/// <summary>
		/// Reads a string from the message.
		/// </summary>
		public string ReadString()
		{
			int length = this.ReadInt32();
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append((char)this.ReadByte());
			}
			return stringBuilder.ToString();
		}

		#endregion Methods
	}

	/// <summary>
	/// Represents a message which can be written to that will go out to other endpoints
	/// </summary>
	public class OutgoingMessage : IWriter
	{
		#region Fields

		/// <summary>The current location to write data to.</summary>
		private int dataIndex = 0;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMessage(byte[] messageData)
		{
			this.MessageData = messageData;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets that raw underlying byte array that is the message data.
		/// </summary>
		public byte[] MessageData { get; }

		/// <summary>
		/// Gets the length of this message (the amount of data that has been written to it).
		/// </summary>
		public int Length { get { return this.dataIndex; } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Resets this message so begins writing at the beginning.
		/// </summary>
		public void Reset()
		{
			this.dataIndex = 0;
		}

		/// <summary>
		/// Returns a new byte array that has the data that this message wrote.
		/// </summary>
		public byte[] ToArray()
		{
			byte[] newData = new byte[this.dataIndex];
			Array.Copy(this.MessageData, newData, newData.Length);
			return newData;
		}

		/// <summary>
		/// Writes an unsigned byte to the message.
		/// </summary>
		public void Write(byte value)
		{
			this.MessageData[this.dataIndex] = value;
			this.dataIndex += sizeof(byte);
		}

		/// <summary>
		/// Writes an signed 16-bit integer to the message.
		/// </summary>
		public void Write(short value)
		{
			this.MessageData[this.dataIndex + 0] = (byte)(value >> 8);
			this.MessageData[this.dataIndex + 1] = (byte)value;
			this.dataIndex += sizeof(short);
		}

		/// <summary>
		/// Writes an unsigned 16-bit integer to the message.
		/// </summary>
		public void Write(ushort value)
		{
			this.MessageData[this.dataIndex + 0] = (byte)(value >> 8);
			this.MessageData[this.dataIndex + 1] = (byte)value;
			this.dataIndex += sizeof(ushort);
		}

		/// <summary>
		/// Writes an signed 32-bit integer to the message.
		/// </summary>
		public void Write(int value)
		{
			this.MessageData[this.dataIndex + 0] = (byte)(value >> 24);
			this.MessageData[this.dataIndex + 1] = (byte)(value >> 16);
			this.MessageData[this.dataIndex + 2] = (byte)(value >> 8);
			this.MessageData[this.dataIndex + 3] = (byte)value;
			this.dataIndex += sizeof(int);
		}

		/// <summary>
		/// Writes an unsigned 32-bit integer to the message.
		/// </summary>
		public void Write(uint value)
		{
			this.MessageData[this.dataIndex + 0] = (byte)(value >> 24);
			this.MessageData[this.dataIndex + 1] = (byte)(value >> 16);
			this.MessageData[this.dataIndex + 2] = (byte)(value >> 8);
			this.MessageData[this.dataIndex + 3] = (byte)value;
			this.dataIndex += sizeof(uint);
		}

		/// <summary>
		/// Writes a 32-bit floating point number to the message.
		/// </summary>
		public void Write(float value)
		{
			SingleToUIntUnion singleToUIntUnion = new SingleToUIntUnion()
			{
				SingleValue = value,
			};
			this.Write(singleToUIntUnion.UIntValue);
		}

		/// <summary>
		/// Writes a boolean value to the message.
		/// </summary>
		public void Write(bool value)
		{
			this.Write(value ? (byte)1 : (byte)0);
		}

		/// <summary>
		/// Writes a string to the message.
		/// </summary>
		public void Write(string value)
		{
			this.Write(value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				this.Write((byte)value[i]);
			}
		}

		#endregion Methods
	}

	/// <summary>Convenience type to convert floats to ints (and vice-versa) byte for byte.</summary>
	[StructLayout(LayoutKind.Explicit)]
	internal struct SingleToUIntUnion
	{
		/// <summary>The value as a 32-bit floating point number.</summary>
		[FieldOffset(0)]
		public float SingleValue;

		/// <summary>The value as an unsigned 32-bit integer.</summary>
		[FieldOffset(0)]
		public uint UIntValue;
	}
}
