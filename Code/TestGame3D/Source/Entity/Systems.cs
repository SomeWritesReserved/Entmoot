﻿using System;
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
				ref ColorComponent colorComponent = ref entity.GetComponent<ColorComponent>();

				this.BasicEffect.AmbientLightColor = Vector3.One * 0.25f;
				this.BasicEffect.DiffuseColor = Color.White.ToVector3();
				if (entity.HasComponent<ColorComponent>())
				{
					this.BasicEffect.DiffuseColor = colorComponent.Color.ToVector3();
				}

				ShapeRenderHelper.RenderBox(this.GraphicsDeviceManager.GraphicsDevice, this.BasicEffect, spatialComponent.Position, spatialComponent.Rotation);
			}
		}

		#endregion Methods
	}

	public class SpinnerSystem : ISystem
	{
		#region Methods

		private int tick;
		private Entity newEntity;
		public void Update(EntityArray entityArray)
		{
			this.tick++;
			if (this.tick == 400)
			{
				entityArray.TryCreateEntity(out this.newEntity);
				this.newEntity.AddComponent<SpatialComponent>().Position = new Vector3(10, 10, 10);
				this.newEntity.AddComponent<SpatialComponent>().Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, 0.707f);
			}
			else if (this.tick == 600)
			{
				this.newEntity.AddComponent<ColorComponent>().Color = new Color(1.0f, 0, 0);
			}

			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<SpatialComponent>()) { continue; }

				ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();

				if (entity.ID == 12)
				{
					spatialComponent.Rotation *= Quaternion.CreateFromYawPitchRoll(0.034f, 0, 0);
				}
				else if (entity.ID == 11)
				{
					spatialComponent.Rotation *= Quaternion.CreateFromYawPitchRoll(0, 0, 0.02f);
				}
			}
		}

		#endregion Methods
	}
}
