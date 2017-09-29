using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.TestGame3D
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

		private readonly bool hasServer;
		private readonly RenderSystem renderSystem;
		private readonly NetworkServer networkServer;
		private readonly NetworkClient networkClient;
		private readonly GameServer<CommandData> gameServer;
		private readonly GameClient<CommandData> gameClient;
		private CommandData commandData = new CommandData();

		#endregion Fields

		#region Constructors

		public MainGame()
		{
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			this.renderSystem = new RenderSystem(this.graphicsDeviceManager);
			this.Content.RootDirectory = "Assets";

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<SpatialComponent>();

			this.hasServer = Environment.GetCommandLineArgs().Any((arg) => arg.Equals("-s", StringComparison.OrdinalIgnoreCase));
			if (this.hasServer)
			{
				this.networkServer = new NetworkServer("1", MainGame.maxClients, 4000, 19876);
				this.gameServer = new GameServer<CommandData>(this.networkServer.ClientNetworkConnections, 20, 30, componentsDefinition, new ISystem[0]);

				// Reserve the first entities for all potential clients
				for (int clientID = 0; clientID < MainGame.maxClients; clientID++)
				{
					this.gameServer.EntityArray.TryCreateEntity(out Entity clientEntity);
					clientEntity.AddComponent<SpatialComponent>();
				}

				// Add some stuff to the world
				for (int x = -2; x <= 2; x++)
				{
					for (int z = -2; z <= 2; z++)
					{
						this.gameServer.EntityArray.TryCreateEntity(out Entity entity);
						entity.AddComponent<SpatialComponent>().Position = new Vector3(x * 5, 0, z * 5);
					}
				}
			}

			this.networkClient = new NetworkClient("1", 4000);
			this.gameClient = new GameClient<CommandData>(this.networkClient, 20, 30, componentsDefinition, new ISystem[] { this.renderSystem });
		}

		#endregion Constructors

		#region Methods

		protected override void OnActivated(object sender, EventArgs args)
		{
			Mouse.SetPosition(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2);
			base.OnActivated(sender, args);
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			this.networkClient.Disconnect();
			if (this.hasServer)
			{
				this.networkServer.Stop();
			}
			base.OnExiting(sender, args);
		}

		protected override void Initialize()
		{
			if (this.hasServer)
			{
				this.networkServer.Start();
			}
			this.networkClient.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 19876));

			base.Initialize();
		}

		protected override void LoadContent()
		{
			this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
			this.spriteFont = this.Content.Load<SpriteFont>("dev_font");

			this.basicEffect = new BasicEffect(this.GraphicsDevice);
			this.basicEffect.LightingEnabled = true;
			this.basicEffect.TextureEnabled = true;
			this.basicEffect.EnableDefaultLighting();
			using (FileStream fileStream = new FileStream(@"Assets\dev_cubeface.png", FileMode.Open, FileAccess.Read))
			{
				this.basicEffect.Texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			}
		}

		protected override void Update(GameTime gameTime)
		{
		}

		protected override void Draw(GameTime gameTime)
		{
			this.GraphicsDevice.Clear(Color.Gray);

			this.basicEffect.View = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
			this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), this.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
			this.renderSystem.BasicEffect = this.basicEffect;

			if (this.hasServer)
			{
				this.networkServer.Update();
				this.gameServer.Update();
			}

			MouseState mouseState = Mouse.GetState();
			KeyboardState keyboardState = Keyboard.GetState();
			{
				if (this.IsActive && mouseState.RightButton == ButtonState.Pressed)
				{
					float deltaX = (this.GraphicsDevice.Viewport.Width / 2 - mouseState.X) * 0.004f;
					float deltaY = (this.GraphicsDevice.Viewport.Height / 2 - mouseState.Y) * 0.004f;
					this.commandData.LookAngles.X += deltaX;
					this.commandData.LookAngles.Y = MathHelper.Clamp(this.commandData.LookAngles.Y + deltaY, -MathHelper.Pi * 0.49f, MathHelper.Pi * 0.49f);
					Mouse.SetPosition(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2);
				}

				this.commandData.Commands = Commands.None;
				if (keyboardState.IsKeyDown(Keys.W)) { this.commandData.Commands |= Commands.MoveForward; }
				if (keyboardState.IsKeyDown(Keys.S)) { this.commandData.Commands |= Commands.MoveBackward; }
				if (keyboardState.IsKeyDown(Keys.A)) { this.commandData.Commands |= Commands.MoveLeft; }
				if (keyboardState.IsKeyDown(Keys.D)) { this.commandData.Commands |= Commands.MoveRight; }

				if (this.gameClient.HasRenderingStarted && this.gameClient.RenderedSnapshot.EntityArray.TryGetEntity(this.gameClient.CommandingEntityID, out Entity clientEntity))
				{
					if (clientEntity.HasComponent<SpatialComponent>())
					{
						ref SpatialComponent spatialComponent = ref clientEntity.GetComponent<SpatialComponent>();
						this.basicEffect.View = Matrix.CreateLookAt(spatialComponent.Position, spatialComponent.Position + Vector3.Transform(Vector3.Forward, spatialComponent.Rotation), Vector3.Up);
					}
				}
				this.networkClient.Update();
				this.gameClient.Update(this.commandData);
			}

			this.drawDebugUI();

			if (keyboardState.IsKeyDown(Keys.Escape)) { this.Exit(); }
		}

		private void drawDebugUI()
		{
			this.stringBuilder.Clear();

			if (this.hasServer)
			{
				this.stringBuilder.Append("SERVER\n RecvBytes/s ");
				this.stringBuilder.Append(Log<LogNetworkServer>.History.Sum((d) => d.ReceivedBytes) / 2);
				this.stringBuilder.Append("\n SentBytes/s ");
				this.stringBuilder.Append(Log<LogNetworkServer>.History.Sum((d) => d.SentBytes) / 2);
				this.stringBuilder.Append("\n NumClients  ");
				this.stringBuilder.Append(Log<LogNetworkServer>.Data.ConnectedClients);
				this.stringBuilder.Append("\n ClientsWait ");
				this.stringBuilder.Append(Log<LogNetworkServer>.Data.ConnectingClients);
			}

			this.stringBuilder.Append("\nCLIENT\n RecvBytes/s ");
			this.stringBuilder.Append(Log<LogNetworkClient>.History.Sum((d) => d.ReceivedBytes) / 2);
			this.stringBuilder.Append("\n SentBytes/s ");
			this.stringBuilder.Append(Log<LogNetworkClient>.History.Sum((d) => d.SentBytes) / 2);

			BlendState blendState = this.GraphicsDevice.BlendState;
			DepthStencilState depthStencilState = this.GraphicsDevice.DepthStencilState;
			this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			this.spriteBatch.DrawString(this.spriteFont, this.stringBuilder, Vector2.One, Color.White);
			this.spriteBatch.End();
			this.GraphicsDevice.BlendState = blendState;
			this.GraphicsDevice.DepthStencilState = depthStencilState;
		}

		#endregion Methods
	}
}
