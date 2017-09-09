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
		public StateArray(int length)
		{
			if (length <= 0) { throw new ArgumentOutOfRangeException(nameof(length)); }

			int storageArrayLength = ((length - 1) / 32) + 1;
			this.storageArray = new int[storageArrayLength];
		}

		#endregion Constructors

		#region Properties

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
			return (this.storageArray[index / 32] & 1 << index % 32) != 0;
		}

		/// <summary>
		/// Sets the value of the boolean state at a specific index.
		/// </summary>
		public void Set(int index, bool value)
		{
			if (value)
			{
				this.storageArray[index / 32] |= 1 << index % 32;
			}
			else
			{
				this.storageArray[index / 32] &= ~(1 << index % 32);
			}
		}

		/// <summary>
		/// Copies all state data to another state array.
		/// </summary>
		public void CopyTo(StateArray other)
		{
			Array.Copy(this.storageArray, other.storageArray, this.storageArray.Length);
		}

		#endregion Methods
	}
}
