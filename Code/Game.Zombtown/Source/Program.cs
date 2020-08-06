using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entmoot.Game.Zombtown
{
	public static class Program
	{
		#region Methods

		public static void Main()
		{
			using (ZombtownXnaGame game = new ZombtownXnaGame())
			{
				game.Run();
			}
		}

		#endregion Methods
	}
}
