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

			float elapsedTime = (1.0f / 60.0f);

			physicsComponent.Acceleration += new Vector2(0, -1000.0f);
			Vector2 frameVelocity = (physicsComponent.Velocity * elapsedTime) + (physicsComponent.Acceleration * elapsedTime * elapsedTime / 2);
			spatialComponent.Position = this.tryMoveAndCollide(spatialComponent.Position, spatialComponent.Extents, frameVelocity);

			physicsComponent.Velocity += physicsComponent.Acceleration * elapsedTime;
			physicsComponent.Velocity *= 0.9f;
			physicsComponent.Acceleration = Vector2.Zero;
		}

		private Vector2 tryMoveAndCollide(Vector2 position, Vector2 extents, Vector2 velocity)
		{
			foreach (Entity solidEntity in this.cachedSolidEntities)
			{
				if (!solidEntity.HasComponent<SpatialComponent>()) { continue; }

				ref SpatialComponent spatialComponent = ref solidEntity.GetComponent<SpatialComponent>();

				if (this.checkCollide(position, extents, velocity, spatialComponent.Position, spatialComponent.Extents, Vector2.Zero, out float firstCollideTime, out float lastCollidTime))
				{
					return position + velocity * (firstCollideTime - 0.001f);
				}
			}

			return position + velocity;
		}

		private bool checkCollide(Vector2 positionA, Vector2 extentsA, Vector2 velocityA,
			Vector2 positionB, Vector2 extentsB, Vector2 velocityB,
			out float firstCollideTime, out float lastCollideTime)
		{
			if (this.checkOverlap(positionA, extentsA, positionB, extentsB))
			{
				firstCollideTime = 0.0f;
				lastCollideTime = 0.0f;
				return true;
			}

			// Treat 'a' as stationary and 'b' as the only one moving
			Vector2 relativeVelocity = velocityB - velocityA;

			firstCollideTime = 0.0f;
			lastCollideTime = 1.0f;

			if (relativeVelocity.X < 0.0f)
			{
				if ((positionB.X + extentsB.X * 0.5f) < (positionA.X - extentsA.X * 0.5f)) { return false; }
				if ((positionA.X + extentsA.X * 0.5f) < (positionB.X - extentsB.X * 0.5f)) { firstCollideTime = Math.Max(((positionA.X + extentsA.X * 0.5f) - (positionB.X - extentsB.X * 0.5f)) / relativeVelocity.X, firstCollideTime); }
				if ((positionB.X + extentsB.X * 0.5f) > (positionA.X - extentsA.X * 0.5f)) { lastCollideTime = Math.Min(((positionA.X - extentsA.X * 0.5f) - (positionB.X + extentsB.X * 0.5f)) / relativeVelocity.X, lastCollideTime); }
			}
			if (relativeVelocity.X > 0.0f)
			{
				if ((positionB.X - extentsB.X * 0.5f) > (positionA.X + extentsA.X * 0.5f)) { return false; }
				if ((positionB.X + extentsB.X * 0.5f) < (positionA.X - extentsA.X * 0.5f)) { firstCollideTime = Math.Max(((positionA.X - extentsA.X * 0.5f) - (positionB.X + extentsB.X * 0.5f)) / relativeVelocity.X, firstCollideTime); }
				if ((positionA.X + extentsA.X * 0.5f) > (positionB.X - extentsB.X * 0.5f)) { lastCollideTime = Math.Min(((positionA.X + extentsA.X * 0.5f) - (positionB.X - extentsB.X * 0.5f)) / relativeVelocity.X, lastCollideTime); }
			}
			if (firstCollideTime > lastCollideTime) { return false; }

			if (relativeVelocity.Y < 0.0f)
			{
				if ((positionB.Y + extentsB.Y * 0.5f) < (positionA.Y - extentsA.Y * 0.5f)) { return false; }
				if ((positionA.Y + extentsA.Y * 0.5f) < (positionB.Y - extentsB.Y * 0.5f)) { firstCollideTime = Math.Max(((positionA.Y + extentsA.Y * 0.5f) - (positionB.Y - extentsB.Y * 0.5f)) / relativeVelocity.Y, firstCollideTime); }
				if ((positionB.Y + extentsB.Y * 0.5f) > (positionA.Y - extentsA.Y * 0.5f)) { lastCollideTime = Math.Min(((positionA.Y - extentsA.Y * 0.5f) - (positionB.Y + extentsB.Y * 0.5f)) / relativeVelocity.Y, lastCollideTime); }
			}
			if (relativeVelocity.Y > 0.0f)
			{
				if ((positionB.Y - extentsB.Y * 0.5f) > (positionA.Y + extentsA.Y * 0.5f)) { return false; }
				if ((positionB.Y + extentsB.Y * 0.5f) < (positionA.Y - extentsA.Y * 0.5f)) { firstCollideTime = Math.Max(((positionA.Y - extentsA.Y * 0.5f) - (positionB.Y + extentsB.Y * 0.5f)) / relativeVelocity.Y, firstCollideTime); }
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
