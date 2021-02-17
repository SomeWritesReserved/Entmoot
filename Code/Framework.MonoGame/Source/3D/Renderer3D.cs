using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.Framework.MonoGame
{
	public static class Renderer3D
	{
		#region Fields

		private static readonly VertexPositionNormalTexture[] boxRenderVertices;

		#endregion Fields

		#region Constructors

		static Renderer3D()
		{
			const float x0 = 0.0f;
			const float x1 = 1.0f / 4.0f;
			const float x2 = 2.0f / 4.0f;
			const float x3 = 3.0f / 4.0f;
			const float x4 = 1.0f;
			const float y0 = 0.0f;
			const float y1 = 1.0f / 3.0f;
			const float y2 = 2.0f / 3.0f;
			const float y3 = 1.0f;
			//   2
			// 1 3 5 6
			//   4
			Renderer3D.boxRenderVertices = new VertexPositionNormalTexture[]
			{
				new VertexPositionNormalTexture(new Vector3(1, 1, -1), Vector3.Right, new Vector2(x1, y1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), Vector3.Right, new Vector2(x1, y2)),
				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.Right, new Vector2(x0, y1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, 1), Vector3.Right, new Vector2(x0, y2)),
				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.Right, new Vector2(x0, y1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), Vector3.Right, new Vector2(x1, y2)),

				new VertexPositionNormalTexture(new Vector3(-1, 1, 1), Vector3.Up, new Vector2(x2, y0)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), Vector3.Up, new Vector2(x2, y1)),
				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.Up, new Vector2(x1, y0)),
				new VertexPositionNormalTexture(new Vector3(1, 1, -1), Vector3.Up, new Vector2(x1, y1)),
				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.Up, new Vector2(x1, y0)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), Vector3.Up, new Vector2(x2, y1)),

				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), Vector3.Forward, new Vector2(x2, y1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, -1), Vector3.Forward, new Vector2(x2, y2)),
				new VertexPositionNormalTexture(new Vector3(1, 1, -1), Vector3.Forward, new Vector2(x1, y1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), Vector3.Forward, new Vector2(x1, y2)),
				new VertexPositionNormalTexture(new Vector3(1, 1, -1), Vector3.Forward, new Vector2(x1, y1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, -1), Vector3.Forward, new Vector2(x2, y2)),

				new VertexPositionNormalTexture(new Vector3(-1, -1, -1), Vector3.Down, new Vector2(x2, y2)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), Vector3.Down, new Vector2(x2, y3)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), Vector3.Down, new Vector2(x1, y2)),
				new VertexPositionNormalTexture(new Vector3(1, -1, 1), Vector3.Down, new Vector2(x1, y3)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), Vector3.Down, new Vector2(x1, y2)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), Vector3.Down, new Vector2(x2, y3)),

				new VertexPositionNormalTexture(new Vector3(-1, 1, 1), Vector3.Left, new Vector2(x3, y1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), Vector3.Left, new Vector2(x3, y2)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), Vector3.Left, new Vector2(x2, y1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, -1), Vector3.Left, new Vector2(x2, y2)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), Vector3.Left, new Vector2(x2, y1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), Vector3.Left, new Vector2(x3, y2)),

				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.Backward, new Vector2(x4, y1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, 1), Vector3.Backward, new Vector2(x4, y2)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, 1), Vector3.Backward, new Vector2(x3, y1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), Vector3.Backward, new Vector2(x3, y2)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, 1), Vector3.Backward, new Vector2(x3, y1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, 1), Vector3.Backward, new Vector2(x4, y2)),
			};
		}

		#endregion Constructors

		#region Methods

		public static void RenderBox<TEffect>(GraphicsDevice graphicsDevice, TEffect effect, Vector3 position, Quaternion rotation)
			where TEffect : Effect, IEffectMatrices
		{
			effect.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
			effect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Renderer3D.boxRenderVertices, 0, Renderer3D.boxRenderVertices.Length / 3);
		}

		public static void RenderBox<TEffect>(GraphicsDevice graphicsDevice, TEffect effect, Box3D box)
			where TEffect : Effect, IEffectMatrices
		{
			effect.World = Matrix.CreateScale(box.Size * 0.5f) * Matrix.CreateTranslation(box.Center);
			effect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Renderer3D.boxRenderVertices, 0, Renderer3D.boxRenderVertices.Length / 3);
		}

		#endregion Methods
	}
}
