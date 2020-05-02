using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.FpsGame
{
	[Flags]
	public enum PlayerInputFlags : short
	{
		None = 0,
		MoveForward = 1,
		MoveBackward = 2,
		MoveLeft = 4,
		MoveRight = 8,
		Jump = 16,
	}

	public static class PlayerInput
	{
		public static void GetPlayerInput(MouseState mouseState, KeyboardState keyboardState, Point mouseLookOrigin, Vector2 mouseSensitivity, ref PlayerCommandData playerCommandData, bool shouldReadInput)
		{
			PlayerInputFlags playerInputFlags = PlayerInputFlags.None;
			Vector2 lookAngles = playerCommandData.LookAngles;

			if (shouldReadInput)
			{
				if (keyboardState.IsKeyDown(Keys.W)) { playerInputFlags |= PlayerInputFlags.MoveForward; }
				if (keyboardState.IsKeyDown(Keys.S)) { playerInputFlags |= PlayerInputFlags.MoveBackward; }
				if (keyboardState.IsKeyDown(Keys.A)) { playerInputFlags |= PlayerInputFlags.MoveLeft; }
				if (keyboardState.IsKeyDown(Keys.D)) { playerInputFlags |= PlayerInputFlags.MoveRight; }
				if (keyboardState.IsKeyDown(Keys.Space)) { playerInputFlags |= PlayerInputFlags.Jump; }

				float lookDeltaX = (mouseLookOrigin.X - mouseState.X) * mouseSensitivity.X;
				float lookDeltaY = (mouseLookOrigin.Y - mouseState.Y) * mouseSensitivity.Y;
				lookAngles.X = lookAngles.X + lookDeltaX;
				lookAngles.Y = MathHelper.Clamp(lookAngles.Y + lookDeltaY, -MathHelper.Pi * 0.49f, MathHelper.Pi * 0.49f);
				Mouse.SetPosition(mouseLookOrigin.X, mouseLookOrigin.Y);
			}

			playerCommandData.PlayerInput = playerInputFlags;
			playerCommandData.LookAngles = lookAngles;
		}
	}
}
