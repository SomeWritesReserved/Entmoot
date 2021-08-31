using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.Game.Fps
{
	public class FpsGame : Microsoft.Xna.Framework.Game
	{
		#region Fields

		private readonly IPAddress ipAddressToConnectTo;
		private readonly GraphicsDeviceManager graphicsDeviceManager;

		private NetworkServer networkServer;
		private GameServer<PlayerCommandData> gameServer;

		private NetworkClient networkClient;
		private GameClient<PlayerCommandData> gameClient;

		private RenderSystem renderSystem;
		private BasicEffect basicEffect;

		#endregion Fields

		#region Constructors

		public FpsGame(IPAddress ipAddressToConnectTo)
		{
			this.ipAddressToConnectTo = ipAddressToConnectTo;

			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			this.IsMouseVisible = false;
		}

		#endregion Constructors

		#region Methods

		#region Startup and shutdown

		/// <summary>
		/// This is where we create the level by creating entities and adding components to them as needed
		/// </summary>
		private void createLevel()
		{
			// Create the level by adding entities for obstacles
			{
				// Ground
				this.gameServer.EntityArray.TryCreateEntity(out Entity platformEntity);
				ref SpatialComponent spatialComponent = ref platformEntity.AddComponent<SpatialComponent>();
				spatialComponent.Position = new Vector3(0, -10, 0);
				spatialComponent.Extents = new Vector3(5000, 10, 5000);
			}
			{
				// Box
				this.gameServer.EntityArray.TryCreateEntity(out Entity platformEntity);
				ref SpatialComponent spatialComponent = ref platformEntity.AddComponent<SpatialComponent>();
				spatialComponent.Position = Vector3.Forward * 100;
				spatialComponent.Extents = new Vector3(5, 10, 5);
			}
			{
				// NPC
				this.gameServer.EntityArray.TryCreateEntity(out Entity platformEntity);
				ref SpatialComponent spatialComponent = ref platformEntity.AddComponent<SpatialComponent>();
				spatialComponent.Position = Vector3.Forward * 100 + Vector3.Right * 30;
				spatialComponent.Extents = new Vector3(11, 72, 11);
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
			const int port = 13466;
			const int maxMessageSize = 4000;

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<SpatialComponent>();
			componentsDefinition.RegisterComponentType<MovementComponent>();
			IServerSystem[] serverSystems = new IServerSystem[] { new PlayerMovementSystem(), };
			IClientSystem[] clientSystems = new IClientSystem[] { new PlayerMovementSystem(), this.renderSystem = new RenderSystem(this.graphicsDeviceManager) };

			// If we don't have an IP address to connect to, start our own server
			if (this.ipAddressToConnectTo == null)
			{
				this.networkServer = new NetworkServer("Entmoot.Game.Fps", maxClients, maxMessageSize, port);
				this.gameServer = new GameServer<PlayerCommandData>(this.networkServer.ClientNetworkConnections, maxEntityHistory, entityCapacity,
					componentsDefinition, serverSystems, this.updateCommandingEntityID);

				this.createLevel();

				this.networkServer.Start();
			}

			// If we don't have an IP address to connect to, connect to the server we just started via loopback
			IPAddress ipAddress = this.ipAddressToConnectTo != null ? this.ipAddressToConnectTo : IPAddress.Loopback;

			this.networkClient = new NetworkClient("Entmoot.Game.Fps", maxMessageSize);
			this.gameClient = new GameClient<PlayerCommandData>(this.networkClient, maxEntityHistory, entityCapacity, componentsDefinition, clientSystems);
			this.networkClient.Connect(new System.Net.IPEndPoint(ipAddress, port));

			base.Initialize();
		}

		/// <summary>
		/// Loads assets (textures, etc.).
		/// </summary>
		protected override void LoadContent()
		{
			this.basicEffect = new BasicEffect(this.GraphicsDevice);
			this.basicEffect.LightingEnabled = true;
			this.basicEffect.TextureEnabled = true;
			this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75.0f), this.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
			this.basicEffect.EnableDefaultLighting();

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
					ref MovementComponent movementComponent = ref clientEntity.AddComponent<MovementComponent>();

					// This is where we define a player's starting position, size, and sprite
					spatialComponent.Position = new Vector3(0, 0, 0);
					spatialComponent.Extents = new Vector3(11, 72, 11);
					spatialComponent.Color = Color.LawnGreen;
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

			string windowTitle = $"FPS (slow frames #{Log<LogGameRendering>.Data.NumberOfSlowDrawFrames})";
			if (this.networkServer != null)
			{
				this.networkServer.Update();
				this.gameServer.Update();
				windowTitle += $", {Log<LogNetworkServer>.Data.ConnectedClients} clients";
			}

			if (this.networkClient != null)
			{
				Point mouseCenterPosition = this.GraphicsDevice.Viewport.Bounds.Center;
				PlayerCommandData playerCommandData = PlayerInput.GetPlayerCommandData(mouseCenterPosition, this.IsActive);

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

			this.GraphicsDevice.Clear(Color.Gray);

			if (this.gameClient != null && this.gameClient.HasRenderingStarted)
			{
				this.renderSystem.BasicEffect = this.basicEffect;
				this.gameClient.SystemArray.ClientRender(this.gameClient.RenderedSnapshot.EntityArray, this.gameClient.GetCommandingEntity());
			}

			base.Draw(gameTime);
		}

		#endregion Update and Draw

		#endregion Methods
	}
}
