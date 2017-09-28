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

		private readonly GraphicsDeviceManager graphicsDeviceManager;
		private readonly RenderSystem renderSystem;
		private readonly ListenServer<CommandData> listenServer;
		private CommandData commandData = new CommandData();
		private BasicEffect basicEffect;

		#endregion Fields

		#region Constructors

		public MainGame()
		{
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<SpatialComponent>();

			this.renderSystem = new RenderSystem(this.graphicsDeviceManager);
			this.listenServer = new ListenServer<CommandData>(4000, 20, 100, componentsDefinition, new ISystem[0], new ISystem[] { this.renderSystem });
			this.listenServer.GameClient.NetworkSendRate = 1;
			this.listenServer.GameClient.InterpolationRenderDelay = 3;
			this.listenServer.GameServer.NetworkSendRate = 1;

			this.listenServer.GameServer.EntityArray.TryCreateEntity(out Entity clientEntity);
			clientEntity.AddComponent<SpatialComponent>();

			for (int x = -2; x <= 2; x++)
			{
				for (int z = -2; z <= 2; z++)
				{
					this.listenServer.GameServer.EntityArray.TryCreateEntity(out Entity entity);
					entity.AddComponent<SpatialComponent>().Position = new Vector3(x * 5, 0, z * 5);
				}
			}
		}

		#endregion Constructors

		#region Methods

		protected override void LoadContent()
		{
			this.basicEffect = new BasicEffect(this.GraphicsDevice);
			this.basicEffect.LightingEnabled = true;
			this.basicEffect.TextureEnabled = true;
			this.basicEffect.EnableDefaultLighting();
			using (FileStream fileStream = new FileStream(@"Assets\dev_cube.png", FileMode.Open, FileAccess.Read))
			{
				this.basicEffect.Texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			}

			Mouse.SetPosition(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			this.GraphicsDevice.Clear(Color.Gray);

			this.basicEffect.View = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
			this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), this.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
			this.renderSystem.BasicEffect = this.basicEffect;

			MouseState mouseState = Mouse.GetState();
			KeyboardState keyboardState = Keyboard.GetState();
			{
				float deltaX = (this.GraphicsDevice.Viewport.Width / 2 - mouseState.X) * 0.004f;
				float deltaY = (this.GraphicsDevice.Viewport.Height / 2 - mouseState.Y) * 0.004f;
				this.commandData.LookAngles.X += deltaX;
				this.commandData.LookAngles.Y = MathHelper.Clamp(this.commandData.LookAngles.Y + deltaY, -MathHelper.Pi * 0.49f, MathHelper.Pi * 0.49f);

				this.commandData.Commands = Commands.None;
				if (keyboardState.IsKeyDown(Keys.W)) { this.commandData.Commands |= Commands.MoveForward; }
				if (keyboardState.IsKeyDown(Keys.S)) { this.commandData.Commands |= Commands.MoveBackward; }
				if (keyboardState.IsKeyDown(Keys.A)) { this.commandData.Commands |= Commands.MoveLeft; }
				if (keyboardState.IsKeyDown(Keys.D)) { this.commandData.Commands |= Commands.MoveRight; }

				if (this.listenServer.GameClient.HasRenderingStarted && this.listenServer.GameClient.RenderedSnapshot.EntityArray.TryGetEntity(this.listenServer.GameClient.CommandingEntityID, out Entity clientEntity))
				{
					if (clientEntity.HasComponent<SpatialComponent>())
					{
						ref SpatialComponent spatialComponent = ref clientEntity.GetComponent<SpatialComponent>();
						this.basicEffect.View = Matrix.CreateLookAt(spatialComponent.Position, spatialComponent.Position + Vector3.Transform(Vector3.Forward, spatialComponent.Rotation), Vector3.Up);
					}
				}

				this.listenServer.Update(this.commandData);
			}

			Mouse.SetPosition(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2);
			if (keyboardState.IsKeyDown(Keys.Escape)) { this.Exit(); }
		}

		#endregion Methods
	}
}
