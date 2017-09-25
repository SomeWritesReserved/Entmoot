using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an object that can read values from a data source.
	/// </summary>
	public interface IReader
	{
		#region Methods

		/// <summary>
		/// Reads an unsigned byte.
		/// </summary>
		byte ReadByte();

		/// <summary>
		/// Reads a signed 16-bit integer.
		/// </summary>
		short ReadInt16();

		/// <summary>
		/// Reads an unsigned 16-bit integer.
		/// </summary>
		ushort ReadUInt16();

		/// <summary>
		/// Reads a signed 32-bit integer.
		/// </summary>
		int ReadInt32();

		/// <summary>
		/// Reads an unsigned 32-bit integer.
		/// </summary>
		uint ReadUInt32();

		/// <summary>
		/// Reads a 32-bit floating point number.
		/// </summary>
		float ReadSingle();

		/// <summary>
		/// Reads a boolean value.
		/// </summary>
		bool ReadBoolean();

		/// <summary>
		/// Reads a string.
		/// </summary>
		string ReadString();

		#endregion Methods
	}

	/// <summary>
	/// Represents an object that can write values to a data source.
	/// </summary>
	public interface IWriter
	{
		#region Methods

		/// <summary>
		/// Writes an unsigned byte.
		/// </summary>
		void Write(byte value);

		/// <summary>
		/// Writes an signed 16-bit integer.
		/// </summary>
		void Write(short value);

		/// <summary>
		/// Writes an unsigned 16-bit integer.
		/// </summary>
		void Write(ushort value);

		/// <summary>
		/// Writes an signed 32-bit integer.
		/// </summary>
		void Write(int value);

		/// <summary>
		/// Writes an unsigned 32-bit integer.
		/// </summary>
		void Write(uint value);

		/// <summary>
		/// Writes a 32-bit floating point number.
		/// </summary>
		void Write(float value);

		/// <summary>
		/// Writes a boolean value.
		/// </summary>
		void Write(bool value);

		/// <summary>
		/// Writes a string value.
		/// </summary>
		void Write(string value);

		#endregion Methods
	}
}
