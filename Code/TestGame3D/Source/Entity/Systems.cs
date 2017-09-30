using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.TestGame3D
{
	public class RenderSystem : ISystem
	{
		#region Constructors

		public RenderSystem(GraphicsDeviceManager graphicsDeviceManager)
		{
			this.GraphicsDeviceManager = graphicsDeviceManager;
		}

		#endregion Constructors

		#region Properties

		public GraphicsDeviceManager GraphicsDeviceManager { get; }

		public BasicEffect BasicEffect { get; set; }

		#endregion Properties

		#region Methods

		public void Update(EntityArray entityArray)
		{
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<SpatialComponent>()) { continue; }

				ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();

				this.BasicEffect.AmbientLightColor = Vector3.One * 0.25f;
				this.BasicEffect.DiffuseColor = entity.ID < 4 ? new Vector3(0.5f, 0.5f, 1.0f) : Vector3.One;
				if (entity.ID == 10 || entity.ID == 9)
				{
					this.BasicEffect.DiffuseColor = new Vector3(1.0f, 0.5f, 0.5f);
				}

				ShapeRenderHelper.RenderBox(this.GraphicsDeviceManager.GraphicsDevice, this.BasicEffect, spatialComponent.Position, spatialComponent.Rotation);
			}
		}

		#endregion Methods
	}

	public class SpinnerSystem : ISystem
	{
		#region Methods

		public void Update(EntityArray entityArray)
		{
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<SpatialComponent>()) { continue; }

				ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();

				if (entity.ID == 10)
				{
					spatialComponent.Rotation *= Quaternion.CreateFromYawPitchRoll(0.034f, 0, 0);
				}
				else if (entity.ID == 9)
				{
					spatialComponent.Rotation *= Quaternion.CreateFromYawPitchRoll(0, 0, 0.02f);
				}
			}
		}

		#endregion Methods
	}
}
