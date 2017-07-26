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

		[Test]
		public void Client_FirstConnect()
		{
		}

		[Test]
		public void Client_RenderStateInterpolation()
		{
			Client client = NetworkClientTests.createDefaultTestCase();
			client.ShouldInterpolate = true;
			NetworkClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 13, 13, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 13, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 15, 13, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 13.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 16.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 16, true, 23.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 16, true, 26.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 16, true, 31.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 23, 16, true, 33.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 16, true, 36.6666f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 26, 16, true, 38.3333f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 27, 16, true, 40.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 28, 16, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 29, 16, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 30, 16, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 3);
		}

		#endregion Tests

		#region Helpers

		private static void updateClientAndAssertState(Client client, int clientFrameTick, int recievedServerFrameTick, bool hasInterpStarted, float? position, int extrapolatedFrames = 0, int noInterpFrames = 0)
		{
			client.Update(CommandKeys.None);
			Assert.AreEqual(clientFrameTick, client.FrameTick, "FrameTick_" + clientFrameTick);
			Assert.AreEqual(recievedServerFrameTick, client.LatestReceivedServerTick, "LatestReceivedServerTick_" + clientFrameTick);
			Assert.AreEqual(hasInterpStarted, client.HasInterpolationStarted, "HasInterpolationStarted_" + clientFrameTick);
			Assert.AreEqual(hasInterpStarted, client.InterpolationStartState != null, "InterpolationStartState_" + clientFrameTick);
			Assert.AreEqual(hasInterpStarted, client.InterpolationEndState != null, "InterpolationEndState_" + clientFrameTick);
			Assert.AreEqual(extrapolatedFrames, client.NumberOfExtrapolatedFrames, "NumberOfExtrapolatedFrames_" + clientFrameTick);
			Assert.AreEqual(noInterpFrames, client.NumberOfNoInterpolationFrames, "NumberOfNoInterpolationFrames_" + clientFrameTick);
			Assert.AreEqual(position.HasValue, client.RenderedState != null, "RenderedState_" + clientFrameTick);
			if (position.HasValue)
			{
				Assert.AreEqual(position.Value, client.RenderedState.Entities[0].Position.X, 0.001f, "RenderedState_" + clientFrameTick);
			}
		}

		private static Client createDefaultTestCase()
		{
			MockServerNetworkConnection networkConnection = new MockServerNetworkConnection();
			networkConnection.SimulateTick(1, new Vector3(10, 0, 0));
			networkConnection.SimulateTick(4, new Vector3(10, 0, 0));
			networkConnection.SimulateTick(7, new Vector3(10, 0, 0));
			networkConnection.SimulateTick(10, new Vector3(20, 0, 0));
			networkConnection.SimulateTick(13, new Vector3(30, 0, 0));
			networkConnection.SimulateTick(16, new Vector3(35, 0, 0));
			Client client = new Client(networkConnection);
			client.InterpolationRenderDelay = 8;
			client.MaxExtrapolationTicks = 3;
			client.ShouldPredictInput = false;
			networkConnection.OwnerClient = client;
			return client;
		}

		#endregion Helpers

		#region Nested Types

		private class MockServerNetworkConnection : INetworkConnection
		{
			#region Fields

			private Dictionary<int, Queue<StateSnapshot>> queuedStateSnapshots = new Dictionary<int, Queue<StateSnapshot>>();

			#endregion Fields

			#region Properties

			public Client OwnerClient { get; set; }

			#endregion Properties

			#region Methods

			public void SimulateTick(int arrivalFrameTick, Vector3 entityPosition)
			{
				if (!this.queuedStateSnapshots.ContainsKey(arrivalFrameTick))
				{
					this.queuedStateSnapshots[arrivalFrameTick] = new Queue<StateSnapshot>();
				}
				StateSnapshot stateSnapshot = new StateSnapshot()
				{
					ServerFrameTick = arrivalFrameTick,
					Entities = new Entity[] { new Entity() { Position = entityPosition } },
				};
				this.queuedStateSnapshots[arrivalFrameTick].Enqueue(stateSnapshot);
			}

			public byte[] GetNextIncomingPacket()
			{
				int clientFrameTick = this.OwnerClient.FrameTick;
				if (!this.queuedStateSnapshots.ContainsKey(clientFrameTick) || !this.queuedStateSnapshots[clientFrameTick].Any()) { return null; }
				return this.queuedStateSnapshots[clientFrameTick].Dequeue().SerializePacket();
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
