using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using NUnit.Framework;

namespace Entmoot.UnitTests
{
	public class StateArrayTests
	{
		#region Tests

		[Test]
		public void Create_Capacity()
		{
			Assert.AreEqual(32, new StateArray(1).Capacity);
			Assert.AreEqual(32, new StateArray(30).Capacity);
			Assert.AreEqual(32, new StateArray(31).Capacity);
			Assert.AreEqual(32, new StateArray(32).Capacity);
			Assert.AreEqual(64, new StateArray(33).Capacity);
			Assert.AreEqual(64, new StateArray(64).Capacity);
			Assert.AreEqual(96, new StateArray(65).Capacity);
			Assert.AreEqual(160, new StateArray(159).Capacity);
			Assert.AreEqual(160, new StateArray(160).Capacity);
			Assert.AreEqual(192, new StateArray(161).Capacity);
		}

		[Test]
		public void SetGet1()
		{
			StateArray stateArray = new StateArray(64);
			stateArray[0] = true;
			stateArray[1] = true;
			stateArray[8] = true;
			stateArray[32] = true;
			stateArray[33] = true;
			stateArray[62] = true;
			stateArray[63] = true;
			for (int i = 0; i < 64; i++)
			{
				if (i == 0 || i == 1 || i == 8 || i == 32 || i == 33 || i == 62 || i == 63)
				{
					Assert.IsTrue(stateArray[i]);
				}
				else
				{
					Assert.IsFalse(stateArray[i]);
				}
			}
		}

		[Test]
		public void SetGet2()
		{
			for (int i = 0; i < 160; i++)
			{
				StateArray stateArray = new StateArray(160);
				stateArray[i] = true;
				for (int k = 0; k < 160; k++)
				{
					if (k == i) { Assert.IsTrue(stateArray[k]); }
					else { Assert.IsFalse(stateArray[k]); }
				}
			}
		}

		[Test]
		public void ToByteArray1()
		{
			StateArray stateArray = new StateArray(64);
			BitArray referenceBitArray = new BitArray(64, false);
			stateArray[0] = referenceBitArray[0] = true;
			stateArray[1] = referenceBitArray[1] = true;
			stateArray[8] = referenceBitArray[8] = true;
			stateArray[32] = referenceBitArray[32] = true;
			stateArray[33] = referenceBitArray[33] = true;
			stateArray[62] = referenceBitArray[62] = true;
			stateArray[63] = referenceBitArray[63] = true;
			byte[] expectedBytes = new byte[64 / 8];
			referenceBitArray.CopyTo(expectedBytes, 0);
			byte[] actualBytes = stateArray.ToByteArray();
			Assert.IsTrue(actualBytes.SequenceEqual(expectedBytes));
		}

		[Test]
		public void ToByteArray2()
		{
			for (int i = 0; i < 160; i++)
			{
				StateArray stateArray = new StateArray(160);
				BitArray referenceBitArray = new BitArray(160, false);
				stateArray[i] = referenceBitArray[i] = true;
				byte[] expectedBytes = new byte[160 / 8];
				referenceBitArray.CopyTo(expectedBytes, 0);
				byte[] actualBytes = stateArray.ToByteArray();
				Assert.IsTrue(actualBytes.SequenceEqual(expectedBytes));
			}
		}

		[Test]
		public void CopyTo1()
		{
			StateArray sourceStateArray = new StateArray(64);
			sourceStateArray[0] = true;
			sourceStateArray[1] = true;
			sourceStateArray[8] = true;
			sourceStateArray[32] = true;
			sourceStateArray[33] = true;
			sourceStateArray[62] = true;
			sourceStateArray[63] = true;
			StateArray destinationStateArray = new StateArray(64);
			sourceStateArray.CopyTo(destinationStateArray);
			for (int i = 0; i < 64; i++)
			{
				if (i == 0 || i == 1 || i == 8 || i == 32 || i == 33 || i == 62 || i == 63)
				{
					Assert.IsTrue(destinationStateArray[i]);
				}
				else
				{
					Assert.IsFalse(destinationStateArray[i]);
				}
			}
		}

		[Test]
		public void CopyTo2()
		{
			for (int i = 0; i < 160; i++)
			{
				StateArray sourceStateArray = new StateArray(160);
				sourceStateArray[i] = true;
				StateArray destinationStateArray = new StateArray(160);
				sourceStateArray.CopyTo(destinationStateArray);
				for (int k = 0; k < 160; k++)
				{
					if (k == i) { Assert.IsTrue(destinationStateArray[k]); }
					else { Assert.IsFalse(destinationStateArray[k]); }
				}
			}
		}

		public void Serialize1()
		{
			StateArray sourceStateArray = new StateArray(64);
			sourceStateArray[0] = true;
			sourceStateArray[1] = true;
			sourceStateArray[8] = true;
			sourceStateArray[32] = true;
			sourceStateArray[33] = true;
			sourceStateArray[62] = true;
			sourceStateArray[63] = true;
			byte[] serializedBytes = new byte[8];
			{
				OutgoingMessage outgoingMessage = new OutgoingMessage(serializedBytes);
				sourceStateArray.Serialize(outgoingMessage);
			}
			StateArray destinationStateArray = new StateArray(64);
			using (MemoryStream memoryStream = new MemoryStream(serializedBytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					destinationStateArray.Deserialize(binaryReader);
				}
			}
			for (int i = 0; i < 64; i++)
			{
				if (i == 0 || i == 1 || i == 8 || i == 32 || i == 33 || i == 62 || i == 63)
				{
					Assert.IsTrue(destinationStateArray[i]);
				}
				else
				{
					Assert.IsFalse(destinationStateArray[i]);
				}
			}
		}

		public void Serialize2()
		{
			for (int i = 0; i < 160; i++)
			{
				StateArray sourceStateArray = new StateArray(64);
				sourceStateArray[i] = true;
				byte[] serializedBytes = new byte[8];
				{
					OutgoingMessage outgoingMessage = new OutgoingMessage(serializedBytes);
					sourceStateArray.Serialize(outgoingMessage);
				}
				StateArray destinationStateArray = new StateArray(64);
				using (MemoryStream memoryStream = new MemoryStream(serializedBytes))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						destinationStateArray.Deserialize(binaryReader);
						Assert.AreEqual(memoryStream.Length, memoryStream.Position);
					}
				}
				for (int k = 0; k < 160; k++)
				{
					if (k == i) { Assert.IsTrue(destinationStateArray[k]); }
					else { Assert.IsFalse(destinationStateArray[k]); }
				}
			}
		}

		#endregion Tests
	}
}
