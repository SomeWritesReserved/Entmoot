using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Fps
{
	/// <summary>
	/// This defines what inputs a player can perform and how to apply those inputs to the player's commanding entity.
	/// </summary>
	public struct PlayerCommandData : ICommandData
	{
		#region Fields

		/// <summary>
		/// The player's input in terms of buttons being pressed. This is what the player is trying to do this frame.
		/// </summary>
		public PlayerInputButtons PlayerInput;

		/// <summary>
		/// The player's desired look direction in yaw and pitch angles. These are absolute angles, not relative angles since last frame. This is where the player is looking this frame.
		/// </summary>
		public Vector2 LookAngles;

		#endregion Fields

		#region Methods

		public void Serialize(IWriter writer)
		{
			writer.Write((short)this.PlayerInput);
			writer.Write(this.LookAngles.X);
			writer.Write(this.LookAngles.Y);
		}

		public void Deserialize(IReader reader)
		{
			this.PlayerInput = (PlayerInputButtons)reader.ReadInt16();
			this.LookAngles.X = reader.ReadSingle();
			this.LookAngles.Y = reader.ReadSingle();
		}

		#endregion Methods
	}
}
