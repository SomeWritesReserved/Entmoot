using Entmoot.Engine;
using Entmoot.Framework.MonoGame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entmoot.Game.Fps
{
	/// <summary>
	/// The system that processes player input to move player entities around.
	/// </summary>
	public class PlayerMovementSystem : IServerSystem, IServerCommandProcessorSystem<PlayerCommandData>, IClientSystem, IClientPredictedSystem<PlayerCommandData>
	{
		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the server.
		/// </summary>
		public void ServerUpdate(EntityArray entityArray)
		{
			// Nothing to do, this system only operates on a player's client command via ProcessClientCommand
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client.
		/// </summary>
		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
			// Nothing to do, this system only operates on a player's client command via PredictClientCommand
		}

		/// <summary>
		/// Allows this system to perform any rendering.
		/// </summary>
		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
		}

		/// <summary>
		/// Updates this system for a specific client's command on a commanding entity. The provided lag compensated entity array might be null
		/// but otherwise contains the rough state of the server at the time the client issued the command (the client's render frame).
		/// </summary>
		public void ProcessClientCommand(EntityArray entityArray, Entity commandingEntity, PlayerCommandData commandData, EntityArray lagCompensatedEntityArray)
		{
			if (!commandingEntity.HasComponent<SpatialComponent>()) { return; }
			if (!commandingEntity.HasComponent<MovementComponent>()) { return; }

			ref SpatialComponent spatialComponent = ref commandingEntity.GetComponent<SpatialComponent>();
			ref MovementComponent movementComponent = ref commandingEntity.GetComponent<MovementComponent>();

			Vector3 playerImpulse = Vector3.Zero;
			if ((commandData.PlayerInput & PlayerInputButtons.MoveForward) != 0) { playerImpulse += Vector3.Forward; }
			if ((commandData.PlayerInput & PlayerInputButtons.MoveBackward) != 0) { playerImpulse += Vector3.Backward; }
			if ((commandData.PlayerInput & PlayerInputButtons.StrafeLeft) != 0) { playerImpulse += Vector3.Left; }
			if ((commandData.PlayerInput & PlayerInputButtons.StrafeRight) != 0) { playerImpulse += Vector3.Right; }

			if (playerImpulse != Vector3.Zero)
			{
				playerImpulse.Normalize();
				Quaternion lookMoveRotation = Quaternion.CreateFromYawPitchRoll(commandData.LookAngles.X, commandData.LookAngles.Y, 0);
				Vector3.Transform(ref playerImpulse, ref lookMoveRotation, out playerImpulse);
			}

			// [TODO]: this logic to apply friction and such needs to be done elsewhere? It should happen every frame, not just when clients send commands
			// but how do we still step players forward in movement when a command comes in? We can't just sum acceleration and apply it all at once.
			// However, a client will send commands for every frame so perhaps we do only do friction here?

			// Impulse force to move player
			{
				movementComponent.Velocity = (playerImpulse + movementComponent.Velocity) * 0.85f;
				spatialComponent.Position += movementComponent.Velocity;

				spatialComponent.Rotation = commandData.LookAngles;
			}
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client but only updates the commanding entity (for client-side prediction of a command).
		/// </summary>
		public void PredictClientCommand(EntityArray entityArray, Entity commandingEntity, PlayerCommandData commandData)
		{
			// Todo:
		}

		#endregion Methods
	}
}
