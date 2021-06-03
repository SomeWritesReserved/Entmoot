using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.Game.Fps
{
	/// <summary>
	/// The player's input in terms of buttons being pressed (not keyboard keys but what they keyboard key results into, i.e. Spacebar becomes Jump).
	/// </summary>
	[Flags]
	public enum PlayerInputButtons : short
	{
		None = 0,

		MoveForward = (1 << 0),
		MoveBackward = (1 << 1),
		StrafeLeft = (1 << 2),
		StrafeRight = (1 << 3),
		Jump = (1 << 4),
		Crouch = (1 << 5),
		Sprint = (1 << 6),
		Fire = (1 << 7),
	}

	/// <summary>
	/// A helper class for reading a player's input.
	/// </summary>
	public static class PlayerInput
	{
		#region Methods

		/// <summary>
		/// A method to populate a <see cref="PlayerCommandData"/> based on the player's mouse and keyboard input.
		/// This maps keyboard keys to <see cref="PlayerInputButtons"/>.
		/// </summary>
		public static PlayerCommandData GetPlayerInput(MouseState mouseState, KeyboardState keyboardState, bool shouldReadInput)
		{
			PlayerInputButtons playerInputFlags = PlayerInputButtons.None;

			if (shouldReadInput)
			{
				if (keyboardState.IsKeyDown(Keys.W)) { playerInputFlags |= PlayerInputButtons.MoveForward; }
				if (keyboardState.IsKeyDown(Keys.S)) { playerInputFlags |= PlayerInputButtons.MoveBackward; }
				if (keyboardState.IsKeyDown(Keys.A)) { playerInputFlags |= PlayerInputButtons.StrafeLeft; }
				if (keyboardState.IsKeyDown(Keys.D)) { playerInputFlags |= PlayerInputButtons.StrafeRight; }
				if (keyboardState.IsKeyDown(Keys.Space)) { playerInputFlags |= PlayerInputButtons.Jump; }
				if (keyboardState.IsKeyDown(Keys.LeftControl)) { playerInputFlags |= PlayerInputButtons.Crouch; }
				if (keyboardState.IsKeyDown(Keys.LeftShift)) { playerInputFlags |= PlayerInputButtons.Sprint; }
				if (mouseState.LeftButton == ButtonState.Pressed) { playerInputFlags |= PlayerInputButtons.Fire; }
			}

			return new PlayerCommandData()
			{
				PlayerInput = playerInputFlags,
			};
		}

		#endregion Methods
	}
}
