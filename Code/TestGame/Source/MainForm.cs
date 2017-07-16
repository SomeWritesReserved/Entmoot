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

		private int serverStepsRemaining = 0;
		private int clientStepsRemaining = 0;

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

			this.clientServerNetworkConnection = new MockNetworkConnection()
			{
				SimulatedLatency = 4,
				SimulatedJitter = 0,
				SimulatedPacketLoss = 0,
			};
			this.client = new Client(this.clientServerNetworkConnection);
			this.server = new Server(new[] { this.clientServerNetworkConnection }, this.serverEntities);

			this.clientServerNetworkConnection.Client = this.client;
			this.clientServerNetworkConnection.Server = this.server;

			this.clientGroupBox.Tag = ClientServerContext.Client;
			this.serverGroupBox.Tag = ClientServerContext.Server;

			this.clientPacketTimelineDisplay.NetworkConnection = this.clientServerNetworkConnection;
			this.clientPacketTimelineDisplay.ClientServerContext = ClientServerContext.Client;
		}

		#endregion Constructors

		#region Events

		private void serverTimer_Tick(object sender, EventArgs e)
		{
			if (this.serverStepsRemaining == 0) { return; }

			this.clientServerNetworkConnection.CurrentContext = ClientServerContext.Server;

			this.serverEntities[0].Position.X = (float)Math.Cos(this.server.FrameTick * 0.065) * 50 + 100;
			this.serverEntities[0].Position.Y = (float)Math.Sin(this.server.FrameTick * 0.065) * 50 + 100;

			this.server.Update();
			this.serverGroupBox.Refresh();
			this.clientPacketTimelineDisplay.Refresh();
			this.serverStepsRemaining--;
		}

		private void clientTimer_Tick(object sender, EventArgs e)
		{
			if (this.clientStepsRemaining == 0) { return; }

			this.clientServerNetworkConnection.CurrentContext = ClientServerContext.Client;
			this.client.Update();
			this.clientGroupBox.Refresh();
			this.clientPacketTimelineDisplay.Refresh();
			this.clientStepsRemaining--;
		}

		private void runPauseServerButton_Click(object sender, EventArgs e)
		{
			if (this.serverStepsRemaining == 0)
			{
				this.serverStepsRemaining = -1;
			}
			else
			{
				this.serverStepsRemaining = 0;
			}
		}

		private void serverStepButton_Click(object sender, EventArgs e)
		{
			this.serverStepsRemaining = (int)this.serverStepNumberPad.Value;
		}

		private void runPauseClientButton_Click(object sender, EventArgs e)
		{
			if (this.clientStepsRemaining == 0)
			{
				this.clientStepsRemaining = -1;
			}
			else
			{
				this.clientStepsRemaining = 0;
			}
		}

		private void runPauseBothButton_Click(object sender, EventArgs e)
		{
			if (this.serverStepsRemaining == 0)
			{
				this.serverStepsRemaining = -1;
			}
			else
			{
				this.serverStepsRemaining = 0;
			}
			if (this.clientStepsRemaining == 0)
			{
				this.clientStepsRemaining = -1;
			}
			else
			{
				this.clientStepsRemaining = 0;
			}
		}

		private void clientStepButton_Click(object sender, EventArgs e)
		{
			this.clientStepsRemaining = (int)this.clientStepNumberPad.Value;
		}

		private void gameGroupBox_Paint(object sender, PaintEventArgs e)
		{
			ClientServerContext clientServerContext = (ClientServerContext)((Control)sender).Tag;
			int now = (clientServerContext == ClientServerContext.Client) ? this.client.FrameTick : this.server.FrameTick;
			IList<Entity> entities = (clientServerContext == ClientServerContext.Client) ? this.client.Entities : this.server.Entities;

			e.Graphics.DrawString(now.ToString(), this.Font, Brushes.Black, 10, 10);
			if (this.drawInterpolationCheckBox.Checked && clientServerContext == ClientServerContext.Client &&
				this.client.InterpolatedStartTick >= 0 && this.client.InterpolatedEndTick >= 0)
			{
				Brush color = this.client.IsInterpolationValid ? Brushes.Gainsboro : Brushes.LightCoral;
				var interpolationStart = this.client.ReceivedStateSnapshots[this.client.InterpolatedStartTick];
				var interpolationEnd = this.client.ReceivedStateSnapshots[this.client.InterpolatedEndTick];
				foreach (Entity entity in interpolationStart.Entities)
				{
					e.Graphics.FillRectangle(color, entity.Position.X, entity.Position.Y, 3, 3);
				}
				foreach (Entity entity in interpolationEnd.Entities)
				{
					e.Graphics.FillRectangle(color, entity.Position.X, entity.Position.Y, 3, 3);
				}
			}
			foreach (Entity entity in entities)
			{
				e.Graphics.FillRectangle(Brushes.Black, entity.Position.X, entity.Position.Y, 3, 3);
			}
		}

		private void drawInterpolationCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.clientGroupBox.Refresh();
		}

		private void dropPacketsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.clientServerNetworkConnection.DropAllPackets = this.dropPacketsCheckBox.Checked;
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

		private Random random = new Random(12345);
		public List<SentPacket> IncomingPacketsForClient = new List<SentPacket>();
		public List<SentPacket> IncomingPacketsForServer = new List<SentPacket>();
		public List<SentPacket> OldPacketsForClient = new List<SentPacket>();
		public List<SentPacket> OldPacketsForServer = new List<SentPacket>();

		#endregion Fields

		#region Properties

		public Client Client { get; set; }

		public Server Server { get; set; }

		public ClientServerContext CurrentContext { get; set; }

		/// <summary>Gets or sets the amount of time it takes to send a packet from endpoint to endpoint, measured in game ticks (not milliseconds).</summary>
		public double SimulatedLatency { get; set; }

		/// <summary>Gets or sets the amount of random variance in the simulated latency, measured in game ticks (not milliseconds).</summary>
		public double SimulatedJitter { get; set; }

		/// <summary>Gets or sets the percent chance that a packet may be dropped, from [0, 1].</summary>
		public double SimulatedPacketLoss { get; set; }

		/// <summary>Gets or sets whether all simulated packets should be dropped.</summary>
		public bool DropAllPackets { get; set; }

		#endregion Properties

		#region Methods

		public byte[] GetNextIncomingPacket()
		{
			return (this.CurrentContext == ClientServerContext.Client) ? this.getArrivedPacket(this.IncomingPacketsForClient, this.OldPacketsForClient) : this.getArrivedPacket(this.IncomingPacketsForServer, this.OldPacketsForServer);
		}

		public void SendPacket(byte[] packet)
		{
			if (random.NextDouble() < this.SimulatedPacketLoss || this.DropAllPackets) { return; }

			// Sent the packet to the other endpoint based on the current context (if client, send to server; vice versa).
			// Use the current context's tick (instead of the other end point's) because we know we are running and the other
			// endpoint might be paused. We still want to simulate the other endpoint getting our packets in the future. This
			// only works because in this test case client and server start at the same time so both ticks start at zero at the
			// same wall time.
			if (this.CurrentContext == ClientServerContext.Client)
			{
				int arrivalTick = (int)(this.Client.FrameTick + this.SimulatedLatency + (this.random.NextDouble() - this.random.NextDouble()) * this.SimulatedJitter);
				SentPacket sentPacket = new SentPacket() { ArrivalTick = arrivalTick, Data = packet };
				this.IncomingPacketsForServer.Add(sentPacket);
			}
			else
			{
				int arrivalTick = (int)(this.Server.FrameTick + this.SimulatedLatency + (this.random.NextDouble() - this.random.NextDouble()) * this.SimulatedJitter);
				SentPacket sentPacket = new SentPacket() { ArrivalTick = arrivalTick, Data = packet };
				this.IncomingPacketsForClient.Add(sentPacket);
			}
		}

		private byte[] getArrivedPacket(List<SentPacket> incomingPackets, List<SentPacket> oldPackets)
		{
			int now = (this.CurrentContext == ClientServerContext.Client) ? this.Client.FrameTick : this.Server.FrameTick;
			SentPacket packet = incomingPackets.FirstOrDefault((p) => p.ArrivalTick <= now);
			if (packet != null)
			{
				incomingPackets.Remove(packet);
				oldPackets.Add(packet);
			}
			return packet?.Data;
		}

		#endregion Methods

		#region Nested Types

		public class SentPacket
		{
			#region Properties

			public int ArrivalTick { get; set; }

			public byte[] Data { get; set; }

			#endregion Properties
		}

		#endregion Nested Types
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

	public class PacketTimelineDisplay : Label
	{
		#region Fields

		private Pen tickLinePen;

		#endregion Fields

		#region Constructors

		public PacketTimelineDisplay()
		{
			this.DoubleBuffered = true;
			this.tickLinePen = new Pen(Color.Gainsboro)
			{
				DashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
			};
		}

		#endregion Constructors

		#region Properties

		public MockNetworkConnection NetworkConnection { get; set; }

		public ClientServerContext ClientServerContext { get; set; }

		#endregion Properties

		#region Methods

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.NetworkConnection == null) { return; }

			float centerX = this.Width / 2.0f;
			float centerY = this.Height / 2.0f;
			int now = (this.ClientServerContext == ClientServerContext.Client) ? this.NetworkConnection.Client.FrameTick : this.NetworkConnection.Server.FrameTick;
			var incomingPackets = (this.ClientServerContext == ClientServerContext.Client) ? this.NetworkConnection.IncomingPacketsForClient : this.NetworkConnection.IncomingPacketsForServer;
			var oldPackets = (this.ClientServerContext == ClientServerContext.Client) ? this.NetworkConnection.OldPacketsForClient : this.NetworkConnection.OldPacketsForServer;

			Func<int, float> timeToX = (time) => time * 12.0f;

			// Shade the "past" side of the timeline display
			e.Graphics.FillRectangle(Brushes.WhiteSmoke, 0, 0, this.Width / 2.0f, this.Height);
			e.Graphics.DrawString(this.NetworkConnection.Client.NumberOfInvalidInterpolations.ToString(), this.Font, Brushes.Black, 0, 0);

			// Make the current tick always centered in the display
			e.Graphics.TranslateTransform(-timeToX(now) + centerX, 0);
			int numberOfTicksToDraw = this.Width / 12;
			foreach (var tick in Enumerable.Range(now - numberOfTicksToDraw / 2, numberOfTicksToDraw))
			{
				if ((Math.Abs(now - tick) % 5) == 0)
				{
					e.Graphics.DrawLine(Pens.LightGray, timeToX(tick), 0, timeToX(tick), this.Height - 17);
					e.Graphics.DrawString(tick.ToString(), this.Font, Brushes.LightGray, timeToX(tick) - 10, this.Height - 16);
				}
				else
				{
					e.Graphics.DrawLine(this.tickLinePen, timeToX(tick), 0, timeToX(tick), this.Height);
				}
			}
			foreach (var incomingPacket in incomingPackets)
			{
				var packetTick = StateSnapshot.DeserializePacket(incomingPacket.Data).FrameTick;
				e.Graphics.DrawLine(Pens.Black, timeToX(incomingPacket.ArrivalTick) - 4, centerY - 4, timeToX(incomingPacket.ArrivalTick), centerY);
				e.Graphics.DrawLine(Pens.Black, timeToX(incomingPacket.ArrivalTick), centerY, timeToX(incomingPacket.ArrivalTick) + 4, centerY - 4);
				e.Graphics.DrawString(packetTick.ToString(), this.Font, Brushes.Black, timeToX(incomingPacket.ArrivalTick) - 6, centerY - 20);
			}
			foreach (var incomingPacket in oldPackets)
			{
				var packetTick = StateSnapshot.DeserializePacket(incomingPacket.Data).FrameTick;
				e.Graphics.DrawLine(Pens.Gray, timeToX(incomingPacket.ArrivalTick) - 4, centerY - 4, timeToX(incomingPacket.ArrivalTick), centerY);
				e.Graphics.DrawLine(Pens.Gray, timeToX(incomingPacket.ArrivalTick), centerY, timeToX(incomingPacket.ArrivalTick) + 4, centerY - 4);
				e.Graphics.DrawString(packetTick.ToString(), this.Font, Brushes.Gray, timeToX(incomingPacket.ArrivalTick) - 6, centerY - 20);
			}
			if (this.ClientServerContext == ClientServerContext.Client)
			{
				foreach (var kvp in this.NetworkConnection.Client.ReceivedStateSnapshots)
				{
					Pen snapshotPen = Pens.Black;
					Brush snapshotBrush = Brushes.Black;
					if (this.NetworkConnection.Client.IsInterpolationValid && (this.NetworkConnection.Client.InterpolatedStartTick == kvp.Key ||
						this.NetworkConnection.Client.InterpolatedEndTick == kvp.Key))
					{
						snapshotPen = Pens.Red;
						snapshotBrush = Brushes.Red;
					}
					StateSnapshot receivedStateSnapshot = kvp.Value;
					e.Graphics.DrawLine(snapshotPen, timeToX(receivedStateSnapshot.FrameTick) - 4, centerY + 4, timeToX(receivedStateSnapshot.FrameTick), centerY);
					e.Graphics.DrawLine(snapshotPen, timeToX(receivedStateSnapshot.FrameTick), centerY, timeToX(receivedStateSnapshot.FrameTick) + 4, centerY + 4);
					e.Graphics.DrawString(receivedStateSnapshot.FrameTick.ToString(), this.Font, snapshotBrush, timeToX(receivedStateSnapshot.FrameTick) - 6, centerY + 6);
				}

				Pen pen = (this.NetworkConnection.Client.IsInterpolationValid) ? Pens.Red : Pens.Green;
				e.Graphics.DrawLine(pen, timeToX(this.NetworkConnection.Client.RenderFrameTick), 0, timeToX(this.NetworkConnection.Client.RenderFrameTick), this.Height);
			}
			e.Graphics.DrawLine(Pens.Blue, timeToX(now), 0, timeToX(now), this.Height);
		}

		#endregion Methods
	}
}
