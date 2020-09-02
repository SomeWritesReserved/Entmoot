using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.Game.Sideswipe
{
	/// <summary>
	/// The player's input in terms of buttons being pressed (not keyboard keys but what they keyboard key results into, i.e. Space becomes Jump).
	/// </summary>
	[Flags]
	public enum PlayerInputButtons : short
	{
		None = 0,

		MoveLeft = (1 << 0),
		MoveRight = (1 << 1),
		Jump = (1 << 2),
		Grab = (1 << 3),
		Crouch = (1 << 4),
		Sprint = (1 << 5),
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
				if (keyboardState.IsKeyDown(Keys.W)) { playerInputFlags |= PlayerInputButtons.Grab; }
				if (keyboardState.IsKeyDown(Keys.S)) { playerInputFlags |= PlayerInputButtons.Crouch; }
				if (keyboardState.IsKeyDown(Keys.A)) { playerInputFlags |= PlayerInputButtons.MoveLeft; }
				if (keyboardState.IsKeyDown(Keys.D)) { playerInputFlags |= PlayerInputButtons.MoveRight; }
				if (keyboardState.IsKeyDown(Keys.Space)) { playerInputFlags |= PlayerInputButtons.Jump; }
				if (keyboardState.IsKeyDown(Keys.LeftShift)) { playerInputFlags |= PlayerInputButtons.Sprint; }
			}

			return new PlayerCommandData()
			{
				PlayerInput = playerInputFlags,
			};
		}

		#endregion Methods
	}
}
