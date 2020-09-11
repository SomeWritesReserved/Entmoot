using System;
using System.Collections.Generic;
using System.Text;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

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

			physicsComponent.Acceleration += new Vector2(0, -50.0f);

			this.cachedAlreadyCollidedEntities.Clear();
			Vector2 newPosition = spatialComponent.Position;
			Vector2 newVelocity = physicsComponent.Velocity;
			for (int i = 0; i < 5; i++)
			{
				if (newVelocity == Vector2.Zero) { break; }
				if (!this.tryMoveAndCollide(entity, newPosition, spatialComponent.Extents, newVelocity, out newPosition, out newVelocity))
				{
					break;
				}
			}
			spatialComponent.Position = newPosition;

			physicsComponent.Velocity = newVelocity + physicsComponent.Acceleration;
			physicsComponent.Velocity *= 0.9f;
			physicsComponent.Acceleration = Vector2.Zero;
		}

		private bool tryMoveAndCollide(Entity movingEntity, Vector2 position, Vector2 extents, Vector2 velocity, out Vector2 newPosition, out Vector2 newVelocity)
		{
			foreach (Entity solidEntity in this.cachedSolidEntities)
			{
				if (solidEntity.ID == movingEntity.ID) { continue; }
				if (!solidEntity.HasComponent<SpatialComponent>()) { continue; }
				if (this.cachedAlreadyCollidedEntities.Contains(solidEntity)) { continue; }

				ref SpatialComponent spatialComponent = ref solidEntity.GetComponent<SpatialComponent>();

				if (this.checkCollide(position, extents, velocity, spatialComponent.Position, spatialComponent.Extents, Vector2.Zero,
					out float firstCollideTime, out Vector2 normal, out Vector2 tangent))
				{
					if (firstCollideTime < 0.0001f) { firstCollideTime = 0.0f; }
					this.cachedAlreadyCollidedEntities.Add(solidEntity);
					newPosition = position + (velocity * firstCollideTime - normal * 0.001f);
					newVelocity = Vector2.Dot(velocity, tangent) * tangent;
					return true;
				}
			}

			newPosition = position + velocity;
			newVelocity = velocity;
			return false;
		}

		private bool checkCollide(Vector2 positionA, Vector2 extentsA, Vector2 velocityA,
			Vector2 positionB, Vector2 extentsB, Vector2 velocityB,
			out float firstCollideTime, out Vector2 normal, out Vector2 tangent)
		{
			normal = Vector2.Zero;
			tangent = Vector2.Zero;

			if (this.checkOverlap(positionA, extentsA, positionB, extentsB))
			{
				firstCollideTime = 0.0f;
				return true;
			}

			// Treat 'a' as stationary and 'b' as the only one moving
			Vector2 relativeVelocity = velocityB - velocityA;

			firstCollideTime = 0.0f;
			float lastCollideTime = 1.0f;

			if (relativeVelocity.X < 0.0f)
			{
				if ((positionB.X + extentsB.X * 0.5f) < (positionA.X - extentsA.X * 0.5f)) { return false; }
				if ((positionA.X + extentsA.X * 0.5f) < (positionB.X - extentsB.X * 0.5f))
				{
					float thisCollideTime = ((positionA.X + extentsA.X * 0.5f) - (positionB.X - extentsB.X * 0.5f)) / relativeVelocity.X;
					if (thisCollideTime > firstCollideTime)
					{
						firstCollideTime = thisCollideTime;
						normal = Vector2.UnitX;
						tangent = Vector2.UnitY;
					}
				}
				if ((positionB.X + extentsB.X * 0.5f) > (positionA.X - extentsA.X * 0.5f)) { lastCollideTime = Math.Min(((positionA.X - extentsA.X * 0.5f) - (positionB.X + extentsB.X * 0.5f)) / relativeVelocity.X, lastCollideTime); }
			}
			if (relativeVelocity.X > 0.0f)
			{
				if ((positionB.X - extentsB.X * 0.5f) > (positionA.X + extentsA.X * 0.5f)) { return false; }
				if ((positionB.X + extentsB.X * 0.5f) < (positionA.X - extentsA.X * 0.5f))
				{
					float thisCollideTime = ((positionA.X - extentsA.X * 0.5f) - (positionB.X + extentsB.X * 0.5f)) / relativeVelocity.X;
					if (thisCollideTime > firstCollideTime)
					{
						firstCollideTime = thisCollideTime;
						normal = -Vector2.UnitX;
						tangent = Vector2.UnitY;
					}
				}
				if ((positionA.X + extentsA.X * 0.5f) > (positionB.X - extentsB.X * 0.5f)) { lastCollideTime = Math.Min(((positionA.X + extentsA.X * 0.5f) - (positionB.X - extentsB.X * 0.5f)) / relativeVelocity.X, lastCollideTime); }
			}
			if (firstCollideTime > lastCollideTime) { return false; }

			if (relativeVelocity.Y < 0.0f)
			{
				if ((positionB.Y + extentsB.Y * 0.5f) < (positionA.Y - extentsA.Y * 0.5f)) { return false; }
				if ((positionA.Y + extentsA.Y * 0.5f) < (positionB.Y - extentsB.Y * 0.5f))
				{
					float thisCollideTime = ((positionA.Y + extentsA.Y * 0.5f) - (positionB.Y - extentsB.Y * 0.5f)) / relativeVelocity.Y;
					if (thisCollideTime > firstCollideTime)
					{
						firstCollideTime = thisCollideTime;
						normal = Vector2.UnitY;
						tangent = Vector2.UnitX;
					}
				}
				if ((positionB.Y + extentsB.Y * 0.5f) > (positionA.Y - extentsA.Y * 0.5f)) { lastCollideTime = Math.Min(((positionA.Y - extentsA.Y * 0.5f) - (positionB.Y + extentsB.Y * 0.5f)) / relativeVelocity.Y, lastCollideTime); }
			}
			if (relativeVelocity.Y > 0.0f)
			{
				if ((positionB.Y - extentsB.Y * 0.5f) > (positionA.Y + extentsA.Y * 0.5f)) { return false; }
				if ((positionB.Y + extentsB.Y * 0.5f) < (positionA.Y - extentsA.Y * 0.5f))
				{
					float thisCollideTime = ((positionA.Y - extentsA.Y * 0.5f) - (positionB.Y + extentsB.Y * 0.5f)) / relativeVelocity.Y;
					if (thisCollideTime > firstCollideTime)
					{
						firstCollideTime = thisCollideTime;
						normal = -Vector2.UnitY;
						tangent = Vector2.UnitX;
					}
				}
				if ((positionA.Y + extentsA.Y * 0.5f) > (positionB.Y - extentsB.Y * 0.5f)) { lastCollideTime = Math.Min(((positionA.Y + extentsA.Y * 0.5f) - (positionB.Y - extentsB.Y * 0.5f)) / relativeVelocity.Y, lastCollideTime); }
			}
			if (firstCollideTime > lastCollideTime) { return false; }

			return true;
		}

		private bool checkOverlap(Vector2 positionA, Vector2 extentsA,
			Vector2 positionB, Vector2 extentsB)
		{
			if ((positionA.X + extentsA.X * 0.5f) < (positionB.X - extentsB.X * 0.5f) || (positionA.X - extentsA.X * 0.5f) > (positionB.X + extentsB.X * 0.5f)) { return false; }
			if ((positionA.Y + extentsA.Y * 0.5f) < (positionB.Y - extentsB.Y * 0.5f) || (positionA.Y - extentsA.Y * 0.5f) > (positionB.Y + extentsB.Y * 0.5f)) { return false; }
			return true;
		}

		#endregion Methods
	}
}
