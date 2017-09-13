using System;
using System.Collections.Generic;
using System.IO;
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
		public void FirstConnect()
		{
			MockClient client = NetworkClientTests.createTestCase0();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 5;
			NetworkClientTests.updateClientAndAssertState(client, 0, -1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 0, -1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 0, -1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 64, 64, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 65, 64, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 66, 64, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 67, 67, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 68, 67, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 69, 67, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 70, 70, true, 13.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 71, 70, true, 16.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 72, 70, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 73, 73, true, 23.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 74, 73, true, 26.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 75, 73, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 76, 76, true, 33.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 77, 76, true, 36.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 78, 76, true, 40.0f);
			NetworkClientTests.updateClientAndAssertState(client, 79, 76, true, 43.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 80, 76, true, 46.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 81, 76, true, 50.0f);
			NetworkClientTests.updateClientAndAssertState(client, 82, 76, true, 53.3333f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 83, 76, true, 56.6666f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 84, 76, true, 60.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 85, 76, true, 63.3333f, extrapolatedFrames: 4);
			NetworkClientTests.updateClientAndAssertState(client, 86, 76, true, 66.6666f, extrapolatedFrames: 5);
			NetworkClientTests.updateClientAndAssertState(client, 87, 76, true, 66.6666f, extrapolatedFrames: 5, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 88, 76, true, 66.6666f, extrapolatedFrames: 5, noInterpFrames: 2);
		}

		[Test]
		public void TestCase1_Interpolation()
		{
			MockClient client = NetworkClientTests.createTestCase1();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
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
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 13.3333f); // <- packets dropped after this
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
		public void TestCase1_NoInterpolation()
		{
			MockClient client = NetworkClientTests.createTestCase1();
			client.EngineClient.ShouldInterpolate = false;
			client.EngineClient.InterpolationRenderDelay = 8;
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
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 20.0f); // <- packets dropped after this
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

		[Test]
		public void TestCase2_Interpolation()
		{
			MockClient client = NetworkClientTests.createTestCase2();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
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
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 11.6666f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2_Prediction()
		{
			MockClient client = NetworkClientTests.createTestCase2();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.ShouldPredictInput = true;
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
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 13, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 13, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2B_Mispredict_Prediction()
		{
			MockClient client = NetworkClientTests.createTestCase2B();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.ShouldPredictInput = true;
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
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 13, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 13, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 25.0f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 30.0f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 30.0f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2C_Jitter_Interpolation()
		{
			MockClient client = NetworkClientTests.createTestCase2C();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			NetworkClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 4, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 13, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 13, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 15, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 19, true, 11.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2C_Jitter_Prediction()
		{
			MockClient client = NetworkClientTests.createTestCase2C();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.ShouldPredictInput = true;
			NetworkClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 4, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 10, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 16, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2D_OutOfOrder1_Interpolation()
		{
			MockClient client = NetworkClientTests.createTestCase2D();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			NetworkClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 13, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 15, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 11.6666f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2D_OutOfOrder1_Prediction()
		{
			MockClient client = NetworkClientTests.createTestCase2D();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.ShouldPredictInput = true;
			NetworkClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 10, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 10, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 10, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2E_OutOfOrder2_Interpolation()
		{
			MockClient client = NetworkClientTests.createTestCase2E();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			NetworkClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 13, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 15, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 10.8333f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 11.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 12.5f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 13.3333f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 14.1666f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2E_OutOfOrder2_Prediction()
		{
			MockClient client = NetworkClientTests.createTestCase2E();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.ShouldPredictInput = true;
			NetworkClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			NetworkClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 10, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 10, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 10, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase3_4DroppedPackets_Interpolation()
		{
			MockClient client = NetworkClientTests.createTestCase3();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
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
			NetworkClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 11.6666f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, 36, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 37, 37, true, 33.3333f, extrapolatedFrames: 3, noInterpFrames: 3); // <- getting packets again
			NetworkClientTests.updateClientAndAssertState(client, 38, 37, true, 31.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 39, 37, true, 30.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 40, 40, true, 28.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 41, 40, true, 26.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 42, 40, true, 25.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 43, 43, true, 23.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 44, 43, true, 21.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 45, 43, true, 20.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 46, 46, true, 18.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 47, 46, true, 16.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 48, 46, true, 15.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 49, 49, true, 13.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 50, 49, true, 11.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
		}

		[Test]
		public void TestCase3_4DroppedPackets_Prediction()
		{
			MockClient client = NetworkClientTests.createTestCase3();
			client.EngineClient.ShouldInterpolate = true;
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.ShouldPredictInput = true;
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
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 13, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 13, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			NetworkClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 28, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 29, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 30, 22, true, 25.0f);
			NetworkClientTests.updateClientAndAssertState(client, 31, 22, true, 25.0f, extrapolatedFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 32, 22, true, 25.0f, extrapolatedFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 33, 22, true, 20.0f, extrapolatedFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 34, 22, true, 20.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			NetworkClientTests.updateClientAndAssertState(client, 35, 22, true, 20.0f, extrapolatedFrames: 3, noInterpFrames: 2);
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 36, 22, true, 15.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 37, 37, true, 15.0f, extrapolatedFrames: 3, noInterpFrames: 3); // <- getting packets again
			NetworkClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 38, 37, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 39, 37, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 40, 40, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 41, 40, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 42, 40, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 43, 43, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 44, 43, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 45, 43, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 46, 46, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 47, 46, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 48, 46, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 49, 49, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			NetworkClientTests.updateClientAndAssertState(client, 50, 49, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
		}

		#endregion Tests

		#region Helpers

		private static void updateClientAndAssertState(MockClient mockClient, int clientFrameTick, int recievedServerFrameTick, bool hasInterpStarted, float? position, int extrapolatedFrames = 0, int noInterpFrames = 0)
		{
			NetworkClientTests.updateClientAndAssertState(mockClient, MockCommandKeys.None, clientFrameTick, recievedServerFrameTick, hasInterpStarted, position, extrapolatedFrames, noInterpFrames);
		}

		private static void updateClientAndAssertState(MockClient mockClient, MockCommandKeys keys, int clientFrameTick, int recievedServerFrameTick, bool hasInterpStarted, float? position, int extrapolatedFrames = 0, int noInterpFrames = 0)
		{
			mockClient.Update(keys);
			Client<MockCommandData> engineClient = mockClient.EngineClient;
			Assert.AreEqual(clientFrameTick, engineClient.FrameTick, "Unexpected FrameTick at tick " + mockClient.NetworkTick);
			Assert.AreEqual(recievedServerFrameTick, engineClient.LatestServerTickAcknowledgedByClient, "Unexpected LatestServerTickAcknowledgedByClient at tick " + mockClient.NetworkTick);
			Assert.AreEqual(hasInterpStarted, engineClient.HasInterpolationStarted, "Unexpected HasInterpolationStarted at tick " + mockClient.NetworkTick);
			Assert.AreEqual(hasInterpStarted, engineClient.InterpolationStartSnapshot != null, "Unexpected InterpolationStartSnapshot at tick " + mockClient.NetworkTick);
			Assert.AreEqual(hasInterpStarted, engineClient.InterpolationEndSnapshot != null, "Unexpected InterpolationEndSnapshot at tick " + mockClient.NetworkTick);
			Assert.AreEqual(extrapolatedFrames, engineClient.NumberOfExtrapolatedFrames, "Unexpected NumberOfExtrapolatedFrames at tick " + mockClient.NetworkTick);
			Assert.AreEqual(noInterpFrames, engineClient.NumberOfNoInterpolationFrames, "Unexpected NumberOfNoInterpolationFrames at tick " + mockClient.NetworkTick);
			Assert.AreEqual(position.HasValue, engineClient.RenderedSnapshot != null, "Unexpected RenderedSnapshot at tick " + mockClient.NetworkTick);
			if (position.HasValue)
			{
				if (engineClient.ShouldInterpolate)
				{
					Assert.AreEqual(engineClient.FrameTick - engineClient.InterpolationRenderDelay, engineClient.RenderedSnapshot.ServerFrameTick, "Unexpected RenderedSnapshot at tick " + mockClient.NetworkTick);
				}
				else
				{
					// Todo: should the client's render frame always be frametick-interpolationRenderDelay? Seems fragile to have the rendered frame tick report the same number for several frames (even though its accurate)
					Assert.AreEqual(engineClient.InterpolationEndSnapshot.ServerFrameTick, engineClient.RenderedSnapshot.ServerFrameTick, "Unexpected RenderedFrameTick at tick " + mockClient.NetworkTick);
				}
				Assert.IsTrue(engineClient.RenderedSnapshot.EntityArray.TryGetEntity(0, out Entity entity), "Expected RenderedState to have entity");
				Assert.IsTrue(entity.HasComponent<MockComponent>(), "Expected RenderedState to have component");
				Assert.AreEqual(position.Value, entity.GetComponent<MockComponent>().Position, 0.001f, "Unexpected RenderedState at tick " + mockClient.NetworkTick);
			}
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// simulating the initial connection of the client. No packet jitter. No acknowledgements from server.
		/// </summary>
		private static MockClient createTestCase0()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.MaxExtrapolationTicks = 5;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(4, 64, 10.0f);
			client.QueueIncomingStateUpdate(7, 67, 20.0f);
			client.QueueIncomingStateUpdate(10, 70, 30.0f);
			client.QueueIncomingStateUpdate(13, 73, 40.0f);
			client.QueueIncomingStateUpdate(16, 76, 50.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). No packet jitter. No acknowledgements from server.
		/// </summary>
		private static MockClient createTestCase1()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, 10.0f);
			client.QueueIncomingStateUpdate(4, 4, 10.0f);
			client.QueueIncomingStateUpdate(7, 7, 10.0f);
			client.QueueIncomingStateUpdate(10, 10, 20.0f);
			client.QueueIncomingStateUpdate(13, 13, 30.0f);
			client.QueueIncomingStateUpdate(16, 16, 35.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). No packet jitter. Mock server acknowledgements of client commands.
		/// </summary>
		private static MockClient createTestCase2()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, -1, 10.0f);
			client.QueueIncomingStateUpdate(4, 4, -1, 10.0f);
			client.QueueIncomingStateUpdate(7, 7, 3, 10.0f);
			client.QueueIncomingStateUpdate(10, 10, 6, 10.0f);
			client.QueueIncomingStateUpdate(13, 13, 9, 10.0f);
			client.QueueIncomingStateUpdate(16, 16, 12, 15.0f);
			client.QueueIncomingStateUpdate(19, 19, 15, 25.0f);
			client.QueueIncomingStateUpdate(22, 22, 18, 30.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). No packet jitter. Mock server acknowledgements of client commands.
		/// Client mis-predicts and corrected by server.
		/// </summary>
		private static MockClient createTestCase2B()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, -1, 10.0f);
			client.QueueIncomingStateUpdate(4, 4, -1, 10.0f);
			client.QueueIncomingStateUpdate(7, 7, 3, 10.0f);
			client.QueueIncomingStateUpdate(10, 10, 6, 10.0f);
			client.QueueIncomingStateUpdate(13, 13, 9, 10.0f);
			client.QueueIncomingStateUpdate(16, 16, 12, 15.0f);
			client.QueueIncomingStateUpdate(19, 19, 15, 20.0f);
			client.QueueIncomingStateUpdate(22, 22, 18, 25.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). Includes one-tick of packet jitter. Mock server acknowledgements of client commands.
		/// </summary>
		private static MockClient createTestCase2C()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, -1, 10.0f);
			client.QueueIncomingStateUpdate(5, 4, -1, 10.0f);
			client.QueueIncomingStateUpdate(7, 7, 3, 10.0f);
			client.QueueIncomingStateUpdate(9, 10, 6, 10.0f);
			client.QueueIncomingStateUpdate(14, 13, 9, 10.0f);
			client.QueueIncomingStateUpdate(15, 16, 12, 15.0f);
			client.QueueIncomingStateUpdate(18, 19, 15, 25.0f);
			client.QueueIncomingStateUpdate(23, 22, 18, 30.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). Includes two-tick of packet jitter and out of order packets but not
		/// out of order enoug to cause interpolation changes. Mock server acknowledgements of client commands.
		/// </summary>
		private static MockClient createTestCase2D()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, -1, 10.0f);
			client.QueueIncomingStateUpdate(4, 4, -1, 10.0f);
			client.QueueIncomingStateUpdate(8, 10, 6, 10.0f);
			client.QueueIncomingStateUpdate(9, 7, 3, 10.0f);
			client.QueueIncomingStateUpdate(16, 16, 12, 15.0f);
			client.QueueIncomingStateUpdate(19, 19, 15, 25.0f);
			client.QueueIncomingStateUpdate(19, 13, 9, 10.0f);
			client.QueueIncomingStateUpdate(22, 22, 18, 30.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). Includes two-tick of packet jitter and out of order packets, with
		/// being out of order enough to cause interpolation changes due to percieved dropped packet. Mock server acknowledgements of client commands.
		/// </summary>
		private static MockClient createTestCase2E()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, -1, 10.0f);
			client.QueueIncomingStateUpdate(4, 4, -1, 10.0f);
			client.QueueIncomingStateUpdate(8, 10, 6, 10.0f);
			client.QueueIncomingStateUpdate(9, 7, 3, 10.0f);
			client.QueueIncomingStateUpdate(16, 16, 12, 15.0f);
			client.QueueIncomingStateUpdate(19, 19, 15, 25.0f);
			client.QueueIncomingStateUpdate(20, 13, 9, 10.0f);
			client.QueueIncomingStateUpdate(22, 22, 18, 30.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). No packet jitter. Mock server acknowledgements of client commands.
		/// Dropped packets for 11 ticks.
		/// </summary>
		private static MockClient createTestCase3()
		{
			MockClient client = MockClient.CreateMockClient();
			client.EngineClient.InterpolationRenderDelay = 8;
			client.EngineClient.MaxExtrapolationTicks = 3;
			client.EngineClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, -1, 10.0f);
			client.QueueIncomingStateUpdate(4, 4, -1, 10.0f);
			client.QueueIncomingStateUpdate(7, 7, 3, 10.0f);
			client.QueueIncomingStateUpdate(10, 10, 6, 10.0f);
			client.QueueIncomingStateUpdate(13, 13, 9, 10.0f);
			client.QueueIncomingStateUpdate(16, 16, 12, 15.0f);
			client.QueueIncomingStateUpdate(19, 19, 15, 25.0f);
			client.QueueIncomingStateUpdate(22, 22, 18, 30.0f);
			client.QueueIncomingStateUpdate(37, 37, 33, 20.0f);
			client.QueueIncomingStateUpdate(40, 40, 36, 15.0f);
			client.QueueIncomingStateUpdate(43, 43, 39, 10.0f);
			client.QueueIncomingStateUpdate(46, 46, 42, 10.0f);
			client.QueueIncomingStateUpdate(49, 49, 45, 10.0f);
			return client;
		}

		#endregion Helpers

		#region Nested Types

		/// <summary>
		/// Represents a mock client that is "connected" to a server (data incoming from the server is mocked, outgoing data is not mocked).
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

			private readonly Dictionary<int, Queue<ServerEntityUpdate>> serverEntityUpdates = new Dictionary<int, Queue<ServerEntityUpdate>>();
			private EntitySnapshot serverEntitySnapshot;

			#endregion Fields

			#region Properties

			/// <summary>Gets the current tick of the network, independent of the underlying <see cref="Client"/>.</summary>
			public int NetworkTick { get; private set; }

			/// <summary>Gets the underlying <see cref="Client"/> object.</summary>
			public Client<MockCommandData> EngineClient { get; private set; }

			#endregion Properties

			#region Methods

			/// <summary>
			/// Returns a newly created <see cref="MockClient"/> object along with an underlying <see cref="Client"/>, all ready to go.
			/// </summary>
			public static MockClient CreateMockClient()
			{
				ComponentsDefinition componentsDefinition = new ComponentsDefinition();
				componentsDefinition.RegisterComponentType<MockComponent>();
				MockClient mockClient = new MockClient()
				{
					serverEntitySnapshot = new EntitySnapshot(10, componentsDefinition),
				};
				Assert.IsTrue(mockClient.serverEntitySnapshot.EntityArray.TryCreateEntity(out _));
				mockClient.serverEntitySnapshot.EntityArray.EndUpdate();
				Client<MockCommandData> engineClient = new Client<MockCommandData>(mockClient, 10, mockClient.serverEntitySnapshot.EntityArray.Capacity, componentsDefinition, new ISystem[0]);
				mockClient.EngineClient = engineClient;
				return mockClient;
			}

			/// <summary>
			/// Updates the state of the network and will also update the underlying <see cref="Client"/> object. It is important
			/// to call this <see cref="Update"/> rather than <see cref="Client.Update"/> otherwise the underlying <see cref="Client"/>
			/// will never get new packets (since the network never gets updated).
			/// </summary>
			public void Update(MockCommandKeys commandKeys)
			{
				this.NetworkTick++;
				this.EngineClient.Update(new MockCommandData() { CommandKeys = commandKeys });
			}

			/// <summary>
			/// Creates and adds a new <see cref="EntitySnapshot"/> to the network that will "arrive" for the client at a specified network tick
			/// (i.e. after that many calls to <see cref="Update"/> the given <see cref="EntitySnapshot"/> packet will "arrive" for the underlying
			/// <see cref="Client"/>).
			/// </summary>
			public void QueueIncomingStateUpdate(int networkTickToArriveOn, int serverFrameTick, float entityPosition)
			{
				// Create a new state snapshot and use its acked tick so we always use whatever the default should be
				this.QueueIncomingStateUpdate(networkTickToArriveOn, serverFrameTick, -1, entityPosition);
			}

			/// <summary>
			/// Creates and adds a new <see cref="EntitySnapshot"/> to the network that will "arrive" for the client at a specified network tick
			/// (i.e. after that many calls to <see cref="Update"/> the given <see cref="EntitySnapshot"/> packet will "arrive" for the underlying
			/// <see cref="Client"/>).
			/// </summary>
			public void QueueIncomingStateUpdate(int networkTickToArriveOn, int serverFrameTick, int acknowledgedClientFrameTick, float entityPosition)
			{
				if (!this.serverEntityUpdates.ContainsKey(networkTickToArriveOn))
				{
					this.serverEntityUpdates[networkTickToArriveOn] = new Queue<ServerEntityUpdate>();
				}
				this.serverEntityUpdates[networkTickToArriveOn].Enqueue(new ServerEntityUpdate()
				{
					ServerFrameTick = serverFrameTick,
					LatestClientTickAcknowledgedByServer = acknowledgedClientFrameTick,
					OwnedEntity = 0,
					NewPosition = entityPosition,
				});
			}

			byte[] INetworkConnection.GetNextIncomingPacket()
			{
				if (!this.serverEntityUpdates.ContainsKey(this.NetworkTick) || !this.serverEntityUpdates[this.NetworkTick].Any()) { return null; }
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						ServerEntityUpdate serverEntityUpdate = this.serverEntityUpdates[this.NetworkTick].Dequeue();
						this.serverEntitySnapshot.EntityArray.TryGetEntity(0, out Entity entity);
						entity.AddComponent<MockComponent>().Position = serverEntityUpdate.NewPosition;
						this.serverEntitySnapshot.UpdateFrom(serverEntityUpdate.ServerFrameTick, this.serverEntitySnapshot.EntityArray);

						binaryWriter.Write(serverEntityUpdate.LatestClientTickAcknowledgedByServer);
						binaryWriter.Write(serverEntityUpdate.OwnedEntity);
						this.serverEntitySnapshot.Serialize(binaryWriter);
						return memoryStream.ToArray();
					}
				}
			}

			void INetworkConnection.SendPacket(byte[] packet)
			{
				// Do nothing, the other endpoint doesn't exist so it won't respond to anything anyway
			}

			#endregion Methods

			#region Nested Types

			private class ServerEntityUpdate
			{
				public int ServerFrameTick;
				public int LatestClientTickAcknowledgedByServer;
				public int OwnedEntity;
				public float NewPosition;
			}

			#endregion Nested Types
		}

		private struct MockCommandData : ICommandData
		{
			#region Fields

			public MockCommandKeys CommandKeys;

			#endregion Fields

			#region Methods

			public void DeserializeData(BinaryReader binaryReader)
			{
				this.CommandKeys = (MockCommandKeys)binaryReader.ReadByte();
			}

			public void SerializeData(BinaryWriter binaryWriter)
			{
				binaryWriter.Write((byte)this.CommandKeys);
			}

			public void ApplyToEntity(Entity entity)
			{
				//if ((this.CommandKeys & MockCommandKeys.MoveForward) != 0) { entity.Position.Y -= 5; }
				//if ((this.CommandKeys & MockCommandKeys.MoveBackward) != 0) { entity.Position.Y += 5; }
				//if ((this.CommandKeys & MockCommandKeys.MoveLeft) != 0) { entity.Position.X -= 5; }
				//if ((this.CommandKeys & MockCommandKeys.MoveRight) != 0) { entity.Position.X += 5; }
			}

			#endregion Methods
		}

		private struct MockComponent : IComponent<MockComponent>
		{
			#region Fields

			public float Position;

			#endregion Fields

			#region Methods

			public void Serialize(BinaryWriter binaryWriter)
			{
				binaryWriter.Write(this.Position);
			}

			public void Deserialize(BinaryReader binaryReader)
			{
				this.Position = binaryReader.ReadSingle();
			}

			#endregion Methods
		}

		private enum MockCommandKeys : byte
		{
			None = 0,
			MoveForward = 1,
			MoveBackward = 2,
			MoveLeft = 4,
			MoveRight = 8,
			Shoot = 16,
		}

		#endregion Nested Types
	}
}
