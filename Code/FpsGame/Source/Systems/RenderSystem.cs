using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.FpsGame
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

		public Dictionary<string, VertexPositionNormalTexture[]> Models { get; } = new Dictionary<string, VertexPositionNormalTexture[]>();

		#endregion Properties

		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the client.
		/// </summary>
		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
			// Particles...?
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client but only updates the commanding entity (for client-side prediction).
		/// </summary>
		public void ClientPrediction(EntityArray entityArray, Entity commandingEntity)
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
				this.BasicEffect.View = Matrix.CreateLookAt(commandingSpatialComponent.Position, commandingSpatialComponent.Position + Vector3.Transform(Vector3.Forward, commandingSpatialComponent.Orientation), Vector3.Up);

				foreach (Entity entity in entityArray)
				{
					if (!entity.HasComponent<SpatialComponent>()) { continue; }
					if (!entity.HasComponent<ModelComponent>()) { continue; }

					ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();
					ref ModelComponent modelComponent = ref entity.GetComponent<ModelComponent>();

					if (modelComponent.ModelName == "" || !this.Models.TryGetValue(modelComponent.ModelName, out VertexPositionNormalTexture[] model)) { continue; }

					this.BasicEffect.World = Matrix.CreateFromQuaternion(spatialComponent.Orientation) * Matrix.CreateTranslation(spatialComponent.Position);
					this.BasicEffect.CurrentTechnique.Passes[0].Apply();

					this.GraphicsDeviceManager.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, model, 0, model.Length / 3);
				}
			}
		}

		#endregion Methods
	}
}
