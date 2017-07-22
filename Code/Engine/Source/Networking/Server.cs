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

		private readonly List<ClientConnection> clients = new List<ClientConnection>();
		private readonly Entity[] entities;

		#endregion Fields

		#region Constructors

		public Server(IEnumerable<INetworkConnection> clientNetworkConnections, IEnumerable<Entity> entities)
		{
			this.clients = clientNetworkConnections.Select((netConn) => new ClientConnection(netConn)).ToList();
			this.entities = entities.ToArray();
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets the current frame tick of the server.</summary>
		public int FrameTick { get; private set; }

		/// <summary>Gets the <see cref="StateSnapshot"/> that is currently the most up-to-date state.</summary>
		public StateSnapshot CurrentState { get; private set; }

		#endregion Properties

		#region Methods

		public void Update()
		{
			this.FrameTick++;

			this.CurrentState = new StateSnapshot()
			{
				ServerFrameTick = this.FrameTick,
				Entities = this.entities,
			};

			if (this.FrameTick % 3 == 0)
			{
				foreach (ClientConnection client in this.clients)
				{
					client.SendStateSnapshot(this.CurrentState);
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
