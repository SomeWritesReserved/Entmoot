﻿using System;
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
			MockClient client = NetworkClientTests.createTestCase1();
			client.EngineClient.ShouldInterpolate = true;
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

		[Test]
		public void Client_RenderStateNoInterpolation()
		{
			MockClient client = NetworkClientTests.createTestCase1();
			client.EngineClient.ShouldInterpolate = false;
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
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 23, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 28, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 16, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 16, true, 35.0f);
		}

		#endregion Tests

		#region Helpers

		private static void updateClientAndAssertState(MockClient mockClient, int clientFrameTick, int recievedServerFrameTick, bool hasInterpStarted, float? position, int extrapolatedFrames = 0, int noInterpFrames = 0)
		{
			mockClient.Update(CommandKeys.None);
			Client engineClient = mockClient.EngineClient;
			Assert.AreEqual(clientFrameTick, engineClient.FrameTick, "FrameTick_" + clientFrameTick);
			Assert.AreEqual(recievedServerFrameTick, engineClient.LatestReceivedServerTick, "LatestReceivedServerTick_" + clientFrameTick);
			Assert.AreEqual(hasInterpStarted, engineClient.HasInterpolationStarted, "HasInterpolationStarted_" + clientFrameTick);
			Assert.AreEqual(hasInterpStarted, engineClient.InterpolationStartState != null, "InterpolationStartState_" + clientFrameTick);
			Assert.AreEqual(hasInterpStarted, engineClient.InterpolationEndState != null, "InterpolationEndState_" + clientFrameTick);
			Assert.AreEqual(extrapolatedFrames, engineClient.NumberOfExtrapolatedFrames, "NumberOfExtrapolatedFrames_" + clientFrameTick);
			Assert.AreEqual(noInterpFrames, engineClient.NumberOfNoInterpolationFrames, "NumberOfNoInterpolationFrames_" + clientFrameTick);
			Assert.AreEqual(position.HasValue, engineClient.RenderedState != null, "RenderedState_" + clientFrameTick);
			if (position.HasValue)
			{
				Assert.AreEqual(position.Value, engineClient.RenderedState.Entities[0].Position.X, 0.001f, "RenderedState_" + clientFrameTick);
			}
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server , 3 tick server network update rate, .
		/// </summary>
		private static MockClient createTestCase1()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, new Vector3(10, 0, 0));
			client.QueueIncomingStateUpdate(4, 4, new Vector3(10, 0, 0));
			client.QueueIncomingStateUpdate(7, 7, new Vector3(10, 0, 0));
			client.QueueIncomingStateUpdate(10, 10, new Vector3(20, 0, 0));
			client.QueueIncomingStateUpdate(13, 13, new Vector3(30, 0, 0));
			client.QueueIncomingStateUpdate(16, 16, new Vector3(35, 0, 0));
			return client;
		}

		#endregion Helpers

		#region Nested Types

		/// <summary>
		/// Represents a mock client that is "connected" to a server (data incoming from the server is, outgoing data is not mocked).
		/// Use this instead of using a <see cref="Client"/> object directly (since this offers deterministic simulated packet arrival).
		/// Add mocked packets by calling <see cref="QueueIncomingStateUpdate"/> and they will "arrive" on the pre-determined tick you specify.
		/// </summary>
		/// <remarks>
		/// This wraps a <see cref="Client"/> object and you should call <see cref="Update"/> on this object rather than on the wrapped
		/// up <see cref="Client"/> object. This is because this class keeps track of its own ticks in order to simulate packet arrivals
		/// in a sane way (which isn't possible if we based off of <see cref="Client"/>'s ticks since it changes its tick as needed
		/// to sync with the server). With this class, the tick is always monotonically increasing by one every call to <see cref="Update"/>.
		/// This class also happens to be the <see cref="INetworkConnection"/> for the wrapped <see cref="Client"/> which makes a weird
		/// circular reference but this is the easiest way to control exactly when the packets come up independent of the <see cref="Client"/>'s
		/// tick. So even though this is a <see cref="INetworkConnection"/> it should be treated as a client (hence the name <see cref="MockClient"/>
		/// rather than something like MockedClientNetworkConnection).
		/// </remarks>
		private class MockClient : INetworkConnection
		{
			#region Fields

			private Dictionary<int, Queue<StateSnapshot>> queuedStateSnapshots = new Dictionary<int, Queue<StateSnapshot>>();

			#endregion Fields

			#region Properties

			/// <summary>Gets the current tick of the network, independent of the underlying <see cref="Client"/>.</summary>
			public int NetworkTick { get; private set; }

			/// <summary>Gets the underlying <see cref="Client"/> object.</summary>
			public Client EngineClient { get; private set; }

			#endregion Properties

			#region Methods

			/// <summary>
			/// Returns a newly created <see cref="MockClient"/> object along with an underlying <see cref="Client"/>, all ready to go.
			/// </summary>
			public static MockClient CreateMockClient()
			{
				MockClient mockClient = new MockClient();
				Client engineClient = new Client(mockClient);
				mockClient.EngineClient = engineClient;
				return mockClient;
			}

			/// <summary>
			/// Updates the state of the network and will also update the underlying <see cref="Client"/> object. It is important
			/// to call this <see cref="Update"/> rather than <see cref="Client.Update"/> otherwise the underlying <see cref="Client"/>
			/// will never get new packets (since the network never gets updated).
			/// </summary>
			public void Update(CommandKeys activeCommandKeys)
			{
				this.NetworkTick++;
				this.EngineClient.Update(activeCommandKeys);
			}

			/// <summary>
			/// Creates and adds a new <see cref="StateSnapshot"/> to the network that will "arrive" for the client at a specified network tick
			/// (i.e. after that many calls to <see cref="Update"/> the given <see cref="StateSnapshot"/> packet will "arrive" for the underlying
			/// <see cref="Client"/>).
			/// </summary>
			public void QueueIncomingStateUpdate(int arrivalNetworkTick, int serverFrameTick, Vector3 entityPosition)
			{
				if (!this.queuedStateSnapshots.ContainsKey(arrivalNetworkTick))
				{
					this.queuedStateSnapshots[arrivalNetworkTick] = new Queue<StateSnapshot>();
				}
				StateSnapshot stateSnapshot = new StateSnapshot()
				{
					ServerFrameTick = serverFrameTick,
					Entities = new Entity[] { new Entity() { Position = entityPosition } },
				};
				this.queuedStateSnapshots[arrivalNetworkTick].Enqueue(stateSnapshot);
			}

			byte[] INetworkConnection.GetNextIncomingPacket()
			{
				if (!this.queuedStateSnapshots.ContainsKey(this.NetworkTick) || !this.queuedStateSnapshots[this.NetworkTick].Any()) { return null; }
				return this.queuedStateSnapshots[this.NetworkTick].Dequeue().SerializePacket();
			}

			void INetworkConnection.SendPacket(byte[] packet)
			{
				// Do nothing, the other endpoint doesn't exist so it won't respond to anything anyway
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
