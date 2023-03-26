using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Entmoot.Engine;

namespace Entmoot.Debug.NetTestWinForms
{
	public partial class MainForm : Form
	{
		#region Fields

		private GameClient<TestCommandData> gameClient;
		private GameServer<TestCommandData> gameServer;
		private TestNetworkConnection clientServerNetworkConnection;

		private int serverStepsRemaining = 0;
		private int clientStepsRemaining = 0;

		#endregion Fields

		#region Constructors

		public MainForm()
		{
			this.InitializeComponent();

			this.clientServerNetworkConnection = new TestNetworkConnection()
			{
				SimulatedLatency = 10,
				SimulatedJitter = 0,
				SimulatedPacketLoss = 0,
			};

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<PositionComponent>();
			componentsDefinition.RegisterComponentType<SpinComponent>();

			this.gameClient = new GameClient<TestCommandData>(this.clientServerNetworkConnection, 10, 5,
				componentsDefinition, new IClientSystem[] { new MovementSystem() });
			this.gameServer = new GameServer<TestCommandData>(new[] { this.clientServerNetworkConnection }, 10, 5,
				componentsDefinition, new IServerSystem[] { new SpinSystem(), new MovementSystem() }, this.updateCommandingEntityID);
			{
				this.gameServer.EntityArray.TryCreateEntity(out Entity entity1);
				this.gameServer.EntityArray.TryCreateEntity(out Entity entity2);
				entity2.AddComponent<PositionComponent>().Position = new Vector3(0, 0, 0);
				entity2.AddComponent<SpinComponent>();
				this.gameServer.EntityArray.TryCreateEntity(out Entity entity3);
				this.gameServer.EntityArray.TryCreateEntity(out Entity entity4);
				this.gameServer.EntityArray.RemoveEntity(entity1);
				this.gameServer.EntityArray.RemoveEntity(entity3);
				entity4.AddComponent<PositionComponent>().Position = new Vector3(99, 99, 0);
			}

			this.clientServerNetworkConnection.GameClient = this.gameClient;
			this.clientServerNetworkConnection.GameServer = this.gameServer;

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
			this.clientServerNetworkConnection.UpdateServer();
			this.serverGroupBox.Refresh();
			this.clientPacketTimelineDisplay.Refresh();
			this.serverStepsRemaining--;
		}

		private void clientTimer_Tick(object sender, EventArgs e)
		{
			if (this.clientStepsRemaining == 0) { return; }

			TestCommandKeys currentCommandKeys = TestCommandKeys.None;
			if (Keyboard.IsKeyDown(Key.W)) { currentCommandKeys |= TestCommandKeys.MoveForward; }
			if (Keyboard.IsKeyDown(Key.S)) { currentCommandKeys |= TestCommandKeys.MoveBackward; }
			if (Keyboard.IsKeyDown(Key.A)) { currentCommandKeys |= TestCommandKeys.MoveLeft; }
			if (Keyboard.IsKeyDown(Key.D)) { currentCommandKeys |= TestCommandKeys.MoveRight; }
			if (Keyboard.IsKeyDown(Key.D1)) { currentCommandKeys |= TestCommandKeys.Seat1; }
			else if (Keyboard.IsKeyDown(Key.D2)) { currentCommandKeys |= TestCommandKeys.Seat2; }

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
			int now = (clientServerContext == ClientServerContext.Client) ? this.gameClient.FrameTick : this.gameServer.FrameTick;
			EntityArray entityArray = (clientServerContext == ClientServerContext.Client) ? this.gameClient.RenderedSnapshot.EntityArray : this.gameServer.EntityArray;

			e.Graphics.DrawString(now.ToString(), this.Font, Brushes.Black, 10, 10);
			if (this.drawInterpolationCheckBox.Checked && clientServerContext == ClientServerContext.Client &&
				this.gameClient.InterpolationStartSnapshot.HasData && this.gameClient.InterpolationEndSnapshot.HasData)
			{
				foreach (Entity entity in this.gameClient.InterpolationStartSnapshot.EntityArray)
				{
					if (!entity.HasComponent<PositionComponent>()) { continue; }

					ref PositionComponent component = ref entity.GetComponent<PositionComponent>();
					e.Graphics.FillRectangle(Brushes.Gainsboro, component.Position.X, component.Position.Y, 3, 3);
				}
				foreach (Entity entity in this.gameClient.InterpolationEndSnapshot.EntityArray)
				{
					ref PositionComponent component = ref entity.GetComponent<PositionComponent>();
					e.Graphics.FillRectangle(Brushes.Gainsboro, component.Position.X, component.Position.Y, 3, 3);
				}
			}
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<PositionComponent>()) { continue; }

