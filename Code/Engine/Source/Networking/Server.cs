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

		#region Constructors

		public Server()
		{
		}

		#endregion Constructors

		#region Methods

		public void Update(Entity[] entities)
		{
			StateSnapshot stateSnapshot = new StateSnapshot()
			{
				FrameTick = this.frameTick,
				Entities = entities,
			};
			foreach (ClientConnection client in this.clients)
			{
				client.SendStateSnapshot(stateSnapshot);
			}
			this.frameTick++;
		}

		#endregion Methods
	}

	public class ClientConnection
	{
		#region Methods

		public void SendStateSnapshot(StateSnapshot stateSnapshot)
		{
		}

		#endregion Methods
	}
}
