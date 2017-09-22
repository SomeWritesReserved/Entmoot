using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an array of boolean state values that can be either on or off.
	/// </summary>
	public class StateArray
	{
		#region Fields

		/// <summary>The array that the individual bits are stored in, as effeciently as possible.</summary>
		private int[] storageArray;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public StateArray(int capacity)
		{
			if (capacity <= 0) { throw new ArgumentOutOfRangeException(nameof(capacity)); }

			int storageArrayLength = ((capacity - 1) / 32) + 1;
			this.storageArray = new int[storageArrayLength];
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the maximum number of boolean state values that can be stored.
		/// </summary>
		public int Capacity
		{
			get { return this.storageArray.Length * 32; }
		}

		/// <summary>
		/// Gets or sets the value of the boolean state at a specific index.
		/// </summary>
		public bool this[int index]
		{
			get { return this.Get(index); }
			set { this.Set(index, value); }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns the value of the boolean state at a specific index.
		/// </summary>
		public bool Get(int index)
		{
			return ((this.storageArray[index / 32] & (1 << (index % 32))) != 0);
		}

		/// <summary>
		/// Sets the value of the boolean state at a specific index.
		/// </summary>
		public void Set(int index, bool value)
		{
			if (value) { this.storageArray[index / 32] |= (1 << (index % 32)); }
			else { this.storageArray[index / 32] &= ~(1 << (index % 32)); }
		}

		/// <summary>
		/// Copies all state data to another state array.
		/// </summary>
		public void CopyTo(StateArray other)
		{
			Array.Copy(this.storageArray, other.storageArray, this.storageArray.Length);
		}

		/// <summary>
		/// Writes all boolean states to a binary source.
		/// </summary>
		public void Serialize(IWriter writer)
		{
			for (int i = 0; i < this.storageArray.Length; i++)
			{
				writer.Write(this.storageArray[i]);
			}
		}

		/// <summary>
		/// Reads and overwrites all boolean states from a binary source.
		/// </summary>
		public void Deserialize(IReader reader)
		{
			for (int i = 0; i < this.storageArray.Length; i++)
			{
				this.storageArray[i] = reader.ReadInt32();
			}
		}

		/// <summary>
		/// Returns a byte array that has this boolean state data packed in to it.
		/// </summary>
		public byte[] ToByteArray()
		{
			byte[] bytes = new byte[this.storageArray.Length * 4];
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = (byte)(this.storageArray[i / 4] >> ((i % 4) * (8 & 255)));
			}
			return bytes;
		}

		#endregion Methods
	}
}
