using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Zombtown
{
	public class CameraSystem : IServerSystem
	{
		#region Methods

		public void ServerUpdate(EntityArray entityArray)
		{
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<CameraComponent>() || !entity.HasComponent<SpatialComponent>()) { continue; }

				ref CameraComponent cameraComponent = ref entity.GetComponent<CameraComponent>();
				ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();

				cameraComponent.Position = spatialComponent.Position;
				cameraComponent.Extents = new Vector2(32, 24);
			}
		}

		#endregion Methods
	}
}
