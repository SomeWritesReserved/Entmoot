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

namespace Entmoot.Game.Zombtown
{
	public class ZombtownXnaGame : Microsoft.Xna.Framework.Game
	{
		#region Fields

		private readonly GraphicsDeviceManager graphicsDeviceManager;

		private NetworkServer networkServer;
		private NetworkClient networkClient;
		private GameServer<PlayerCommandData> gameServer;
		private GameClient<PlayerCommandData> gameClient;

		private Render2dSystem render2DSystem;

		#endregion Fields

		#region Constructors

		public ZombtownXnaGame()
		{
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			this.graphicsDeviceManager.PreferredBackBufferWidth = 1024;
			this.graphicsDeviceManager.PreferredBackBufferHeight = 768;
			this.Content.RootDirectory = "Assets";
		}

		#endregion Constructors

		#region Methods

		#region Startup and shutdown

		protected override void Initialize()
		{
			Log<LogGameRendering>.StartNew();

			const int maxClients = 1;
			const int entityCapacity = 1000;
			const int maxEntityHistory = 30;
			const int port = 13460;
			const int maxMessageSize = 4000;

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<SpatialComponent>();
			componentsDefinition.RegisterComponentType<SpriteComponent>();
			componentsDefinition.RegisterComponentType<CameraComponent>();
			IServerSystem[] serverSystems = new IServerSystem[] { new CameraSystem() };
			IClientSystem[] clientSystems = new IClientSystem[] { this.render2DSystem = new Render2dSystem(this.graphicsDeviceManager) };

			this.networkServer = new NetworkServer("Entmoot.Game.Zombtown", maxClients, maxMessageSize, port);
			this.gameServer = new GameServer<PlayerCommandData>(this.networkServer.ClientNetworkConnections, maxEntityHistory, entityCapacity, componentsDefinition, serverSystems, this.updateCommandingEntityID);

			this.networkClient = new NetworkClient("Entmoot.Game.Zombtown", maxMessageSize);
			this.gameClient = new GameClient<PlayerCommandData>(this.networkClient, maxEntityHistory, entityCapacity, componentsDefinition, clientSystems);

			this.networkServer.Start();
			this.networkClient.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port));

			base.Initialize();
		}

		protected override void LoadContent()
		{
			this.render2DSystem.Clear();
			this.render2DSystem.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
			foreach (string filename in Directory.GetFiles("Assets", "*.png", SearchOption.AllDirectories))
			{
				using (FileStream fileStream = File.OpenRead(filename))
				{
					Texture2D texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
					this.render2DSystem.AddTexture(filename.Substring(7).Replace('\\', '/'), texture);
				}
			}

			base.LoadContent();
			GC.Collect();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			if (this.networkClient != null)
			{
				this.networkClient.Disconnect();
				this.networkClient = null;
				this.gameClient = null;
			}
			if (this.networkServer != null)
			{
				this.networkServer.Stop();
				this.networkServer = null;
				this.gameServer = null;
			}

			base.OnExiting(sender, args);
		}

		protected override void OnActivated(object sender, EventArgs args)
		{
			base.OnActivated(sender, args);
		}

		#endregion Startup and shutdown

		#region Update and Draw

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
					ref SpatialComponent spatialComponent = ref clientEntity.AddComponent<SpatialComponent>();
					ref SpriteComponent spriteComponent = ref clientEntity.AddComponent<SpriteComponent>();
					ref CameraComponent cameraComponent = ref clientEntity.AddComponent<CameraComponent>();
					spatialComponent.Position = new Vector2(56, 23);
					spatialComponent.Radius = 1;
					spatialComponent.Rotation = 0.78f;
					spriteComponent.SetSprite("Character.png");
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
			if (gameTime.IsRunningSlowly) { Log<LogGameRendering>.Data.NumberOfSlowUpdateFrames++; }

			if (this.networkServer != null)
			{
				this.networkServer.Update();
				this.gameServer.Update();
			}

			if (this.networkClient != null)
			{
				this.networkClient.Update();
				this.gameClient.Update(new PlayerCommandData());
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			if (gameTime.IsRunningSlowly) { Log<LogGameRendering>.Data.NumberOfSlowDrawFrames++; }

			this.GraphicsDevice.Clear(Color.Black);

			if (this.gameClient != null && this.gameClient.HasRenderingStarted)
			{
				this.gameClient.SystemArray.ClientRender(this.gameClient.RenderedSnapshot.EntityArray, this.gameClient.GetCommandingEntity());
			}

			base.Draw(gameTime);
		}

		#endregion Update and Draw

		#endregion Methods
	}
}
