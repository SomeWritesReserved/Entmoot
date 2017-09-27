using System;
using System.Collections.Generic;
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

		#endregion Fields

		#region Constructors

		public MainGame()
		{
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
		}

		#endregion Constructors

		#region Methods

		protected override void LoadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			KeyboardState keyboardState = Keyboard.GetState();
			if (keyboardState.IsKeyDown(Keys.Escape)) { this.Exit(); }

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			this.graphicsDeviceManager.GraphicsDevice.Clear(Color.Gray);

			base.Draw(gameTime);
		}

		#endregion Methods
	}
}
