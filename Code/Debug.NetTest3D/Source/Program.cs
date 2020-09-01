using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Debug.NetTest3D
{
	public static class Program
	{
		#region Methods

		[STAThread]
		public static void Main(string[] args)
		{
			GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

			using (MainGame game = new MainGame())
			{
				game.Run();
			}
		}

		#endregion Methods
	}
}
