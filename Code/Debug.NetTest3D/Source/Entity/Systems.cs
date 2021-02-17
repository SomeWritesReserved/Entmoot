using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Entmoot.Framework.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.Debug.NetTest3D
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

					Renderer3D.RenderBox(this.GraphicsDeviceManager.GraphicsDevice, this.BasicEffect, spatialComponent.Position, spatialComponent.Rotation);
				}
			}
		}

		#endregion Methods
	}

	public class SpinnerSystem : IServerSystem
	{
		#region Fields

		private int tick;
		private Entity newEntity;

		#endregion Fields

		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the server.
		/// </summary>
		public void ServerUpdate(EntityArray entityArray)
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

	public class ClientCommandSystem : IServerSystem, IServerCommandProcessorSystem<CommandData>, IClientSystem, IClientPredictedSystem<CommandData>
	{
		#region Methods

		public void ServerUpdate(EntityArray entityArray)
		{
		}

		public void ProcessClientCommand(EntityArray entityArray, Entity commandingEntity, CommandData commandData, EntityArray lagCompensatedEntityArray)
		{
			if (!commandingEntity.HasComponent<SpatialComponent>()) { return; }
			if (!commandingEntity.HasComponent<PhysicsComponent>()) { return; }

			Vector3 movement = Vector3.Zero;
			if ((commandData.Commands & Commands.MoveForward) != 0) { movement += Vector3.Forward; }
			if ((commandData.Commands & Commands.MoveBackward) != 0) { movement += Vector3.Backward; }
			if ((commandData.Commands & Commands.MoveLeft) != 0) { movement += Vector3.Left; }
			if ((commandData.Commands & Commands.MoveRight) != 0) { movement += Vector3.Right; }

			if (movement != Vector3.Zero)
			{
				movement.Normalize();
				Quaternion lookMoveRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, commandData.LookAngles.X);
				Vector3.Transform(ref movement, ref lookMoveRotation, out movement);
			}

			ref SpatialComponent spatialComponent = ref commandingEntity.GetComponent<SpatialComponent>();
			ref PhysicsComponent physicsComponent = ref commandingEntity.GetComponent<PhysicsComponent>();
			physicsComponent.Acceleration += movement * CommandData.MoveImpulse;
			spatialComponent.Rotation = Quaternion.CreateFromYawPitchRoll(commandData.LookAngles.X, commandData.LookAngles.Y, 0.0f);
		}

		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void PredictClientCommand(EntityArray entityArray, Entity commandingEntity, CommandData commandData)
		{
			this.ProcessClientCommand(entityArray, commandingEntity, commandData, null);
		}

		#endregion Methods
	}

	public class PhysicsSystem : IServerSystem, IClientSystem, IClientPredictedSystem<CommandData>
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
		/// Runs this system over the given array of entities on the client but only updates the commanding entity (for client-side prediction of a command).
		/// </summary>
		public void PredictClientCommand(EntityArray entityArray, Entity commandingEntity, CommandData commandData)
		{
			this.runPhysicsOnEntity(commandingEntity);
		}
		
		/// <summary>
		/// Allows this system to perform any rendering.
		/// </summary>
		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
		}

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
			physicsComponent.Acceleration = Vector3.Zero;
		}

		#endregion Methods
	}
}
