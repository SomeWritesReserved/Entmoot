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
	public class PlayerSystem : IServerSystem, IServerCommandProcessorSystem<PlayerCommandData>, IClientSystem, IClientPredictedSystem<PlayerCommandData>
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
		/// <remarks>
		/// Sideswipe's player commands are "additive impulse" commands, so we add velocities but never change position directly. This is because
		/// the <see cref="PhysicsSystem"/> will actually move the entity the player is controlling.
		/// </remarks>
		public void ProcessClientCommand(EntityArray entityArray, Entity commandingEntity, PlayerCommandData commandData, EntityArray lagCompensatedEntityArray)
		{
			if (!commandingEntity.HasComponent<SpatialComponent>()) { return; }
			if (!commandingEntity.HasComponent<MovementComponent>()) { return; }

			ref SpatialComponent spatialComponent = ref commandingEntity.GetComponent<SpatialComponent>();
			ref MovementComponent movementComponent = ref commandingEntity.GetComponent<MovementComponent>();

			Vector3 movement = Vector3.Zero;
			if ((commandData.PlayerInput & PlayerInputButtons.MoveForward) != 0) { movement += Vector3.Forward; }
			if ((commandData.PlayerInput & PlayerInputButtons.MoveBackward) != 0) { movement += Vector3.Backward; }
			if ((commandData.PlayerInput & PlayerInputButtons.StrafeLeft) != 0) { movement += Vector3.Left; }
			if ((commandData.PlayerInput & PlayerInputButtons.StrafeRight) != 0) { movement += Vector3.Right; }

			if (movement != Vector3.Zero)
			{
				movement.Normalize();
				//Quaternion lookMoveRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, commandData.LookAngles.X);
				//Vector3.Transform(ref movement, ref lookMoveRotation, out movement);
			}

			Vector3 frameVelocity = movement;

			// Collisions
			this.cachedAlreadyCollidedEntities.Clear();
			for (int i = 0; i < 3; i++)
			{
				foreach (Entity solidEntity in this.cachedSolidEntities)
				{
					if (this.cachedAlreadyCollidedEntities.Contains(solidEntity)) { continue; }

					SpatialComponent solidSpatialComponent = solidEntity.GetComponent<SpatialComponent>();
					if (Collision3D.CollideBoxBox(spatialComponent.Box2D, frameVelocity, solidSpatialComponent.Box2D, 0.00001f,
						out float collisionTime, out Vector3 collisionNormal, out Vector3 collisionEdgeUp, out Vector3 collisionEdgeSide))
					{
						spatialComponent.Position += frameVelocity * collisionTime;
						frameVelocity *= (1.0f - collisionTime);
						frameVelocity = collisionEdgeSide * Vector3.Dot(frameVelocity, collisionEdgeSide);
						this.cachedAlreadyCollidedEntities.Add(solidEntity);

						// If we hit something we need to clip the real velocity (not just the frame's velocity)
						// to make sure the object doesn't try to continue in that direction anymore (we could even
						// bounce here if there is restitution).
						//physicsComponent.Velocity = collisionEdgeSide * Vector3.Dot(physicsComponent.Velocity, collisionEdgeSide);

						break;
					}
				}
			}

			spatialComponent.Position = frameVelocity;
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
