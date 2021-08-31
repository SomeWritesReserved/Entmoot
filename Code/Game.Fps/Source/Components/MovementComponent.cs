﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Fps
{
	/// <summary>
	/// Represents an entity's ability to move around in the world.
	/// </summary>
	public struct MovementComponent : IComponent<MovementComponent>
	{
		#region Fields

		/// <summary>
		/// The velocity of this entity in the world, with the center of travel being the entity's position.
		/// </summary>
		public Vector3 Velocity;

		/// <summary>
		/// The acceleration of this entity in the world, with the center of travel being the entity's position.
		/// </summary>
		//public Vector3 Acceleration;

		#endregion Fields

		#region Methods

		public bool Equals(MovementComponent other)
		{
			return this.Velocity.Equals(other.Velocity);
		}

		public void Interpolate(MovementComponent otherA, MovementComponent otherB, float amount)
		{
			// This component is not networked, clients need not know about it, the positions of entities will always be interpolated, never extrapolated based on velocity
		}

		public void Serialize(IWriter writer)
		{
			// This component is not networked, clients need not know about it, the positions of entities will always be interpolated, never extrapolated based on velocity
			// [TODO]: if this isn't networked, then clients will never get a correct velocity/acceleration from the server which means their prediction could be forever wrong
		}

		public void Deserialize(IReader reader)
		{
			// This component is not networked, clients need not know about it, the positions of entities will always be interpolated, never extrapolated based on velocity
		}

		public void ResetToDefaults()
		{
			this.Velocity = Vector3.Zero;
		}

		#endregion Methods
	}
}
