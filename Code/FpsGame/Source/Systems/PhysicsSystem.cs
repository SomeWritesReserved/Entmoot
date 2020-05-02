using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	public class PhysicsSystem : IServerSystem, IClientSystem
	{
		#region Methods

		public void ServerUpdate(EntityArray entityArray)
		{
			foreach (Entity entity in entityArray)
			{
				this.updatePhysicsForEntity(entity);
			}
		}

		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientPrediction(EntityArray entityArray, Entity commandingEntity)
		{
			this.updatePhysicsForEntity(commandingEntity);
		}

		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
		}

		private void updatePhysicsForEntity(Entity entity)
		{
			if (!entity.HasComponent<SpatialComponent>()) { return; }
			if (!entity.HasComponent<PhysicsComponent>()) { return; }

			ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();
			ref PhysicsComponent physicsComponent = ref entity.GetComponent<PhysicsComponent>();

			float elapsedTime = (1.0f / 60.0f);
			spatialComponent.Position += (physicsComponent.Velocity * elapsedTime) + (physicsComponent.Acceleration * elapsedTime * elapsedTime / 2);
			physicsComponent.Velocity += physicsComponent.Acceleration * elapsedTime;
			physicsComponent.Velocity *= 0.9f;
			physicsComponent.Acceleration = Vector3.Zero;
		}

		#endregion Methods
	}
}
