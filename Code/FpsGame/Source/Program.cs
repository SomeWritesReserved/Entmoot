using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	public static class Program
	{
		#region Methods

		public static void Main()
		{
			GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

			using (FpsGame game = new FpsGame())
			{
				game.Run();
			}
		}

		#endregion Methods
	}
}
