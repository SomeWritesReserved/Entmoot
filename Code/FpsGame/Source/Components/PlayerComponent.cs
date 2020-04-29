using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	public struct PlayerComponent : IComponent<PlayerComponent>
	{
		#region Fields

		public Vector2 LookAngles;
		public Vector3 PlayerImpulse;

		#endregion Fields

		#region Methods

		public bool Equals(PlayerComponent other)
		{
			return this.LookAngles.Equals(other.LookAngles) &&
				this.PlayerImpulse.Equals(other.PlayerImpulse);
		}

		public void Interpolate(PlayerComponent otherA, PlayerComponent otherB, float amount)
		{
			// This component is only meant for client side data storage to be processed by PlayerMovementSystem,
			// thus it does not need replicated so nothing gets interpolated (client owned entities don't get interpolated on the client that owns them).
		}

		public void Serialize(IWriter writer)
		{
			// This component is only meant for client side data storage to be processed by PlayerMovementSystem,
			// thus it does not need replicated so nothing gets serialized/deserialized.
		}

		public void Deserialize(IReader reader)
		{
			// This component is only meant for client side data storage to be processed by PlayerMovementSystem,
			// thus it does not need replicated so nothing gets serialized/deserialized.
		}

		public void ResetToDefaults()
		{
			this.LookAngles = Vector2.Zero;
			this.PlayerImpulse = Vector3.Zero;
		}

		#endregion Methods
	}
}
