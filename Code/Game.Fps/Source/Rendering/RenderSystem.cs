using System;
using System.Collections.Generic;
using System.Text;
using Entmoot.Engine;
using Entmoot.Framework.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.Game.Fps
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
				Vector3 eyePosition = commandingSpatialComponent.Position + new Vector3(0, commandingSpatialComponent.Extents.Y * 0.92f, 0);
				this.BasicEffect.View = Matrix.CreateLookAt(eyePosition,
					eyePosition + Vector3.Transform(Vector3.Forward, Quaternion.CreateFromYawPitchRoll(commandingSpatialComponent.Rotation.X, commandingSpatialComponent.Rotation.Y, 0)),
					Vector3.Up);

				foreach (Entity entity in entityArray)
				{
					if (!entity.HasComponent<SpatialComponent>()) { continue; }

					ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();

					this.BasicEffect.DiffuseColor = spatialComponent.Color.ToVector3();

					Box3D worldBox = new Box3D(spatialComponent.Position + new Vector3(0, spatialComponent.Extents.Y * 0.5f, 0),
						spatialComponent.Extents.X * 2.0f,
						spatialComponent.Extents.Y,
						spatialComponent.Extents.Z * 2.0f);
					Renderer3D.RenderBox(this.GraphicsDeviceManager.GraphicsDevice, this.BasicEffect, worldBox);
				}
			}
		}

		#endregion Methods
	}
}
