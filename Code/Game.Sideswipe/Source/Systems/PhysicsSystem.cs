using System;
using System.Collections.Generic;
using System.Text;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Sideswipe
{
	public class PhysicsSystem : IServerSystem, IServerCommandProcessorSystem<PlayerCommandData>, IClientSystem, IClientPredictedSystem<PlayerCommandData>
	{
		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the server.
		/// </summary>
		public void ServerUpdate(EntityArray entityArray)
		{
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
			spatialComponent.Position += (physicsComponent.Velocity * elapsedTime) + (physicsComponent.Acceleration * elapsedTime * elapsedTime / 2);
			physicsComponent.Velocity += physicsComponent.Acceleration * elapsedTime;
			physicsComponent.Velocity *= 0.9f;
			physicsComponent.Acceleration = Vector2.Zero;
		}

		#endregion Methods
	}
}
