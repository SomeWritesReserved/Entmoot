using System;
using System.Collections.Generic;
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

		#endregion Fields

		#region Constructors

		public ZombtownXnaGame()
		{
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

			const int maxClients = 4;
			const int entityCapacity = 1000;
			const int maxEntityHistory = 30;
			const int port = 13460;
			const int maxMessageSize = 4000;

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();

			IServerSystem[] serverSystems = new IServerSystem[] { };

			IClientSystem[] clientSystems = new IClientSystem[] { new Render2dSystem(this.graphicsDeviceManager) };

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
			base.LoadContent();
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
