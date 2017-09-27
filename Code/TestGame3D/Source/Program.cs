using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entmoot.TestGame3D
{
	public static class Program
	{
		#region Methods

		[STAThread]
		public static void Main()
		{
			using (MainGame game = new MainGame())
			{
				game.Run();
			}
		}

		#endregion Methods
	}
}
