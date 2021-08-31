using System;
using System.Net;

namespace Entmoot.Game.Fps
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

			using (FpsGame game = new FpsGame(ipAddressToConnectTo))
			{
				game.Run();
			}
		}

		#endregion Methods
	}
}
