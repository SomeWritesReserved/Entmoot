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
	public class RenderSystem : IClientSystem
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

		public void Update(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void UpdatePrediction(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void Render(EntityArray entityArray, Entity commandingEntity)
		{
			if (commandingEntity.IsValid && commandingEntity.HasComponent<SpatialComponent>())
			{
				ref SpatialComponent commandingSpatialComponent = ref commandingEntity.GetComponent<SpatialComponent>();
				this.BasicEffect.View = Matrix.CreateLookAt(commandingSpatialComponent.Position, commandingSpatialComponent.Position + Vector3.Transform(Vector3.Forward, commandingSpatialComponent.Rotation), Vector3.Up);

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
		}

		#endregion Methods
	}

	public class SpinnerSystem : IServerSystem
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

	public class PhysicsSystem : IServerSystem, IClientSystem
	{
		#region Methods

		public void Update(EntityArray entityArray)
		{
			foreach (Entity entity in entityArray)
			{
				this.updateEntity(entity);
			}
		}

		public void Update(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void UpdatePrediction(EntityArray entityArray, Entity commandingEntity)
		{
			this.updateEntity(commandingEntity);
		}

		public void Render(EntityArray entityArray, Entity commandingEntity)
		{
		}

		private void updateEntity(Entity entity)
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
