using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.Game.Sideswipe
{
	public class SideswipeGame : Microsoft.Xna.Framework.Game
	{
		#region Fields

		private readonly IPAddress ipAddressToConnectTo;
		private readonly GraphicsDeviceManager graphicsDeviceManager;
		private Render2dSystem render2DSystem;

		private NetworkServer networkServer;
		private GameServer<PlayerCommandData> gameServer;

		private NetworkClient networkClient;
		private GameClient<PlayerCommandData> gameClient;


		#endregion Fields

		#region Constructors

		public SideswipeGame(IPAddress ipAddressToConnectTo)
		{
			this.ipAddressToConnectTo = ipAddressToConnectTo;

			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			this.IsMouseVisible = true;
		}

		#endregion Constructors

		#region Methods

		#region Startup and shutdown

		/// <summary>
		/// This is where we create the level by creating entities and adding components to them as needed
		/// </summary>
		private void createLevel()
		{
			// Create the level by adding entities for platforms
			{
				// Ground
				this.gameServer.EntityArray.TryCreateEntity(out Entity platformEntity);
				ref SpatialComponent spatialComponent = ref platformEntity.AddComponent<SpatialComponent>();
				ref SpriteComponent spriteComponent = ref platformEntity.AddComponent<SpriteComponent>();
				spatialComponent.Position = new Vector2(0, -7);
				spatialComponent.Extents = new Vector2(700, 14);
				spatialComponent.IsSolid = true;
			}
			{
				// Platform 1
				this.gameServer.EntityArray.TryCreateEntity(out Entity platformEntity);
				ref SpatialComponent spatialComponent = ref platformEntity.AddComponent<SpatialComponent>();
				ref SpriteComponent spriteComponent = ref platformEntity.AddComponent<SpriteComponent>();
				spatialComponent.Position = new Vector2(250, 120);
				spatialComponent.Extents = new Vector2(150, 14);
				spatialComponent.IsSolid = true;
			}
			{
				// Platform 2
				this.gameServer.EntityArray.TryCreateEntity(out Entity platformEntity);
				ref SpatialComponent spatialComponent = ref platformEntity.AddComponent<SpatialComponent>();
				ref SpriteComponent spriteComponent = ref platformEntity.AddComponent<SpriteComponent>();
				spatialComponent.Position = new Vector2(-270, 220);
				spatialComponent.Extents = new Vector2(160, 14);
				spatialComponent.IsSolid = true;
			}
			{
				// Platform 3
				this.gameServer.EntityArray.TryCreateEntity(out Entity platformEntity);
				ref SpatialComponent spatialComponent = ref platformEntity.AddComponent<SpatialComponent>();
				ref SpriteComponent spriteComponent = ref platformEntity.AddComponent<SpriteComponent>();
				spatialComponent.Position = new Vector2(10, 300);
				spatialComponent.Extents = new Vector2(120, 14);
				spatialComponent.IsSolid = true;
			}
		}

		/// <summary>
		/// Initializes the game: creating client/server, creating players, and creating the world
		/// </summary>
		protected override void Initialize()
		{
			Log<LogGameRendering>.StartNew();

			const int maxClients = 10;
			const int entityCapacity = 1000;
			const int maxEntityHistory = 30;
			const int port = 13465;
			const int maxMessageSize = 4000;

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<SpatialComponent>();
			componentsDefinition.RegisterComponentType<PhysicsComponent>();
			componentsDefinition.RegisterComponentType<SpriteComponent>();
			componentsDefinition.RegisterComponentType<CameraComponent>();
			IServerSystem[] serverSystems = new IServerSystem[] { new PlayerInputSystem(), new PhysicsSystem() };
			IClientSystem[] clientSystems = new IClientSystem[] { new PlayerInputSystem(), new PhysicsSystem(), this.render2DSystem = new Render2dSystem(this.graphicsDeviceManager) };

			// If we don't have an IP address to connect to, start our own server
			if (this.ipAddressToConnectTo == null)
			{
				this.networkServer = new NetworkServer("Entmoot.Game.Sideswipe", maxClients, maxMessageSize, port);
				this.gameServer = new GameServer<PlayerCommandData>(this.networkServer.ClientNetworkConnections, maxEntityHistory, entityCapacity,
					componentsDefinition, serverSystems, this.updateCommandingEntityID);

				this.createLevel();

				this.networkServer.Start();
			}

			// If we don't have an IP address to connect to, connect to the server we just started via loopback
			IPAddress ipAddress = this.ipAddressToConnectTo != null ? this.ipAddressToConnectTo : IPAddress.Loopback;

			this.networkClient = new NetworkClient("Entmoot.Game.Sideswipe", maxMessageSize);
			this.gameClient = new GameClient<PlayerCommandData>(this.networkClient, maxEntityHistory, entityCapacity, componentsDefinition, clientSystems);
			this.networkClient.Connect(new System.Net.IPEndPoint(ipAddress, port));

			base.Initialize();
		}

		/// <summary>
		/// Loads assets (textures, etc.). This will load up all PNGs in the Assets folder to be used by any <see cref="SpriteComponent"/>.
		/// </summary>
		protected override void LoadContent()
		{
			this.render2DSystem.SpriteBatch = new SpriteBatch(this.GraphicsDevice);

			this.render2DSystem.ClearSprites();
			using (FileStream fileStream = File.OpenRead("Assets/Blank.png"))
			{
				Texture2D texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
				this.render2DSystem.AddLoadedSprite("", texture);
			}
			foreach (string filename in Directory.GetFiles("Assets", "*.png", SearchOption.AllDirectories))
			{
				using (FileStream fileStream = File.OpenRead(filename))
				{
					Texture2D texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
					this.render2DSystem.AddLoadedSprite(filename.Substring(7).Replace('\\', '/'), texture);
				}
			}

			base.LoadContent();
			GC.Collect();
		}

		/// <summary>
		/// Happens when the game is exiting/closing, just disconnects and cleans up.
		/// </summary>
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
					ref PhysicsComponent physicsComponent = ref clientEntity.AddComponent<PhysicsComponent>();
					ref SpriteComponent spriteComponent = ref clientEntity.AddComponent<SpriteComponent>();
					ref CameraComponent cameraComponent = ref clientEntity.AddComponent<CameraComponent>();

					// This is where we define a player's starting position, size, and sprite
					spatialComponent.Position = new Vector2(0, 70);
					spatialComponent.Extents = new Vector2(32, 60);
					spatialComponent.Rotation = 0;
					spriteComponent.SpriteColor = Color.Aquamarine;

					cameraComponent.Position = new Vector2(0, 180);
					cameraComponent.Extents = new Vector2(800, 480);
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

		/// <summary>
		/// Updates the game, which takes input, updates entities, and resolves things like physics. Systems will get called here.
		/// </summary>
		protected override void Update(GameTime gameTime)
		{
			if (gameTime.IsRunningSlowly) { Log<LogGameRendering>.Data.NumberOfSlowUpdateFrames++; }

			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				this.Exit();
				return;
			}

			string windowTitle = "Sideswipe";
			if (this.networkServer != null)
			{
				this.networkServer.Update();
				this.gameServer.Update();
				windowTitle += $", {Log<LogNetworkServer>.Data.ConnectedClients} clients";
			}

			if (this.networkClient != null)
			{
				PlayerCommandData playerCommandData = PlayerInput.GetPlayerInput(Mouse.GetState(), Keyboard.GetState(), this.IsActive);

				this.networkClient.Update();
				this.gameClient.Update(playerCommandData);
				windowTitle += $", connected={this.networkClient.IsConnected}";
			}

			this.Window.Title = windowTitle;
			base.Update(gameTime);
		}

		/// <summary>
		/// Renders the game. This won't update any entities, this just renders the current state to the screen.
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
			if (gameTime.IsRunningSlowly) { Log<LogGameRendering>.Data.NumberOfSlowDrawFrames++; }

			this.GraphicsDevice.Clear(Color.CornflowerBlue);

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
