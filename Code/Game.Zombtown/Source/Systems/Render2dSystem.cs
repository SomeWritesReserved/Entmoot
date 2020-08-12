using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.Game.Zombtown
{
	public class Render2dSystem : IClientSystem
	{
		#region Fields

		private readonly Dictionary<int, string> textureIdToName = new Dictionary<int, string>();
		private readonly Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();

		#endregion Fields

		#region Constructors

		public Render2dSystem(GraphicsDeviceManager graphicsDeviceManager)
		{
			this.GraphicsDeviceManager = graphicsDeviceManager;
		}

		#endregion Constructors

		#region Properties

		public GraphicsDeviceManager GraphicsDeviceManager { get; }

		public SpriteBatch SpriteBatch { get; set; }

		#endregion Properties

		#region Methods

		public void Clear()
		{
			this.textureIdToName.Clear();
			this.textures.Clear();
		}

		public void AddTexture(string name, Texture2D texture)
		{
			int id = name.GetHashCode();
			textureIdToName.Add(id, name);
			textures.Add(id, texture);
		}

		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientPrediction(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
			if (!commandingEntity.IsValid || !commandingEntity.HasComponent<SpatialComponent>()) { return; }

			CameraComponent playerCameraComponent = commandingEntity.GetComponent<CameraComponent>();

			this.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<SpatialComponent>() || !entity.HasComponent<SpriteComponent>()) { continue; }

				SpatialComponent spatialComponent = entity.GetComponent<SpatialComponent>();
				SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();

				if (spatialComponent.Radius == 0) { continue; }
				if (spriteComponent.SpriteId == 0 || !this.textures.TryGetValue(spriteComponent.SpriteId, out Texture2D spriteTexture)) { continue; }

				this.renderSprite(spriteTexture, spatialComponent.Position, spatialComponent.Radius, spatialComponent.Rotation, spriteComponent.SpriteDepth,
					playerCameraComponent.Position, playerCameraComponent.Extents);
			}
			this.SpriteBatch.End();
		}

		/// <summary>
		/// Renders a sprite in a specific world location based on the specified camera view.
		/// </summary>
		/// <remarks>
		/// The size or dimensions of the texture does not matter, it will be scaled to fit into the sprites world position and rotated.
		/// Note: it is critical that the camera's aspect ratio match the render target's aspect ratios, otherwise the scaling and rotation will be wrong.
		/// </remarks>
		private void renderSprite(Texture2D texture, Vector2 spriteWorldPosition, float spriteWorldSize, float spriteWorldRotation, byte spriteRenderDepth, Vector2 cameraWorldPosition, Vector2 cameraWorldExtents)
		{
			// Get the dimensions of the camera in both screen and world space so we can transform between them.
			float cameraWorldWidth = cameraWorldExtents.X * 2.0f;
			float cameraWorldHeight = cameraWorldExtents.Y * 2.0f;
			float cameraScreenWidth = this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width;
			float cameraScreenHeight = this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height;

			// Get world positions of the camera's rectangle and the sprite's rectangle.
			// Use top-left and bottom-right (instead of bottom-left and top-right) as the rectangle extents to match the
			// inverted Y axis of screen space (where 0,0 is the top-left corner and 1,1 is the bottom-right).
			Vector2 cameraWorldTopLeft = new Vector2(
				cameraWorldPosition.X - cameraWorldExtents.X,
				cameraWorldPosition.Y + cameraWorldExtents.Y);
			Vector2 cameraWorldBottomRight = new Vector2(
				cameraWorldPosition.X + cameraWorldExtents.X,
				cameraWorldPosition.Y - cameraWorldExtents.Y);

			Vector2 spriteWorldTopLeft = new Vector2(
				spriteWorldPosition.X - spriteWorldSize,
				spriteWorldPosition.Y + spriteWorldSize);
			Vector2 spriteWorldCenter = new Vector2(
				spriteWorldPosition.X,
				spriteWorldPosition.Y);
			Vector2 spriteWorldBottomRight = new Vector2(
				spriteWorldPosition.X + spriteWorldSize,
				spriteWorldPosition.Y - spriteWorldSize);

			// Convert world positions to screen positions for the sprite based on the ratio between screen size and camera size, and the camera's world position.
			Vector2 spriteScreenTopLeft = new Vector2(
				(spriteWorldTopLeft.X - cameraWorldTopLeft.X) * (cameraScreenWidth / cameraWorldWidth),
				-(spriteWorldTopLeft.Y - cameraWorldTopLeft.Y) * (cameraScreenHeight / cameraWorldHeight));
			Vector2 spriteScreenCenter = new Vector2(
				(spriteWorldCenter.X - cameraWorldTopLeft.X) * (cameraScreenWidth / cameraWorldWidth),
				-(spriteWorldCenter.Y - cameraWorldTopLeft.Y) * (cameraScreenHeight / cameraWorldHeight));
			Vector2 spriteScreenBottomRight = new Vector2(
				(spriteWorldBottomRight.X - cameraWorldTopLeft.X) * (cameraScreenWidth / cameraWorldWidth),
				-(spriteWorldBottomRight.Y - cameraWorldTopLeft.Y) * (cameraScreenHeight / cameraWorldHeight));

			// SpriteBatch is weird because changing the origin changes how translation is applied. You would expect the destination
			// rectangle to be the top-left and bottom-right corners but because we change the origin to be the center of the texture
			// we need to specify the destination rectangle as starting in the center of the sprite.
			// I'm not sure why this is, I would expect to specify the top-left, but this is how SpriteBatch appears to work.
			Rectangle spriteScreenRectangle = new Rectangle(
				(int)Math.Round(spriteScreenCenter.X),
				(int)Math.Round(spriteScreenCenter.Y),
				(int)Math.Round(spriteScreenBottomRight.X - spriteScreenTopLeft.X),
				(int)Math.Round(spriteScreenBottomRight.Y - spriteScreenTopLeft.Y));

			// After all this calculation it doesn't even work because rotations happen _after_ the scaling, so the scaled sprite is rotated causing the elongated
			// axis to spin. This means the aspect ratio of the camera must match the screen.
			Vector2 textureOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			this.SpriteBatch.Draw(texture, spriteScreenRectangle, null, Color.White, spriteWorldRotation, textureOrigin, SpriteEffects.None, spriteRenderDepth / 255.0f);
		}

		#endregion Methods
	}
}
