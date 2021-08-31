using System;
using System.Collections.Generic;
using System.Text;

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
}
