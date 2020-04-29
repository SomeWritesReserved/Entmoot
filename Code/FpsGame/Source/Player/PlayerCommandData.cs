using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	[Flags]
	public enum PlayerCommandFlags : short
	{
		None = 0,
		MoveForward = 1,
		MoveBackward = 2,
		MoveLeft = 4,
		MoveRight = 8,
		Jump = 16,
	}

	public struct PlayerCommandData : ICommandData
	{
		#region Fields

		public PlayerCommandFlags PlayerCommandFlags;
		public Vector2 LookAngles;

		#endregion Fields

		#region Methods

		public void Serialize(IWriter writer)
		{
			writer.Write((short)this.PlayerCommandFlags);
			writer.Write(this.LookAngles.X);
			writer.Write(this.LookAngles.Y);
		}

		public void Deserialize(IReader reader)
		{
			this.PlayerCommandFlags = (PlayerCommandFlags)reader.ReadInt16();
			this.LookAngles.X = reader.ReadSingle();
			this.LookAngles.Y = reader.ReadSingle();
		}

		public void ApplyToEntity(Entity entity)
		{
			if (!entity.HasComponent<PlayerComponent>()) { return; }

			ref PlayerComponent playerComponent = ref entity.GetComponent<PlayerComponent>();
			
			// We don't care about incremental/intermediate look angles, we only care to store the most recent one so just stomp on whatever was there previously
			playerComponent.LookAngles = this.LookAngles;

			Vector3 playerImpulse = Vector3.Zero;


			// Make sure to add to the component's player movement vector to handle multiple successive commands worth of moves
			playerComponent.PlayerImpulse += playerImpulse;
		}

		#endregion Methods
	}
}
