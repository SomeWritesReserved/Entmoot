using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public static class LogStats
	{
		#region Fields

		#region Client

		public static float Client_LatencyToServer = 0;
		public static int Client_NumOutOfOrderPackets = 0;
		public static int Client_LatestReceivedServerPacket = 0;

		#endregion Client

		#region Server

		public static float[] Server_LatencyToClients = new float[64];

		#endregion Server

		#endregion Fields
	}
}
