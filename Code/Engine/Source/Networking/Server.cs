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

		#endregion Fields

		#region Constructors

		public Server(IEnumerable<INetworkConnection> clientNetworkConnections, IEnumerable<Entity> entities)
		{
			this.clients = clientNetworkConnections.Select((netConn) => new ClientConnection(netConn)).ToList();
			this.entities = entities.ToList();
		}

		#endregion Constructors

		#region Properties

		private int frameTick = 0;
		public int FrameTick
		{
			get { return this.frameTick; }
		}

		private List<Entity> entities = new List<Entity>();
		public IList<Entity> Entities
		{
			get { return this.entities.AsReadOnly(); }
		}

		#endregion Properties

		#region Methods

		public void Update()
		{
			this.frameTick++;
			if (this.FrameTick % 3 == 0)
			{
				StateSnapshot stateSnapshot = new StateSnapshot()
				{
					FrameTick = this.frameTick,
					Entities = this.Entities.ToArray(),
				};
				foreach (ClientConnection client in this.clients)
				{
					client.SendStateSnapshot(stateSnapshot);
				}
			}
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
