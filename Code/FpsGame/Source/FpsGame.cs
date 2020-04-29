using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.FpsGame
{
	public class FpsGame : Game
	{
		#region Fields

		private readonly GraphicsDeviceManager graphicsDeviceManager;

		private NetworkServer networkServer;
		private NetworkClient networkClient;
		private GameServer<PlayerCommandData> gameServer;
		private GameClient<PlayerCommandData> gameClient;

		#endregion Fields

		#region Constructors

		public FpsGame()
		{
			this.IsMouseVisible = true;
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			this.graphicsDeviceManager.PreferredBackBufferWidth = 960;
			this.graphicsDeviceManager.PreferredBackBufferHeight = 540;
			this.Content.RootDirectory = "Assets";
		}

		#endregion Constructors

		#region Methods

		#region Startup and shutdown

		protected override void Initialize()
		{
			Log<LogGameRendering>.StartNew();
			this.startServer();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			this.stopServer();

			base.OnExiting(sender, args);
		}

		private void startServer()
		{
			const byte maxClients = 4;
			const int entityCapacity = 1000;
			const int maxEntityHistory = 30;
			const int port = 13450;
			const int maxMessageSize = 4000;

			this.stopServer();

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			//componentsDefinition.RegisterComponentType<SomeComponent>();

			IServerSystem[] serverSystems = new IServerSystem[] { new PlayerMovementSystem() };

			IClientSystem[] clientSystems = new IClientSystem[] { new PlayerMovementSystem() };

			this.networkServer = new NetworkServer("Entmoot.FpsGame", maxClients, maxMessageSize, port);
			this.gameServer = new GameServer<PlayerCommandData>(this.networkServer.ClientNetworkConnections, maxEntityHistory, entityCapacity, componentsDefinition, serverSystems);

			// Reserve the first entities for all potential clients
			for (int clientID = 0; clientID < maxClients; clientID++)
			{
				this.gameServer.EntityArray.TryCreateEntity(out Entity clientEntity);
				//clientEntity.AddComponent<SomeComponent>();
			}

			this.networkClient = new NetworkClient("Entmoot.FpsGame", maxMessageSize);
			this.gameClient = new GameClient<PlayerCommandData>(this.networkClient, maxEntityHistory, entityCapacity, componentsDefinition, clientSystems);

			this.networkServer.Start();
			this.networkClient.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port));
		}

		private void stopServer()
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
		}

		#endregion Startup and shutdown

		#region Update and Draw

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

			this.GraphicsDevice.Clear(Color.Gray);

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
