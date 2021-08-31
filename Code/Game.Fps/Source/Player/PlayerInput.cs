using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.Game.Fps
{
	/// <summary>
	/// A helper class for reading a player's input.
	/// </summary>
	public static class PlayerInput
	{
		#region Fields

		private static Vector2 lookAngles;
		private static bool didReadInputLastFrame = false;

		#endregion Fields

		#region Methods

		/// <summary>
		/// A method to populate a <see cref="PlayerCommandData"/> based on the player's mouse and keyboard input.
		/// This maps keyboard keys to <see cref="PlayerInputButtons"/> and handles mouse input.
		/// </summary>
		public static PlayerCommandData GetPlayerCommandData(Point mouseCenterPosition, bool shouldReadInput)
		{
			PlayerInputButtons playerInputFlags = PlayerInputButtons.None;

			if (shouldReadInput)
			{
				if (!didReadInputLastFrame)
				{
					// If we didn't get input last frame but we are this frame, its a good chance the mouse is no longer centered which would cause a large look angle difference,
					// causing the player to loose their direction. To avoid this, we simply recenter the mouse right now so the player stays looking the same direction.
					Mouse.SetPosition(mouseCenterPosition.X, mouseCenterPosition.Y);
				}

				MouseState mouseState = Mouse.GetState();
				KeyboardState keyboardState = Keyboard.GetState();

				if (keyboardState.IsKeyDown(Keys.W)) { playerInputFlags |= PlayerInputButtons.MoveForward; }
				if (keyboardState.IsKeyDown(Keys.S)) { playerInputFlags |= PlayerInputButtons.MoveBackward; }
				if (keyboardState.IsKeyDown(Keys.A)) { playerInputFlags |= PlayerInputButtons.StrafeLeft; }
				if (keyboardState.IsKeyDown(Keys.D)) { playerInputFlags |= PlayerInputButtons.StrafeRight; }
				if (keyboardState.IsKeyDown(Keys.Space)) { playerInputFlags |= PlayerInputButtons.Jump; }
				if (keyboardState.IsKeyDown(Keys.LeftControl)) { playerInputFlags |= PlayerInputButtons.Crouch; }
				if (keyboardState.IsKeyDown(Keys.LeftShift)) { playerInputFlags |= PlayerInputButtons.Sprint; }
				if (mouseState.LeftButton == ButtonState.Pressed) { playerInputFlags |= PlayerInputButtons.Fire; }

				float deltaX = (mouseCenterPosition.X - mouseState.X) * 0.004f;
				float deltaY = (mouseCenterPosition.Y - mouseState.Y) * 0.004f;
				PlayerInput.lookAngles.X += deltaX;
				PlayerInput.lookAngles.Y = MathHelper.Clamp(PlayerInput.lookAngles.Y + deltaY, -MathHelper.Pi * 0.49f, MathHelper.Pi * 0.49f);
				Mouse.SetPosition(mouseCenterPosition.X, mouseCenterPosition.Y);
			}

			PlayerInput.didReadInputLastFrame = shouldReadInput;

			return new PlayerCommandData()
			{
				PlayerInput = playerInputFlags,
				LookAngles = PlayerInput.lookAngles,
			};
		}

		#endregion Methods
	}
}
