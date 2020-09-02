using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Sideswipe
{
	/// <summary>
	/// This defines what inputs a player can perform and how to apply those inputs to the player's commanding entity.
	/// </summary>
	public struct PlayerCommandData : ICommandData
	{
		#region Fields

		/// <summary>
		/// The amount of "oomphf" to give a player when the player moves left or right. Increasing this increases acceleration and speed.
		/// </summary>
		public const float MoveImpulseAccelerationAmount = 1000.0f;

		/// <summary>
		/// The amount of "oomphf" to give a player when the player moves left or right while sprinting. Increasing this increases acceleration and speed.
		/// </summary>
		public const float SprintImpulseAccelerationAmount = 2000.0f;

		/// <summary>
		/// The amount of "oomphf" to give a player when the player jumps. Increasing this increases acceleration and speed.
		/// </summary>
		public const float JumpImpulseAccelerationAmount = 5000.0f;

		/// <summary>
		/// The player's input in terms of buttons being pressed. This is what the player is trying to do this frame.
		/// </summary>
		public PlayerInputButtons PlayerInput;

		#endregion Fields

		#region Methods

		public void Serialize(IWriter writer)
		{
			writer.Write((short)this.PlayerInput);
		}

		public void Deserialize(IReader reader)
		{
			this.PlayerInput = (PlayerInputButtons)reader.ReadInt16();
		}

		#endregion Methods
	}
}