				ref PositionComponent component = ref entity.GetComponent<PositionComponent>();
				e.Graphics.FillRectangle(Brushes.Black, component.Position.X, component.Position.Y, 3, 3);
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

		/// <summary>
		/// Handles updating the commanding entity ID of a client, either giving the client a new entity
		/// when the client first connects or removing the commanding entity if the client disconnects.
		/// </summary>
		private int updateCommandingEntityID(bool isClientConnected, int currentCommandingEntityID)
		{
			if (isClientConnected && currentCommandingEntityID == -1)
			{
				if (this.gameServer.EntityArray.TryCreateEntity(out Entity clientEntity))
				{
					clientEntity.AddComponent<PositionComponent>().Position = new Vector3(100, 50, 0);
					return clientEntity.ID;
				}
			}
			else if (!isClientConnected && currentCommandingEntityID != -1)
			{
				this.gameServer.EntityArray.RemoveEntity(currentCommandingEntityID);
				return -1;
			}
			return currentCommandingEntityID;
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

		public GameClient<TestCommandData> GameClient { get; set; }

		public GameServer<TestCommandData> GameServer { get; set; }

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

		/// <summary>Gets whether or not this network connection is actually connected to another endpoint.</summary>
		public bool IsConnected { get { return true; } }

		#endregion Properties

		#region Methods

		public void UpdateServer()
		{
			this.NetworkServerTick++;
			this.GameServer.Update();
		}

		public void UpdateClient(TestCommandKeys commandKeys)
		{
			this.NetworkClientTick++;
			this.GameClient.Update(new TestCommandData() { CommandKeys = commandKeys });
		}

		public IncomingMessage GetNextIncomingMessage()
		{
			byte[] data = this.GetNextIncomingPacket();
			if (data == null) { return null; }
			IncomingMessage incomingMessage = new IncomingMessage(data);
			incomingMessage.Length = data.Length;
			return incomingMessage;
		}

		public byte[] GetNextIncomingPacket()
		{
			return (this.CurrentContext == ClientServerContext.Client) ? this.getArrivedPacket(this.IncomingPacketsForClient, this.OldPacketsForClient) : this.getArrivedPacket(this.IncomingPacketsForServer, this.OldPacketsForServer);
		}

		public OutgoingMessage GetOutgoingMessageToSend()
		{
			return new OutgoingMessage(new byte[1024]);
		}

		public void SendMessage(OutgoingMessage outgoingMessage)
		{
			if (random.NextDouble() < this.SimulatedPacketLoss || this.DropAllPackets) { return; }

			if (this.CurrentContext == ClientServerContext.Client)
			{
				int arrivalNetworkTick = (int)(this.NetworkServerTick + this.SimulatedLatency + (this.random.NextDouble() - this.random.NextDouble()) * this.SimulatedJitter);
				SentPacket sentPacket = new SentPacket() { ArrivalNetworkTick = arrivalNetworkTick, Data = outgoingMessage.ToArray() };
				this.IncomingPacketsForServer.Add(sentPacket);
			}
			else
			{
				int arrivalNetworkTick = (int)(this.NetworkClientTick + this.SimulatedLatency + (this.random.NextDouble() - this.random.NextDouble()) * this.SimulatedJitter);
				SentPacket sentPacket = new SentPacket() { ArrivalNetworkTick = arrivalNetworkTick, Data = outgoingMessage.ToArray() };
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
			this.tickLinePen = new Pen(System.Drawing.Color.Gainsboro)
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

			/*float centerX = this.Width / 2.0f;
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
			e.Graphics.DrawLine(Pens.Blue, engineTickToX(nowTick), 0, engineTickToX(nowTick), this.Height);*/
		}

		#endregion Methods
	}

	public struct PositionComponent : IComponent<PositionComponent>
	{
		#region Fields

		public Vector3 Position;
		public Vector3 Velocity;

		#endregion Fields

		#region Methods

		public bool Equals(PositionComponent other)
		{
			return this.Position == other.Position &&
				this.Velocity == other.Velocity;
		}

		public void Interpolate(PositionComponent otherA, PositionComponent otherB, float amount)
		{
			this.Position = Vector3.Lerp(otherA.Position, otherB.Position, amount);
			this.Velocity = Vector3.Lerp(otherA.Velocity, otherB.Velocity, amount);
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			writer.Write(this.Position.Z);
			writer.Write(this.Velocity.X);
			writer.Write(this.Velocity.Y);
			writer.Write(this.Velocity.Z);
		}

		public void Deserialize(IReader reader)
		{
			this.Position.X = reader.ReadSingle();
			this.Position.Y = reader.ReadSingle();
			this.Position.Z = reader.ReadSingle();
			this.Velocity.X = reader.ReadSingle();
			this.Velocity.Y = reader.ReadSingle();
			this.Velocity.Z = reader.ReadSingle();
		}

		public void ResetToDefaults()
		{
			this = default(PositionComponent);
		}

		#endregion Methods
	}

	public struct SpinComponent : IComponent<SpinComponent>
	{
		#region Methods

		public bool Equals(SpinComponent other)
		{
			return true;
		}

		public void Interpolate(SpinComponent otherA, SpinComponent otherB, float amount)
		{
		}

		public void Serialize(IWriter writer)
		{
		}

		public void Deserialize(IReader reader)
		{
		}

		public void ResetToDefaults()
		{
		}

		#endregion Methods
	}

	public class SpinSystem : IServerSystem
	{
		#region Properties

		public int Tick { get; set; }

		#endregion Properties

		#region Methods

		public void ServerUpdate(EntityArray entityArray)
		{
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<PositionComponent>()) { continue; }
				if (!entity.HasComponent<SpinComponent>()) { continue; }

				ref PositionComponent component = ref entity.GetComponent<PositionComponent>();
				component.Position.X = (float)Math.Cos(this.Tick * 0.15) * 50 + 100;
				component.Position.Y = (float)Math.Sin(this.Tick * 0.15) * 50 + 100;
			}
			this.Tick++;
		}

