using Entmoot.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entmoot.Game.Sideswipe
{
	public class PlayerInputSystem : IServerSystem, IServerCommandProcessorSystem<PlayerCommandData>, IClientSystem, IClientPredictedSystem<PlayerCommandData>
	{
		#region Fields

		/// <summary>
		/// The amount of "oomphf" to give a player when the player moves left or right. Increasing this increases acceleration and speed.
		/// </summary>
		public const float MoveImpulseAccelerationAmount = 50.0f;

		/// <summary>
		/// The amount of "oomphf" to give a player when the player moves left or right while sprinting. Increasing this increases acceleration and speed.
		/// </summary>
		public const float SprintImpulseAccelerationAmount = 100.0f;

		/// <summary>
		/// The amount of "oomphf" to give a player when the player jumps. Increasing this increases acceleration and speed.
		/// </summary>
		public const float JumpImpulseAccelerationAmount = 2500.0f;

		#endregion Fields

		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the server.
		/// </summary>
		public void ServerUpdate(EntityArray entityArray)
		{
			// Nothing to do, this system only operates on a player's client command
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client.
		/// </summary>
		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
			// Nothing to do, this system only operates on a player's client command
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
		/// <remarks>
		/// Sideswipe's player commands are "additive impulse" commands, so we add velocities but never change position directly. This is because
		/// the <see cref="PhysicsSystem"/> will actually move the entity the player is controlling.
		/// </remarks>
		public void ProcessClientCommand(EntityArray entityArray, Entity commandingEntity, PlayerCommandData commandData, EntityArray lagCompensatedEntityArray)
		{
			if (!commandingEntity.HasComponent<SpatialComponent>()) { return; }
			if (!commandingEntity.HasComponent<PhysicsComponent>()) { return; }

			ref SpatialComponent spatialComponent = ref commandingEntity.GetComponent<SpatialComponent>();
			ref PhysicsComponent physicsComponent = ref commandingEntity.GetComponent<PhysicsComponent>();

			Vector2 moveAcceleration = Vector2.Zero;
			if ((commandData.PlayerInput & PlayerInputButtons.MoveLeft) == PlayerInputButtons.MoveLeft) { moveAcceleration -= Vector2.UnitX; }
			if ((commandData.PlayerInput & PlayerInputButtons.MoveRight) == PlayerInputButtons.MoveRight) { moveAcceleration += Vector2.UnitX; }

			float targetRotation = Vector2.Dot(Vector2.UnitX, moveAcceleration) * 0.1f;
			spatialComponent.Rotation = MathHelper.Lerp(spatialComponent.Rotation, targetRotation, 0.2f);

			if ((commandData.PlayerInput & PlayerInputButtons.Sprint) == PlayerInputButtons.Sprint)
			{
				moveAcceleration *= PlayerInputSystem.SprintImpulseAccelerationAmount;
			}
			else
			{
				moveAcceleration *= PlayerInputSystem.MoveImpulseAccelerationAmount;
			}

			Vector2 jumpAcceleration = Vector2.Zero;
			if ((commandData.PlayerInput & PlayerInputButtons.Jump) == PlayerInputButtons.Jump && physicsComponent.IsOnGround) { jumpAcceleration += Vector2.UnitY; }
			jumpAcceleration *= PlayerInputSystem.JumpImpulseAccelerationAmount;

			Vector2 totalAcceleration = moveAcceleration + jumpAcceleration;

			physicsComponent.Acceleration += totalAcceleration;
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client but only updates the commanding entity (for client-side prediction of a command).
		/// </summary>
		public void PredictClientCommand(EntityArray entityArray, Entity commandingEntity, PlayerCommandData commandData)
		{
			this.ProcessClientCommand(entityArray, commandingEntity, commandData, null);
		}

		#endregion Methods
	}
}
