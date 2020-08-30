using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Entmoot.Game.Sideswipe
{
	public static class Program
	{
		#region Methods

		public static void Main(string[] args)
		{
			IPAddress ipAddressToConnectTo = null;
			int ipAddressArgIndex = Array.IndexOf(args, "-ip");
			if (ipAddressArgIndex >= 0 && ipAddressArgIndex <= args.Length - 2)
			{
				ipAddressToConnectTo = IPAddress.Parse(args[ipAddressArgIndex + 1]);
			}

			using (SideswipeGame game = new SideswipeGame(ipAddressToConnectTo))
			{
				game.Run();
			}
		}

		#endregion Methods
	}
}
