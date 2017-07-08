using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entmoot.Engine;
using Entmoot.Engine.Client;
using Entmoot.Engine.Server;

namespace Entmoot.TestGame
{
	public partial class MainForm : Form
	{
		#region Fields

		private Client client;
		private Server server;
		private MockNetworkConnection clientServerNetworkConnection;
		private Entity[] serverEntities;

		#endregion Fields

		#region Constructors

		public MainForm()
		{
			this.InitializeComponent();

			this.serverEntities = new Entity[]
			{
				new Entity() { Position = new Vector3(10, 10, 0), },
				new Entity() { Position = new Vector3(20, 40, 0), },
			};

			this.clientServerNetworkConnection = new MockNetworkConnection();
			this.client = new Client(this.clientServerNetworkConnection);
			this.server = new Server(new[] { this.clientServerNetworkConnection });
		}

		#endregion Constructors

		#region Events

		private void serverTimer_Tick(object sender, EventArgs e)
		{
			this.clientServerNetworkConnection.CurrentContext = ClientServerContext.Server;
			this.server.Update(this.serverEntities);
		}

		private void clientTimer_Tick(object sender, EventArgs e)
		{
			this.clientServerNetworkConnection.CurrentContext = ClientServerContext.Client;
			this.client.Update();
		}

		#endregion Events

		#region Methods

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.serverTimer.Start();
			Thread.Sleep(50);
			this.clientTimer.Start();
		}

		#endregion Methods
	}

	public class MockNetworkConnection : INetworkConnection
	{
		#region Fields

		private Queue<byte[]> incomingPacketsForClient = new Queue<byte[]>();
		private Queue<byte[]> incomingPacketsForServer = new Queue<byte[]>();

		#endregion Fields

		#region Properties

		public ClientServerContext CurrentContext { get; set; }

		public bool HasIncomingPackets
		{
			get { return (this.CurrentContext == ClientServerContext.Client) ? this.incomingPacketsForClient.Any() : this.incomingPacketsForServer.Any(); }
		}

		#endregion Properties

		#region Methods

		public byte[] GetNextIncomingPacket()
		{
			return (this.CurrentContext == ClientServerContext.Client) ? this.incomingPacketsForClient.Dequeue() : this.incomingPacketsForServer.Dequeue();
		}

		public void SendPacket(byte[] packet)
		{
			if (this.CurrentContext == ClientServerContext.Client)
			{
				this.incomingPacketsForServer.Enqueue(packet);
			}
			else
			{
				this.incomingPacketsForClient.Enqueue(packet);
			}
		}

		#endregion Methods
	}

	public enum ClientServerContext
	{
		Client,
		Server,
	}
}
