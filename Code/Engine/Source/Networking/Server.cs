using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine.Server
{
	public class Server
	{
		#region Fields

		private List<ClientConnection> clients = new List<ClientConnection>();
		private int frameTick;

		#endregion Fields

		#region Methods

		public void Update()
		{
			this.frameTick++;
		}

		#endregion Methods
	}

	public class ClientConnection
	{
	}
}
