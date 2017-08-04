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
using System.Windows.Input;
using Entmoot.Engine;

namespace Entmoot.TestGame
{
	public partial class MainForm : Form
	{
		#region Fields

		private Client client;
		private Server server;
		private TestNetworkConnection clientServerNetworkConnection;
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
				new Entity() { Position = new Vector3(100, 80, 0), },
				new Entity() { Position = new Vector3(50, 50, 0), },
			};

			this.clientServerNetworkConnection = new TestNetworkConnection()
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

			//this.serverEntities[1].Position.X = (float)Math.Cos(this.server.FrameTick * 0.065) * 50 + 100;
			//this.serverEntities[1].Position.Y = (float)Math.Sin(this.server.FrameTick * 0.065) * 50 + 100;

			this.clientServerNetworkConnection.UpdateServer();
			this.serverGroupBox.Refresh();
			this.clientPacketTimelineDisplay.Refresh();
			this.serverStepsRemaining--;
		}

		private void clientTimer_Tick(object sender, EventArgs e)
		{
			if (this.clientStepsRemaining == 0) { return; }

			CommandKeys currentCommandKeys = CommandKeys.None;
			if (Keyboard.IsKeyDown(Key.W)) { currentCommandKeys |= CommandKeys.MoveForward; }
			if (Keyboard.IsKeyDown(Key.S)) { currentCommandKeys |= CommandKeys.MoveBackward; }
			if (Keyboard.IsKeyDown(Key.A)) { currentCommandKeys |= CommandKeys.MoveLeft; }
			if (Keyboard.IsKeyDown(Key.D)) { currentCommandKeys |= CommandKeys.MoveRight; }
			if (Keyboard.IsKeyDown(Key.D1)) { currentCommandKeys |= CommandKeys.Seat1; }
			else if (Keyboard.IsKeyDown(Key.D2)) { currentCommandKeys |= CommandKeys.Seat2; }

			this.clientServerNetworkConnection.CurrentContext = ClientServerContext.Client;
			this.clientServerNetworkConnection.UpdateClient(currentCommandKeys);
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
			IList<Entity> entities = (clientServerContext == ClientServerContext.Client) ? this.client.RenderedState?.Entities : this.server.CurrentState?.Entities;

			e.Graphics.DrawString(now.ToString(), this.Font, Brushes.Black, 10, 10);
			if (this.drawInterpolationCheckBox.Checked && clientServerContext == ClientServerContext.Client &&
				this.client.InterpolationStartState != null && this.client.InterpolationEndState != null)
			{
				foreach (Entity entity in this.client.InterpolationStartState.Entities)
				{
					e.Graphics.FillRectangle(Brushes.Gainsboro, entity.Position.X, entity.Position.Y, 3, 3);
				}
				foreach (Entity entity in this.client.InterpolationEndState.Entities)
				{
					e.Graphics.FillRectangle(Brushes.Gainsboro, entity.Position.X, entity.Position.Y, 3, 3);
				}
			}
			if (entities != null)
			{
				foreach (Entity entity in entities)
				{
					e.Graphics.FillRectangle(Brushes.Black, entity.Position.X, entity.Position.Y, 3, 3);
				}
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

	public class TestNetworkConnection : INetworkConnection
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

		/// <summary>Gets the network tick that the server is currently reading from (which can be different from the server's current frame tick).</summary>
		public int NetworkServerTick { get; private set; }

		/// <summary>Gets the network tick that the client is currently reading from (which can be different from the client's current frame tick and render tick).</summary>
		public int NetworkClientTick { get; private set; }

		#endregion Properties

		#region Methods

		public void UpdateServer()
		{
			this.NetworkServerTick++;
			this.Server.Update();
		}

		public void UpdateClient(CommandKeys commandKeys)
		{
			this.NetworkClientTick++;
			this.Client.Update(commandKeys);
		}

		public byte[] GetNextIncomingPacket()
		{
			return (this.CurrentContext == ClientServerContext.Client) ? this.getArrivedPacket(this.IncomingPacketsForClient, this.OldPacketsForClient) : this.getArrivedPacket(this.IncomingPacketsForServer, this.OldPacketsForServer);
		}

		public void SendPacket(byte[] packet)
		{
			if (random.NextDouble() < this.SimulatedPacketLoss || this.DropAllPackets) { return; }

			if (this.CurrentContext == ClientServerContext.Client)
			{
				int arrivalNetworkTick = (int)(this.NetworkServerTick + this.SimulatedLatency + (this.random.NextDouble() - this.random.NextDouble()) * this.SimulatedJitter);
				SentPacket sentPacket = new SentPacket() { ArrivalNetworkTick = arrivalNetworkTick, Data = packet };
				this.IncomingPacketsForServer.Add(sentPacket);
			}
			else
			{
				int arrivalNetworkTick = (int)(this.NetworkClientTick + this.SimulatedLatency + (this.random.NextDouble() - this.random.NextDouble()) * this.SimulatedJitter);
				SentPacket sentPacket = new SentPacket() { ArrivalNetworkTick = arrivalNetworkTick, Data = packet };
				this.IncomingPacketsForClient.Add(sentPacket);
			}
		}

		private byte[] getArrivedPacket(List<SentPacket> incomingPackets, List<SentPacket> oldPackets)
		{
			int nowNetworkTick = (this.CurrentContext == ClientServerContext.Client) ? this.NetworkClientTick : this.NetworkServerTick;
			SentPacket packet = incomingPackets.FirstOrDefault((p) => p.ArrivalNetworkTick <= nowNetworkTick);
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

			public int ArrivalNetworkTick { get; set; }

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

		public TestNetworkConnection NetworkConnection { get; set; }

		public ClientServerContext ClientServerContext { get; set; }

		#endregion Properties

		#region Methods

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.NetworkConnection == null) { return; }

			float centerX = this.Width / 2.0f;
			float centerY = this.Height / 2.0f;
			int nowTick = (this.ClientServerContext == ClientServerContext.Client) ? this.NetworkConnection.Client.FrameTick : this.NetworkConnection.Server.FrameTick;
			int nowNetworkTick = (this.ClientServerContext == ClientServerContext.Client) ? this.NetworkConnection.NetworkClientTick : this.NetworkConnection.NetworkServerTick;
			var incomingPackets = (this.ClientServerContext == ClientServerContext.Client) ? this.NetworkConnection.IncomingPacketsForClient : this.NetworkConnection.IncomingPacketsForServer;
			var oldPackets = (this.ClientServerContext == ClientServerContext.Client) ? this.NetworkConnection.OldPacketsForClient : this.NetworkConnection.OldPacketsForServer;

			Func<int, float> engineTickToX = (time) => time * 12.0f;
			Func<int, float> networkTickToX = (time) => (time - (nowNetworkTick - nowTick)) * 12.0f;

			// Shade the "past" side of the timeline display
			e.Graphics.FillRectangle(Brushes.WhiteSmoke, 0, 0, this.Width / 2.0f, this.Height);
			e.Graphics.DrawString(string.Format("{0} {1}", this.NetworkConnection.Client.NumberOfExtrapolatedFrames, this.NetworkConnection.Client.NumberOfNoInterpolationFrames), this.Font, Brushes.Black, 0, 0);

			// Make the current tick always centered in the display
			e.Graphics.TranslateTransform(-engineTickToX(nowTick) + centerX, 0);
			int numberOfTicksToDraw = this.Width / 12;
			foreach (var tick in Enumerable.Range(nowTick - numberOfTicksToDraw / 2, numberOfTicksToDraw))
			{
				if ((Math.Abs(nowTick - tick) % 5) == 0)
				{
					e.Graphics.DrawLine(Pens.LightGray, engineTickToX(tick), 0, engineTickToX(tick), this.Height - 17);
					e.Graphics.DrawString(tick.ToString(), this.Font, Brushes.LightGray, engineTickToX(tick) - 10, this.Height - 16);
				}
				else
				{
					e.Graphics.DrawLine(this.tickLinePen, engineTickToX(tick), 0, engineTickToX(tick), this.Height);
				}
			}
			foreach (var incomingPacket in incomingPackets)
			{
				var packetTick = StateSnapshot.DeserializePacket(incomingPacket.Data).ServerFrameTick;
				e.Graphics.DrawLine(Pens.Black, networkTickToX(incomingPacket.ArrivalNetworkTick) - 4, centerY - 4, networkTickToX(incomingPacket.ArrivalNetworkTick), centerY);
				e.Graphics.DrawLine(Pens.Black, networkTickToX(incomingPacket.ArrivalNetworkTick), centerY, networkTickToX(incomingPacket.ArrivalNetworkTick) + 4, centerY - 4);
				e.Graphics.DrawString(packetTick.ToString(), this.Font, Brushes.Black, networkTickToX(incomingPacket.ArrivalNetworkTick) - 6, centerY - 20);
			}
			foreach (var incomingPacket in oldPackets)
			{
				var packetTick = StateSnapshot.DeserializePacket(incomingPacket.Data).ServerFrameTick;
				e.Graphics.DrawLine(Pens.Gray, networkTickToX(incomingPacket.ArrivalNetworkTick) - 4, centerY - 4, networkTickToX(incomingPacket.ArrivalNetworkTick), centerY);
				e.Graphics.DrawLine(Pens.Gray, networkTickToX(incomingPacket.ArrivalNetworkTick), centerY, networkTickToX(incomingPacket.ArrivalNetworkTick) + 4, centerY - 4);
				e.Graphics.DrawString(packetTick.ToString(), this.Font, Brushes.Gray, networkTickToX(incomingPacket.ArrivalNetworkTick) - 6, centerY - 20);
			}
			if (this.ClientServerContext == ClientServerContext.Client)
			{
				foreach (var kvp in this.NetworkConnection.Client.ReceivedStateSnapshots)
				{
					Pen snapshotPen = Pens.Black;
					Brush snapshotBrush = Brushes.Black;
					if (this.NetworkConnection.Client.HasInterpolationStarted &&
						(this.NetworkConnection.Client.InterpolationStartState.ServerFrameTick == kvp.Key || this.NetworkConnection.Client.InterpolationEndState.ServerFrameTick == kvp.Key))
					{
						snapshotPen = Pens.Red;
						snapshotBrush = Brushes.Red;
					}
					StateSnapshot receivedStateSnapshot = kvp.Value;
					e.Graphics.DrawLine(snapshotPen, engineTickToX(receivedStateSnapshot.ServerFrameTick) - 4, centerY + 4, engineTickToX(receivedStateSnapshot.ServerFrameTick), centerY);
					e.Graphics.DrawLine(snapshotPen, engineTickToX(receivedStateSnapshot.ServerFrameTick), centerY, engineTickToX(receivedStateSnapshot.ServerFrameTick) + 4, centerY + 4);
					e.Graphics.DrawString(receivedStateSnapshot.ServerFrameTick.ToString(), this.Font, snapshotBrush, engineTickToX(receivedStateSnapshot.ServerFrameTick) - 6, centerY + 6);
				}

				if (this.NetworkConnection.Client.RenderedState != null)
				{
					e.Graphics.DrawLine(Pens.Red, engineTickToX(this.NetworkConnection.Client.RenderedState.ServerFrameTick), 0, engineTickToX(this.NetworkConnection.Client.RenderedState.ServerFrameTick), this.Height);
				}
			}
			e.Graphics.DrawLine(Pens.Blue, engineTickToX(nowTick), 0, engineTickToX(nowTick), this.Height);
		}

		#endregion Methods
	}
}
