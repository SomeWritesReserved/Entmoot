using System;
using System.Collections.Generic;
using System.Text;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Entmoot.Framework.MonoGame;

namespace Entmoot.Game.Sideswipe
{
	public class PhysicsSystem : IServerSystem, IServerCommandProcessorSystem<PlayerCommandData>, IClientSystem, IClientPredictedSystem<PlayerCommandData>
	{
		#region Fields

		/// <summary>
		/// This is a list of entities that are solid, which is calculated and cached per-frame. This is an optimization mechanism to avoid every entity checking every other entity for collisions.
		/// </summary>
		private readonly List<Entity> cachedSolidEntities = new List<Entity>(128);

		/// <summary>
		/// This is a list of entities that were already collided with by the main entity this frame. This will be cleared and reused every time a new entity starts to move.
		/// </summary>
		private readonly List<Entity> cachedAlreadyCollidedEntities = new List<Entity>(5);

		#endregion Fields

		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the server.
		/// </summary>
		public void ServerUpdate(EntityArray entityArray)
		{
			this.cachedSolidEntities.Clear();
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<SpatialComponent>()) { continue; }
				if (entity.GetComponent<SpatialComponent>().IsSolid) { this.cachedSolidEntities.Add(entity); }
			}

			foreach (Entity entity in entityArray)
			{
				this.runPhysicsOnEntity(entity);
			}
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client.
		/// </summary>
		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
			this.cachedSolidEntities.Clear();
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<SpatialComponent>()) { continue; }
				if (entity.GetComponent<SpatialComponent>().IsSolid) { this.cachedSolidEntities.Add(entity); }
			}
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
			this.runPhysicsOnEntity(commandingEntity);
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client but only updates the commanding entity (for client-side prediction of a command).
		/// </summary>
		public void PredictClientCommand(EntityArray entityArray, Entity commandingEntity, PlayerCommandData commandData)
		{
			this.ProcessClientCommand(entityArray, commandingEntity, commandData, null);
		}

		/// <summary>
		/// Performs motion and physics on a given entity, moving it through the world based on its acceleration and velocity.
		/// </summary>
		private void runPhysicsOnEntity(Entity entity)
		{
			if (!entity.HasComponent<SpatialComponent>()) { return; }
			if (!entity.HasComponent<PhysicsComponent>()) { return; }

			ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();
			ref PhysicsComponent physicsComponent = ref entity.GetComponent<PhysicsComponent>();

			// Gravity and acceleration
			physicsComponent.Acceleration += new Vector2(0, -0.5f);
			physicsComponent.Velocity += physicsComponent.Acceleration;
			Vector2 frameVelocity = physicsComponent.Velocity;

			bool isOnGround = false;

			// Collisions
			this.cachedAlreadyCollidedEntities.Clear();
			for (int i = 0; i < 2; i++)
			{
				foreach (Entity solidEntity in this.cachedSolidEntities)
				{
					if (this.cachedAlreadyCollidedEntities.Contains(solidEntity)) { continue; }

					SpatialComponent solidSpatialComponent = solidEntity.GetComponent<SpatialComponent>();
					if (Collision2D.CollideBoxBox(spatialComponent.Box2D, frameVelocity, solidSpatialComponent.Box2D, 0.00001f,
						out float collisionTime, out Vector2 collisionNormal, out Vector2 collisionEdge))
					{
						spatialComponent.Position += frameVelocity * collisionTime;
						frameVelocity *= (1.0f - collisionTime);
						frameVelocity = collisionEdge * Vector2.Dot(frameVelocity, collisionEdge);
						this.cachedAlreadyCollidedEntities.Add(solidEntity);
						isOnGround |= collisionNormal == Vector2.UnitY;
						break;
					}
				}
			}

			spatialComponent.Position += frameVelocity;
			physicsComponent.Velocity *= 0.95f;
			physicsComponent.Acceleration = Vector2.Zero;

			if (isOnGround)
			{
				physicsComponent.Velocity.Y = 0;
				physicsComponent.OffGroundCount = 0;
			}
			else
			{
				physicsComponent.OffGroundCount++;
			}
		}

		#endregion Methods
	}
}
