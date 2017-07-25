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
	public class NetworkClientTests
	{
		#region Tests

		#endregion Tests

		#region Helpers

		private static Client createDefaultTestCase()
		{
			MockNetworkConnection networkConnection = new MockNetworkConnection();
			Client client = new Client(networkConnection);
			networkConnection.OwnerClient = client;
			return client;
		}

		#endregion Helpers

		#region Nested Types

		private class MockNetworkConnection : INetworkConnection
		{
			#region Fields

			public Client OwnerClient;
			private Dictionary<int, Queue<StateSnapshot>> queuedStateSnapshots = new Dictionary<int, Queue<StateSnapshot>>();

			#endregion Fields

			#region Methods

			public void QueueStateSnapshot(int arrivalFrameTick, StateSnapshot stateSnapshot)
			{
				if (!this.queuedStateSnapshots.ContainsKey(arrivalFrameTick))
				{
					this.queuedStateSnapshots[arrivalFrameTick] = new Queue<StateSnapshot>();
				}
				this.queuedStateSnapshots[arrivalFrameTick].Enqueue(stateSnapshot);
			}

			public byte[] GetNextIncomingPacket()
			{
				int frameTick = this.OwnerClient.FrameTick;
				if (!this.queuedStateSnapshots.ContainsKey(frameTick) || !this.queuedStateSnapshots[frameTick].Any()) { return null; }
				return this.queuedStateSnapshots[frameTick].Dequeue().SerializePacket();
			}

			public void SendPacket(byte[] packet)
			{
				// Do nothing, the other endpoint doesn't exist so it won't respond to anything anyway
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
