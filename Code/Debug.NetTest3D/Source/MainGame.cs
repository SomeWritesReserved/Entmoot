﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.Debug.NetTest3D
{
	public class MainGame : Game
	{
		#region Fields

		private const int maxClients = 4;

		private readonly GraphicsDeviceManager graphicsDeviceManager;
		private BasicEffect basicEffect;

		private readonly StringBuilder stringBuilder = new StringBuilder(2048);
		private SpriteBatch spriteBatch;
		private SpriteFont spriteFont;
		private Graph graph;

		private readonly bool hasServer;
		private readonly RenderSystem renderSystem;
		private readonly NetworkServer networkServer;
		private readonly NetworkClient networkClient;
		private readonly GameServer<CommandData> gameServer;
		private readonly GameClient<CommandData> gameClient;
		private CommandData commandData = new CommandData();

		private bool isNetworkedPaused;
		private int slowFrames;

		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;
		private MouseState currentMouseState;
		private MouseState previousMouseState;
		private Point mouseDownPoint;
		private bool isDragging;

		#endregion Fields

		#region Constructors

		public MainGame()
		{
			this.InactiveSleepTime = TimeSpan.Zero;
			this.IsMouseVisible = true;
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			this.renderSystem = new RenderSystem(this.graphicsDeviceManager);
			this.Content.RootDirectory = "Assets";

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<SpatialComponent>();
			componentsDefinition.RegisterComponentType<PhysicsComponent>();
			componentsDefinition.RegisterComponentType<ColorComponent>();

			this.hasServer = !Environment.GetCommandLineArgs().Any((arg) => arg.Equals("-ns", StringComparison.OrdinalIgnoreCase));
			if (this.hasServer)
			{
				this.networkServer = new NetworkServer("1", MainGame.maxClients, 4000, 19876);
				this.gameServer = new GameServer<CommandData>(this.networkServer.ClientNetworkConnections, 20, 30,
					componentsDefinition, new IServerSystem[] { new ClientCommandSystem(), new SpinnerSystem(), new PhysicsSystem() }, this.updateCommandingEntityID);

				// Make some dummy entities that we'll remove to have a gap in entity IDs
				this.gameServer.EntityArray.TryCreateEntity(out Entity dummy1);
				this.gameServer.EntityArray.TryCreateEntity(out Entity dummy2);

				// Add some stuff to the world
				for (int x = -1; x <= 1; x++)
				{
					for (int z = -1; z <= 1; z++)
					{
						this.gameServer.EntityArray.TryCreateEntity(out Entity entity);
						entity.AddComponent<SpatialComponent>().Position = new Vector3(x * 5, 0, z * 5);
						if (entity.ID == 11)
						{
							entity.AddComponent<ColorComponent>().Color = new Color(1.0f, 0.5f, 0.5f);
						}
						else if (entity.ID == 12)
						{
							entity.AddComponent<ColorComponent>().Color = new Color(0.0f, 1.0f, 0.5f);
						}
					}
				}

				// Remove those dummy entities to have a gap in entity IDs
				this.gameServer.EntityArray.RemoveEntity(dummy1);
				this.gameServer.EntityArray.RemoveEntity(dummy2);
				this.gameServer.EntityArray.EndUpdate();
			}

			this.networkClient = new NetworkClient("1", 4000);
			this.gameClient = new GameClient<CommandData>(this.networkClient, 20, 30,
				componentsDefinition, new IClientSystem[] { new ClientCommandSystem(), new PhysicsSystem(), this.renderSystem });
		}

		#endregion Constructors

		#region Methods

		protected override void Initialize()
		{
			if (this.hasServer)
			{
				this.networkServer.Start();
			}

			base.Initialize();
		}

		protected override void LoadContent()
		{
			this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
			this.spriteFont = this.Content.Load<SpriteFont>("dev_font");
			this.graph = new Graph(this.GraphicsDevice, new Point(this.GraphicsDevice.Viewport.Width, 100));
			this.graph.Position = new Vector2(0, this.GraphicsDevice.Viewport.Height - 5);

			this.basicEffect = new BasicEffect(this.GraphicsDevice);
			this.basicEffect.LightingEnabled = true;
			this.basicEffect.TextureEnabled = true;
			this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), this.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
			this.basicEffect.EnableDefaultLighting();

			using (FileStream fileStream = new FileStream(@"Assets/dev_cubeface.png", FileMode.Open, FileAccess.Read))
			{
				this.basicEffect.Texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			}
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			this.networkClient.Disconnect();
			if (this.hasServer)
			{
				this.networkServer.Stop();
			}
			System.Threading.Thread.Sleep(250);
			base.OnExiting(sender, args);
		}

		protected override void OnDeactivated(object sender, EventArgs args)
		{
			this.isDragging = false;
			base.OnDeactivated(sender, args);
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
					clientEntity.AddComponent<SpatialComponent>();
					clientEntity.AddComponent<PhysicsComponent>();
					clientEntity.AddComponent<ColorComponent>().Color = new Color(0.5f, 0.5f, 1.0f);
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

		protected override void Update(GameTime gameTime)
		{
			if (gameTime.IsRunningSlowly) { this.slowFrames++; }

			this.currentKeyboardState = Keyboard.GetState();
			this.currentMouseState = Mouse.GetState();

			if (this.isKeyPressed(Keys.U) && !this.networkClient.IsConnected)
			{
				this.networkClient.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 19876));
			}
			else if (this.isKeyPressed(Keys.P))
			{
				this.isNetworkedPaused = !this.isNetworkedPaused;
			}
			else if (this.isKeyPressed(Keys.Escape))
			{
				this.Exit();
			}

			if (this.isRightMousePressed())
			{
				this.isDragging = true;
				this.mouseDownPoint = new Point(this.currentMouseState.X, this.currentMouseState.Y);
			}
			else if (this.currentMouseState.RightButton == ButtonState.Released)
			{
				this.isDragging = false;
			}

			this.updateClientAndServer();
			this.Window.Title = this.networkClient.IsConnected ? "Connected" : "Disconnected";

			this.previousKeyboardState = this.currentKeyboardState;
			this.previousMouseState = this.currentMouseState;
		}

		protected override void Draw(GameTime gameTime)
		{
			if (gameTime.IsRunningSlowly) { this.slowFrames++; }

			this.GraphicsDevice.Clear(Color.Gray);
			if (this.gameClient.HasRenderingStarted)
			{
				this.renderSystem.BasicEffect = this.basicEffect;
				this.gameClient.SystemArray.ClientRender(this.gameClient.RenderedSnapshot.EntityArray, this.gameClient.GetCommandingEntity());
			}

			this.drawDebugUI();
		}

		private void updateClientAndServer()
		{
			if (this.hasServer && !this.isNetworkedPaused)
			{
				this.networkServer.Update();
				this.gameServer.Update();
			}

			// Client
			{
				if (this.IsActive && this.isDragging)
				{
					float deltaX = (this.mouseDownPoint.X - this.currentMouseState.X) * 0.004f;
					float deltaY = (this.mouseDownPoint.Y - this.currentMouseState.Y) * 0.004f;
					this.commandData.LookAngles.X += deltaX;
					this.commandData.LookAngles.Y = MathHelper.Clamp(this.commandData.LookAngles.Y + deltaY, -MathHelper.Pi * 0.49f, MathHelper.Pi * 0.49f);
					Mouse.SetPosition(this.mouseDownPoint.X, this.mouseDownPoint.Y);
				}

				this.commandData.Commands = Commands.None;
				if (this.currentKeyboardState.IsKeyDown(Keys.W)) { this.commandData.Commands |= Commands.MoveForward; }
				if (this.currentKeyboardState.IsKeyDown(Keys.S)) { this.commandData.Commands |= Commands.MoveBackward; }
				if (this.currentKeyboardState.IsKeyDown(Keys.A)) { this.commandData.Commands |= Commands.MoveLeft; }
				if (this.currentKeyboardState.IsKeyDown(Keys.D)) { this.commandData.Commands |= Commands.MoveRight; }

				if (!this.isNetworkedPaused)
				{
					this.networkClient.Update();
					this.gameClient.Update(this.commandData);
				}
			}
		}

		private void drawDebugUI()
		{
			this.stringBuilder.Clear();

			this.stringBuilder.Append("SlowFrames   ");
			this.stringBuilder.Append(this.slowFrames);
			if (this.hasServer)
			{
				this.stringBuilder.Append("\nSERVER");
				this.stringBuilder.Append("\n FrameTick   ");
				this.stringBuilder.Append(this.gameServer.FrameTick);
				this.stringBuilder.Append("\n RecvBytes/s ");
				this.stringBuilder.Append(Log<LogNetworkServer>.History.Sum((d) => d.ReceivedBytes) / 2);
				this.stringBuilder.Append("\n SentBytes/s ");
				this.stringBuilder.Append(Log<LogNetworkServer>.History.Sum((d) => d.SentBytes) / 2);
				this.stringBuilder.Append("\n SerializeMS ");
				if (Log<LogServerUpdateSerialization>.History?.Any() == true)
				{
					this.stringBuilder.AppendFormat("{0:0.000}", Log<LogServerUpdateSerialization>.History.Average((d) => d.SerializationTime.DurationMs));
				}
				this.stringBuilder.Append("\n Clients     ");
				this.stringBuilder.Append(Log<LogNetworkServer>.Data.ConnectedClients);
				this.stringBuilder.Append("\n Connecting  ");
				this.stringBuilder.Append(Log<LogNetworkServer>.Data.ConnectingClients);
			}

			this.stringBuilder.Append("\nCLIENT");
			this.stringBuilder.Append("\n FrameTick   ");
			this.stringBuilder.Append(this.gameClient.FrameTick);
			this.stringBuilder.Append("\n RecvBytes/s ");
			this.stringBuilder.Append(Log<LogNetworkClient>.History.Sum((d) => d.ReceivedBytes) / 2);
			this.stringBuilder.Append("\n SentBytes/s ");
			this.stringBuilder.Append(Log<LogNetworkClient>.History.Sum((d) => d.SentBytes) / 2);
			this.stringBuilder.Append("\n DeserialzMS ");
			if (Log<LogServerUpdateDeserialization>.History?.Any() == true)
			{
				this.stringBuilder.AppendFormat("{0:0.000}", Log<LogServerUpdateDeserialization>.History.Average((d) => d.DeserializationTime.DurationMs));
			}

			BlendState blendState = this.GraphicsDevice.BlendState;
			DepthStencilState depthStencilState = this.GraphicsDevice.DepthStencilState;
			this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			this.spriteBatch.DrawString(this.spriteFont, this.stringBuilder, Vector2.One, Color.White);
			this.spriteBatch.End();
			this.GraphicsDevice.BlendState = blendState;
			this.GraphicsDevice.DepthStencilState = depthStencilState;

			if (this.hasServer)
			{
				//this.drawGraph(Log<LogNetworkServer>.History, (log) => log.ReceivedBytes, Color.AliceBlue);
				this.drawGraph(Log<LogNetworkServer>.History, (log) => log.SentBytes, Color.AliceBlue);
			}
			else
			{
				this.drawGraph(Log<LogNetworkClient>.History, (log) => log.ReceivedBytes, Color.AliceBlue);
				//this.drawGraph(Log<LogNetworkClient>.History, (log) => log.SentBytes, Color.AliceBlue);
			}
		}

		private float[] graphData = new float[120];
		private void drawGraph<T>(Queue<T> history, Func<T, float> selector, Color color)
		{
			float max = 0;
			int index = this.graphData.Length - history.Count;
			foreach (T t in history)
			{
				float value = selector(t);
				if (value > max) { max = value; }
				this.graphData[index++] = value;
			}
			this.graph.MaxValue = max;
			this.graph.Draw(this.graphData, color);
		}

		private bool isKeyPressed(Keys key)
		{
			return (this.currentKeyboardState.IsKeyDown(key) && this.previousKeyboardState.IsKeyUp(key));
		}

		private bool isRightMousePressed()
		{
			return (this.currentMouseState.RightButton == ButtonState.Pressed && this.previousMouseState.RightButton == ButtonState.Released);
		}

		#endregion Methods
	}
}
