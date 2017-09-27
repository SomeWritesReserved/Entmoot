using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.TestGame3D
{
	public class MainGame : Game
	{
		#region Fields

		private readonly GraphicsDeviceManager graphicsDeviceManager;
		private BasicEffect basicEffect;

		#endregion Fields

		#region Constructors

		public MainGame()
		{
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
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
		}

		protected override void Update(GameTime gameTime)
		{
			KeyboardState keyboardState = Keyboard.GetState();
			if (keyboardState.IsKeyDown(Keys.Escape)) { this.Exit(); }

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			this.GraphicsDevice.Clear(Color.Gray);

			this.basicEffect.View = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
			this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), this.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);

			ShapeRenderHelper.DrawBox(this.GraphicsDevice, this.basicEffect, Vector3.Forward * 3, (float)gameTime.TotalGameTime.TotalSeconds);

			base.Draw(gameTime);
		}

		#endregion Methods
	}
}