		#endregion Methods
	}

	public class MovementSystem : IServerSystem, IServerCommandProcessorSystem<TestCommandData>, IClientSystem, IClientPredictedSystem<TestCommandData>
	{
		#region Methods

		public void ServerUpdate(EntityArray entityArray)
		{
		}

		public void ProcessClientCommand(EntityArray entityArray, Entity commandingEntity, TestCommandData commandData, EntityArray lagCompensatedEntityArray)
		{
			if (!commandingEntity.HasComponent<PositionComponent>()) { return; }

			ref PositionComponent component = ref commandingEntity.GetComponent<PositionComponent>();
			if ((commandData.CommandKeys & TestCommandKeys.MoveForward) != 0) { component.Velocity.Y = -5; }
			if ((commandData.CommandKeys & TestCommandKeys.MoveBackward) != 0) { component.Velocity.Y = 5; }
			if ((commandData.CommandKeys & TestCommandKeys.MoveLeft) != 0) { component.Velocity.X = -5; }
			if ((commandData.CommandKeys & TestCommandKeys.MoveRight) != 0) { component.Velocity.X = 5; }

			component.Position += component.Velocity;
			component.Velocity *= 0.65f;
		}

		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void PredictClientCommand(EntityArray entityArray, Entity commandingEntity, TestCommandData commandData)
		{
			this.ProcessClientCommand(entityArray, commandingEntity, commandData, null);
		}

		#endregion Methods
	}

	public struct TestCommandData : ICommandData
	{
		#region Fields

		public TestCommandKeys CommandKeys;

		#endregion Fields

		#region Methods

		public void Deserialize(IReader reader)
		{
			this.CommandKeys = (TestCommandKeys)reader.ReadByte();
		}

		public void Serialize(IWriter writer)
		{
			writer.Write((byte)this.CommandKeys);
		}

		#endregion Methods
	}

	public enum TestCommandKeys : byte
	{
		None = 0,
		MoveForward = 1,
		MoveBackward = 2,
		MoveLeft = 4,
		MoveRight = 8,
		Shoot = 16,
		Seat1 = 32,
		Seat2 = 64,
	}
}
