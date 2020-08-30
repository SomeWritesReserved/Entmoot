using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.Game.Sideswipe
{
	/// <summary>
	/// The system that renders the world to the screen in a 2D manner.
	/// </summary>
	public class Render2dSystem : IClientSystem
	{
		#region Fields

		/// <summary>The dictionary the stores all the loaded sprite textures and can be looked up by sprite asset name.</summary>
		private readonly Dictionary<string, Texture2D> spriteAssets = new Dictionary<string, Texture2D>();

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
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

		/// <summary>
		/// Clears all sprites from this render system, usually when sprites need to be reloaded (level change, graphics device switch, etc.).
		/// </summary>
		public void ClearSprites()
		{
			this.spriteAssets.Clear();
		}

		/// <summary>
		/// Adds a sprite that was already loaded to this render system so <see cref="SpriteComponent"/> can reference it by the same name (case-sensitive).
		/// The asset name should be the file name as it exists in the Assets folder.
		/// </summary>
		public void AddLoadedSprite(string assetName, Texture2D texture)
		{
			spriteAssets.Add(assetName, texture);
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client.
		/// </summary>
		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
		}

		/// <summary>
		/// Runs this system over the given array of entities on the client but only updates the commanding entity (for client-side prediction).
		/// </summary>
		public void ClientPrediction(EntityArray entityArray, Entity commandingEntity)
		{
		}

		/// <summary>
		/// Allows this system to perform any rendering. This is a 2D renderer that maps X and Y to screen space (where +X is right and +Y is up).
		/// This uses <see cref="SpatialComponent"/> and <see cref="SpriteComponent"/>, so be sure entities have those setup to render.
		/// This uses the commanding entity's <see cref="CameraComponent"/> to know where to position the screen.
		/// </summary>
		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
			if (!commandingEntity.IsValid || !commandingEntity.HasComponent<CameraComponent>()) { return; }

			ref CameraComponent playerCameraComponent = ref commandingEntity.GetComponent<CameraComponent>();

			this.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
			foreach (Entity entity in entityArray)
			{
				if (!entity.HasComponent<SpatialComponent>() || !entity.HasComponent<SpriteComponent>()) { continue; }

				ref SpatialComponent spatialComponent = ref entity.GetComponent<SpatialComponent>();
				ref SpriteComponent spriteComponent = ref entity.GetComponent<SpriteComponent>();

				if (spatialComponent.Extents == Vector2.Zero) { continue; }
				if (spriteComponent.SpriteAssetName == null || !this.spriteAssets.TryGetValue(spriteComponent.SpriteAssetName, out Texture2D spriteTexture)) { continue; }

				this.renderSprite(spriteTexture, spriteComponent.SpriteColor, spriteComponent.ZOrder,
					spatialComponent.Position, spatialComponent.Extents, spatialComponent.Rotation,
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
		private void renderSprite(Texture2D texture, Color spriteColor, byte spriteRenderDepth,
			Vector2 spriteWorldPosition, Vector2 spriteWorldExtents, float spriteWorldRotation,
			Vector2 cameraWorldPosition, Vector2 cameraWorldExtents)
		{
			// Get the dimensions of the camera in both screen and world space so we can transform between them.
			float cameraWorldWidth = cameraWorldExtents.X;
			float cameraWorldHeight = cameraWorldExtents.Y;
			float cameraScreenWidth = this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width;
			float cameraScreenHeight = this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height;

			// Get world positions of the camera's rectangle and the sprite's rectangle.
			// Use top-left and bottom-right (instead of bottom-left and top-right) as the rectangle extents to match the
			// inverted Y axis of screen space (where 0,0 is the top-left corner and 1,1 is the bottom-right).
			Vector2 cameraWorldTopLeft = new Vector2(
				cameraWorldPosition.X - cameraWorldExtents.X * 0.5f,
				cameraWorldPosition.Y + cameraWorldExtents.Y * 0.5f);
			Vector2 cameraWorldBottomRight = new Vector2(
				cameraWorldPosition.X + cameraWorldExtents.X * 0.5f,
				cameraWorldPosition.Y - cameraWorldExtents.Y * 0.5f);

			Vector2 spriteWorldTopLeft = new Vector2(
				spriteWorldPosition.X - spriteWorldExtents.X * 0.5f,
				spriteWorldPosition.Y + spriteWorldExtents.Y * 0.5f);
			Vector2 spriteWorldCenter = new Vector2(
				spriteWorldPosition.X,
				spriteWorldPosition.Y);
			Vector2 spriteWorldBottomRight = new Vector2(
				spriteWorldPosition.X + spriteWorldExtents.X * 0.5f,
				spriteWorldPosition.Y - spriteWorldExtents.Y * 0.5f);

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
				(int)MathF.Round(spriteScreenCenter.X),
				(int)MathF.Round(spriteScreenCenter.Y),
				(int)MathF.Round(spriteScreenBottomRight.X - spriteScreenTopLeft.X),
				(int)MathF.Round(spriteScreenBottomRight.Y - spriteScreenTopLeft.Y));

			// After all this calculation it doesn't even work because rotations happen _after_ the scaling, so the scaled sprite is rotated causing the elongated
			// axis to spin. This means the aspect ratio of the camera must match the screen.
			Vector2 textureOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			this.SpriteBatch.Draw(texture, spriteScreenRectangle, null, spriteColor, spriteWorldRotation, textureOrigin, SpriteEffects.None, spriteRenderDepth / 255.0f);
		}

		#endregion Methods
	}
}
