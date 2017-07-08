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

		private int frameTick;
		private List<ClientConnection> clients = new List<ClientConnection>();

		#endregion Fields

		#region Constructors

		public Server(IEnumerable<INetworkConnection> clientNetworkConnections)
		{
			this.clients = clientNetworkConnections.Select((netConn) => new ClientConnection(netConn)).ToList();
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
		#region Fields

		private INetworkConnection clientNetworkConnection;

		#endregion Fields

		#region Constructors

		public ClientConnection(INetworkConnection clientCetworkConnection)
		{
			this.clientNetworkConnection = clientCetworkConnection;
		}

		#endregion Constructors

		#region Methods

		public void SendStateSnapshot(StateSnapshot stateSnapshot)
		{
			byte[] packet = stateSnapshot.SerializePacket();
			this.clientNetworkConnection.SendPacket(packet);
		}

		#endregion Methods
	}
}
