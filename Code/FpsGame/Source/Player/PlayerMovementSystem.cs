using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;

namespace Entmoot.FpsGame
{
	public class PlayerMovementSystem : IServerSystem, IClientSystem
	{
		#region Methods

		public void ServerUpdate(EntityArray entityArray)
		{
		}

		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientPrediction(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
			// Nothing
		}

		private void movePlayer(Entity playerEntity, PlayerComponent playerComponent, SpatialComponent spatialComponent)
		{
		}

		#endregion Methods
	}
}
