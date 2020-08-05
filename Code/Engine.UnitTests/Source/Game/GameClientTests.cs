using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Entmoot.Engine.UnitTests
{
	[TestFixture]
	public class GameClientTests
	{
		#region Tests

		[Test]
		public void FirstConnect()
		{
			MockClient client = GameClientTests.createTestCase0();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 5;
			GameClientTests.updateClientAndAssertState(client, 0, -1, false, null);
			GameClientTests.updateClientAndAssertState(client, 0, -1, false, null);
			GameClientTests.updateClientAndAssertState(client, 0, -1, false, null);
			GameClientTests.updateClientAndAssertState(client, 64, 64, false, null);
			GameClientTests.updateClientAndAssertState(client, 65, 64, false, null);
			GameClientTests.updateClientAndAssertState(client, 66, 64, false, null);
			GameClientTests.updateClientAndAssertState(client, 67, 67, false, null);
			GameClientTests.updateClientAndAssertState(client, 68, 67, false, null);
			GameClientTests.updateClientAndAssertState(client, 69, 67, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 70, 70, true, 13.3333f);
			GameClientTests.updateClientAndAssertState(client, 71, 70, true, 16.6666f);
			GameClientTests.updateClientAndAssertState(client, 72, 70, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 73, 73, true, 23.3333f);
			GameClientTests.updateClientAndAssertState(client, 74, 73, true, 26.6666f);
			GameClientTests.updateClientAndAssertState(client, 75, 73, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 76, 76, true, 33.3333f);
			GameClientTests.updateClientAndAssertState(client, 77, 76, true, 36.6666f);
			GameClientTests.updateClientAndAssertState(client, 78, 76, true, 40.0f);
			GameClientTests.updateClientAndAssertState(client, 79, 76, true, 43.3333f);
			GameClientTests.updateClientAndAssertState(client, 80, 76, true, 46.6666f);
			GameClientTests.updateClientAndAssertState(client, 81, 76, true, 50.0f);
			GameClientTests.updateClientAndAssertState(client, 82, 76, true, 53.3333f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 83, 76, true, 56.6666f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 84, 76, true, 60.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 85, 76, true, 63.3333f, extrapolatedFrames: 4);
			GameClientTests.updateClientAndAssertState(client, 86, 76, true, 66.6666f, extrapolatedFrames: 5);
			GameClientTests.updateClientAndAssertState(client, 87, 76, true, 66.6666f, extrapolatedFrames: 5, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 88, 76, true, 66.6666f, extrapolatedFrames: 5, noInterpFrames: 2);
		}

		[Test]
		public void TestCase1_Interpolation()
		{
			MockClient client = GameClientTests.createTestCase1();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 13, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 15, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 16, 16, true, 13.3333f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 16.6666f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 16, true, 23.3333f);
			GameClientTests.updateClientAndAssertState(client, 20, 16, true, 26.6666f);
			GameClientTests.updateClientAndAssertState(client, 21, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 16, true, 31.6666f);
			GameClientTests.updateClientAndAssertState(client, 23, 16, true, 33.3333f);
			GameClientTests.updateClientAndAssertState(client, 24, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 16, true, 36.6666f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 26, 16, true, 38.3333f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 27, 16, true, 40.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 28, 16, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 29, 16, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 30, 16, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 3);
		}

		[Test]
		public void TestCase1_NoInterpolation()
		{
			MockClient client = GameClientTests.createTestCase1();
			client.GameClient.ShouldInterpolate = false;
			client.GameClient.InterpolationRenderDelay = 8;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 13, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 15, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 16, 16, true, 20.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 23, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 24, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 28, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 16, true, 35.0f);
		}

		[Test]
		public void TestCase2_Interpolation()
		{
			MockClient client = GameClientTests.createTestCase2();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 13, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 15, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 11.6666f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2_Prediction()
		{
			MockClient client = GameClientTests.createTestCase2();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.ShouldPredictInput = true;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 13, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 13, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2B_Mispredict_Prediction()
		{
			MockClient client = GameClientTests.createTestCase2B();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.ShouldPredictInput = true;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 13, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 13, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 25.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 30.0f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 30.0f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2B2_Mispredict_Prediction()
		{
			MockClient client = GameClientTests.createTestCase2B_2();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.ShouldPredictInput = true;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 12, 10, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 13, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 14, 13, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 13, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 20.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 25.0f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 25.0f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 30.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 30.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 30.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2C_Jitter_Interpolation()
		{
			MockClient client = GameClientTests.createTestCase2C();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 13, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 15, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 19, true, 11.6666f);
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2C_Jitter_Prediction()
		{
			MockClient client = GameClientTests.createTestCase2C();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.ShouldPredictInput = true;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 10, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 16, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2D_OutOfOrder1_Interpolation()
		{
			MockClient client = GameClientTests.createTestCase2D();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 13, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 15, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 11.6666f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2D_OutOfOrder1_Prediction()
		{
			MockClient client = GameClientTests.createTestCase2D();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.ShouldPredictInput = true;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 10, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 10, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 10, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2E_OutOfOrder2_Interpolation()
		{
			MockClient client = GameClientTests.createTestCase2E();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 13, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 15, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 10.8333f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 11.6666f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 12.5f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 13.3333f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 14.1666f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase2E_OutOfOrder2_Prediction()
		{
			MockClient client = GameClientTests.createTestCase2E();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.ShouldPredictInput = true;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 10, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 10, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 10, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 10, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 28, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 35.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 35.0f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 35.0f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 33, 22, true, 40.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 40.0f, extrapolatedFrames: 3, noInterpFrames: 2);
		}

		[Test]
		public void TestCase3_4DroppedPackets_Interpolation()
		{
			MockClient client = GameClientTests.createTestCase3();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 11, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 13, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 15, 13, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 16, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 11.6666f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 13.3333f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 18.3333f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 21.6666f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 28, 22, true, 26.6666f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 28.3333f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 31.6666f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 33.3333f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 33, 22, true, 35.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 2);
			GameClientTests.updateClientAndAssertState(client, 36, 22, true, 35.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 37, 37, true, 33.3333f, extrapolatedFrames: 3, noInterpFrames: 3); // <- getting packets again
			GameClientTests.updateClientAndAssertState(client, 38, 37, true, 31.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 39, 37, true, 30.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 40, 40, true, 28.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 41, 40, true, 26.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 42, 40, true, 25.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 43, 43, true, 23.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 44, 43, true, 21.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 45, 43, true, 20.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 46, 46, true, 18.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 47, 46, true, 16.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 48, 46, true, 15.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 49, 49, true, 13.3333f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 50, 49, true, 11.6666f, extrapolatedFrames: 3, noInterpFrames: 3);
		}

		[Test]
		public void TestCase3_4DroppedPackets_Prediction()
		{
			MockClient client = GameClientTests.createTestCase3();
			client.GameClient.ShouldInterpolate = true;
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.ShouldPredictInput = true;
			GameClientTests.updateClientAndAssertState(client, 1, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 2, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 3, 1, false, null);
			GameClientTests.updateClientAndAssertState(client, 4, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 5, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 6, 4, false, null);
			GameClientTests.updateClientAndAssertState(client, 7, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 8, 7, false, null);
			GameClientTests.updateClientAndAssertState(client, 9, 7, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, 10, 10, true, 10.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 11, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, 12, 10, true, 15.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 13, 13, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, 14, 13, true, 20.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 15, 13, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveRight, 16, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 17, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 18, 16, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 19, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 20, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 21, 19, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 22, 22, true, 30.0f); // <- packets dropped after this
			GameClientTests.updateClientAndAssertState(client, 23, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 24, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 25, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 26, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, 27, 22, true, 30.0f);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 28, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 29, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 30, 22, true, 25.0f);
			GameClientTests.updateClientAndAssertState(client, 31, 22, true, 25.0f, extrapolatedFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 32, 22, true, 25.0f, extrapolatedFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 33, 22, true, 20.0f, extrapolatedFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 34, 22, true, 20.0f, extrapolatedFrames: 3, noInterpFrames: 1);
			GameClientTests.updateClientAndAssertState(client, 35, 22, true, 20.0f, extrapolatedFrames: 3, noInterpFrames: 2);
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 36, 22, true, 15.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 37, 37, true, 15.0f, extrapolatedFrames: 3, noInterpFrames: 3); // <- getting packets again
			GameClientTests.updateClientAndAssertState(client, MockCommandKeys.MoveLeft, 38, 37, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 39, 37, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 40, 40, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 41, 40, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 42, 40, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 43, 43, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 44, 43, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 45, 43, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 46, 46, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 47, 46, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 48, 46, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 49, 49, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
			GameClientTests.updateClientAndAssertState(client, 50, 49, true, 10.0f, extrapolatedFrames: 3, noInterpFrames: 3);
		}

		#endregion Tests

		#region Helpers

		private static void updateClientAndAssertState(MockClient mockClient, int clientFrameTick, int recievedServerFrameTick, bool hasInterpStarted, float? position, int extrapolatedFrames = 0, int noInterpFrames = 0)
		{
			GameClientTests.updateClientAndAssertState(mockClient, MockCommandKeys.None, clientFrameTick, recievedServerFrameTick, hasInterpStarted, position, extrapolatedFrames, noInterpFrames);
		}

		private static void updateClientAndAssertState(MockClient mockClient, MockCommandKeys keys, int clientFrameTick, int recievedServerFrameTick, bool hasInterpStarted, float? position, int extrapolatedFrames = 0, int noInterpFrames = 0)
		{
			mockClient.Update(keys);
			GameClient<MockCommandData> gameClient = mockClient.GameClient;
			Assert.AreEqual(clientFrameTick, gameClient.FrameTick, "Unexpected FrameTick at tick " + mockClient.NetworkTick);
			Assert.AreEqual(recievedServerFrameTick, gameClient.LatestServerTickReceived, "Unexpected LatestServerTickAcknowledgedByClient at tick " + mockClient.NetworkTick);
			Assert.AreEqual(hasInterpStarted, gameClient.HasRenderingStarted, "Unexpected HasRenderingStarted at tick " + mockClient.NetworkTick);
			Assert.AreEqual(hasInterpStarted, gameClient.HasInterpolationStarted, "Unexpected HasInterpolationStarted at tick " + mockClient.NetworkTick);
			Assert.AreEqual(hasInterpStarted, gameClient.InterpolationStartSnapshot.HasData, "Unexpected InterpolationStartSnapshot at tick " + mockClient.NetworkTick);
			Assert.AreEqual(hasInterpStarted, gameClient.InterpolationEndSnapshot.HasData, "Unexpected InterpolationEndSnapshot at tick " + mockClient.NetworkTick);
			Assert.AreEqual(extrapolatedFrames, gameClient.NumberOfExtrapolatedFrames, "Unexpected NumberOfExtrapolatedFrames at tick " + mockClient.NetworkTick);
			Assert.AreEqual(noInterpFrames, gameClient.NumberOfNoInterpolationFrames, "Unexpected NumberOfNoInterpolationFrames at tick " + mockClient.NetworkTick);
			Assert.AreEqual(position.HasValue, gameClient.RenderedSnapshot.HasData, "Unexpected RenderedSnapshot at tick " + mockClient.NetworkTick);
			if (position.HasValue)
			{
				if (gameClient.ShouldInterpolate)
				{
					Assert.AreEqual(gameClient.FrameTick - gameClient.InterpolationRenderDelay, gameClient.RenderedSnapshot.ServerFrameTick, "Unexpected RenderedSnapshot at tick " + mockClient.NetworkTick);
				}
				else
				{
					Assert.AreEqual(gameClient.InterpolationEndSnapshot.ServerFrameTick, gameClient.RenderedSnapshot.ServerFrameTick, "Unexpected RenderedFrameTick at tick " + mockClient.NetworkTick);
				}
				Assert.IsTrue(gameClient.RenderedSnapshot.EntityArray.TryGetEntity(0, out Entity entity), "Expected RenderedState to have entity");
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
			client.GameClient.MaxExtrapolationTicks = 5;
			client.GameClient.ShouldPredictInput = false;
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
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
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
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
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
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
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
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). No packet jitter. Mock server acknowledgements of client commands.
		/// Client mis-predicts several times and corrected by server.
		/// </summary>
		private static MockClient createTestCase2B_2()
		{
			MockClient client = MockClient.CreateMockClient();
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
			client.QueueIncomingStateUpdate(1, 1, -1, 10.0f);
			client.QueueIncomingStateUpdate(4, 4, -1, 10.0f);
			client.QueueIncomingStateUpdate(7, 7, 3, 10.0f);
			client.QueueIncomingStateUpdate(10, 10, 6, 10.0f);
			client.QueueIncomingStateUpdate(13, 13, 9, 10.0f);
			client.QueueIncomingStateUpdate(16, 16, 12, 15.0f);
			client.QueueIncomingStateUpdate(19, 19, 15, 15.0f);
			client.QueueIncomingStateUpdate(22, 22, 18, 20.0f);
			return client;
		}

		/// <summary>
		/// Creates and returns a standard test case of incoming packets; simulates 2 tick latecy to server, 3 tick server network update rate,
		/// but doesn't simulate connecting (i.e. mock a mid-stream connection). Includes one-tick of packet jitter. Mock server acknowledgements of client commands.
		/// </summary>
		private static MockClient createTestCase2C()
		{
			MockClient client = MockClient.CreateMockClient();
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
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
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
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
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
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
			client.GameClient.InterpolationRenderDelay = 8;
			client.GameClient.MaxExtrapolationTicks = 3;
			client.GameClient.ShouldPredictInput = false;
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
		/// Use this instead of using a <see cref="GameClient"/> object directly (since this offers deterministic simulated packet arrival).
		/// Add mocked packets by calling <see cref="QueueIncomingStateUpdate"/> and they will "arrive" on the pre-determined tick you specify.
		/// </summary>
		/// <remarks>
		/// This wraps a <see cref="GameClient"/> object and you should call <see cref="Update"/> on this object rather than on the wrapped
		/// up <see cref="GameClient"/> object. This is because this class keeps track of its own ticks in order to simulate packet arrivals
		/// in a sane way (which isn't possible if we based off of <see cref="GameClient"/>'s ticks since it changes its tick as needed
		/// to sync with the server). With this class, the tick is always monotonically increasing by one every call to <see cref="Update"/>.
		/// This class also happens to be the <see cref="INetworkConnection"/> for the wrapped <see cref="GameClient"/> which makes a weird
		/// circular reference but this is the easiest way to control exactly when the packets come up independent of the <see cref="GameClient"/>'s
		/// tick. So even though this is a <see cref="INetworkConnection"/> it should be treated as a client (hence the name <see cref="MockClient"/>
		/// rather than something like MockedClientNetworkConnection).
		/// </remarks>
		private class MockClient : INetworkConnection
		{
			#region Fields

			private readonly Dictionary<int, Queue<MockServerUpdate>> mockServerUpdates = new Dictionary<int, Queue<MockServerUpdate>>();
			private EntitySnapshot serverEntitySnapshot;

			#endregion Fields

			#region Properties

			/// <summary>Gets the current tick of the network, independent of the underlying <see cref="GameClient"/>.</summary>
			public int NetworkTick { get; private set; }

			/// <summary>Gets the underlying GameClient object.</summary>
			public GameClient<MockCommandData> GameClient { get; private set; }

			/// <summary>Gets whether or not this network connection is actually connected to another endpoint.</summary>
			public bool IsConnected { get { return true; } }

			#endregion Properties

			#region Methods

			/// <summary>
			/// Returns a newly created <see cref="MockClient"/> object along with an underlying <see cref="GameClient"/>, all ready to go.
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
				GameClient<MockCommandData> gameClient = new GameClient<MockCommandData>(mockClient, 10, mockClient.serverEntitySnapshot.EntityArray.Capacity, componentsDefinition, new IClientSystem[0]);
				mockClient.GameClient = gameClient;
				return mockClient;
			}

			/// <summary>
			/// Updates the state of the network and will also update the underlying <see cref="GameClient"/> object. It is important
			/// to call this <see cref="Update"/> rather than <see cref="GameClient.Update"/> otherwise the underlying <see cref="GameClient"/>
			/// will never get new packets (since the network never gets updated).
			/// </summary>
			public void Update(MockCommandKeys commandKeys)
			{
				this.NetworkTick++;
				this.GameClient.Update(new MockCommandData() { CommandKeys = commandKeys });
			}

			/// <summary>
			/// Creates and adds a new <see cref="EntitySnapshot"/> to the network that will "arrive" for the client at a specified network tick
			/// (i.e. after that many calls to <see cref="Update"/> the given <see cref="EntitySnapshot"/> packet will "arrive" for the underlying
			/// <see cref="GameClient"/>).
			/// </summary>
			public void QueueIncomingStateUpdate(int networkTickToArriveOn, int serverFrameTick, float entityPosition)
			{
				// Create a new state snapshot and use its acked tick so we always use whatever the default should be
				this.QueueIncomingStateUpdate(networkTickToArriveOn, serverFrameTick, -1, entityPosition);
			}

			/// <summary>
			/// Creates and adds a new <see cref="EntitySnapshot"/> to the network that will "arrive" for the client at a specified network tick
			/// (i.e. after that many calls to <see cref="Update"/> the given <see cref="EntitySnapshot"/> packet will "arrive" for the underlying
			/// <see cref="GameClient"/>).
			/// </summary>
			public void QueueIncomingStateUpdate(int networkTickToArriveOn, int serverFrameTick, int acknowledgedClientFrameTick, float entityPosition)
			{
				if (!this.mockServerUpdates.ContainsKey(networkTickToArriveOn))
				{
					this.mockServerUpdates[networkTickToArriveOn] = new Queue<MockServerUpdate>();
				}
				this.mockServerUpdates[networkTickToArriveOn].Enqueue(new MockServerUpdate()
				{
					LatestClientTickReceived = acknowledgedClientFrameTick,
					CommandingEntityID = 0,
					ServerFrameTick = serverFrameTick,
					NewPosition = entityPosition,
				});
			}

			IncomingMessage INetworkConnection.GetNextIncomingMessage()
			{
				if (!this.mockServerUpdates.ContainsKey(this.NetworkTick) || !this.mockServerUpdates[this.NetworkTick].Any()) { return null; }

				MockServerUpdate mockServerUpdate = this.mockServerUpdates[this.NetworkTick].Dequeue();
				this.serverEntitySnapshot.EntityArray.TryGetEntity(0, out Entity entity);
				entity.AddComponent<MockComponent>().Position = mockServerUpdate.NewPosition;
				this.serverEntitySnapshot.Update(mockServerUpdate.ServerFrameTick, this.serverEntitySnapshot.EntityArray);

				OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[1024]);
				ServerUpdateSerializer.Serialize(outgoingMessage, null, this.serverEntitySnapshot, mockServerUpdate.LatestClientTickReceived, mockServerUpdate.CommandingEntityID);
				return new IncomingMessage(outgoingMessage.ToArray());
			}

			OutgoingMessage INetworkConnection.GetOutgoingMessageToSend()
			{
				return new OutgoingMessage(new byte[1024]);
			}

			void INetworkConnection.SendMessage(OutgoingMessage outgoingMessage)
			{
				// Do nothing, the other endpoint doesn't exist so it won't respond to anything anyway
			}

			#endregion Methods

			#region Nested Types

			private class MockServerUpdate
			{
				public int LatestClientTickReceived;
				public int CommandingEntityID;
				public int ServerFrameTick;
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

			public void Serialize(IWriter writer)
			{
				writer.Write((byte)this.CommandKeys);
			}

			public void Deserialize(IReader reader)
			{
				this.CommandKeys = (MockCommandKeys)reader.ReadByte();
			}

			public void ApplyToEntity(Entity entity)
			{
				if (!entity.HasComponent<MockComponent>()) { return; }

				ref MockComponent component = ref entity.GetComponent<MockComponent>();
				if ((this.CommandKeys & MockCommandKeys.MoveForward) != 0) { component.Position -= 5; }
				if ((this.CommandKeys & MockCommandKeys.MoveBackward) != 0) { component.Position += 5; }
				if ((this.CommandKeys & MockCommandKeys.MoveLeft) != 0) { component.Position -= 5; }
				if ((this.CommandKeys & MockCommandKeys.MoveRight) != 0) { component.Position += 5; }
			}

			#endregion Methods
		}

		private struct MockComponent : IComponent<MockComponent>
		{
			#region Fields

			public float Position;

			#endregion Fields

			#region Methods

			public bool Equals(MockComponent other)
			{
				return this.Position == other.Position;
			}

			public void Interpolate(MockComponent otherA, MockComponent otherB, float amount)
			{
				this.Position = otherA.Position + (otherB.Position - otherA.Position) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.Position);
			}

			public void Deserialize(IReader reader)
			{
				this.Position = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this = default(MockComponent);
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
