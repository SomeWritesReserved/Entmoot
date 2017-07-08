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
				new Entity() { Position = new Vector3(50, 50, 0), },
				new Entity() { Position = new Vector3(60, 80, 0), },
			};

			this.clientServerNetworkConnection = new MockNetworkConnection();
			this.client = new Client(this.clientServerNetworkConnection);
			this.server = new Server(new[] { this.clientServerNetworkConnection }, this.serverEntities);

			this.clientGroupBox.Tag = this.client;
			this.serverGroupBox.Tag = this.server;
		}

		#endregion Constructors

		#region Events

		private void serverTimer_Tick(object sender, EventArgs e)
		{
			this.clientServerNetworkConnection.CurrentContext = ClientServerContext.Server;

			this.serverEntities[0].Position.X = (float)Math.Cos(this.server.FrameTick * 0.025) * 50 + 100;
			this.serverEntities[0].Position.Y = (float)Math.Sin(this.server.FrameTick * 0.025) * 50 + 100;

			this.server.Update();
			this.serverGroupBox.Refresh();
		}

		private void clientTimer_Tick(object sender, EventArgs e)
		{
			this.clientServerNetworkConnection.CurrentContext = ClientServerContext.Client;
			this.client.Update();
			this.clientGroupBox.Refresh();
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

		private void gameGroupBox_Paint(object sender, PaintEventArgs e)
		{
			dynamic entityContainer = ((Control)sender).Tag;
			IList<Entity> entities = entityContainer.Entities;
			foreach (Entity entity in entities)
			{
				e.Graphics.FillRectangle(Brushes.Black, entity.Position.X, entity.Position.Y, 3, 3);
			}
		}
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

	public class DoubleBufferedGroupBox : GroupBox
	{
		#region Constructors

		public DoubleBufferedGroupBox()
		{
			this.DoubleBuffered = true;
		}

		#endregion Constructors
	}
}
