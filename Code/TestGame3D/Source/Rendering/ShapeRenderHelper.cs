using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.TestGame3D
{
	public static class ShapeRenderHelper
	{
		#region Fields

		private static readonly VertexPositionNormalTexture[] unitBoxRenderVertices;
		private static readonly VertexPositionNormalTexture[] boxRenderVertices;
		private static readonly VertexPositionNormalTexture[] lineVertices;

		#endregion Fields

		#region Constructors

		static ShapeRenderHelper()
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
			ShapeRenderHelper.unitBoxRenderVertices = new VertexPositionNormalTexture[]
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

			ShapeRenderHelper.boxRenderVertices = ShapeRenderHelper.unitBoxRenderVertices
				.Select((vertex) => new VertexPositionNormalTexture(vertex.Position * 0.5f + Vector3.Up * 0.5f, vertex.Normal, vertex.TextureCoordinate))
				.ToArray();

			ShapeRenderHelper.lineVertices = new VertexPositionNormalTexture[]
			{
				//new VertexPositionColor(Vector3.Zero, Color.Blue),
				//new VertexPositionColor(Vector3.Up, Color.LightBlue),
				new VertexPositionNormalTexture(Vector3.Zero, Vector3.Left, Vector2.Zero),
				new VertexPositionNormalTexture(Vector3.Up, Vector3.Left, Vector2.One),
			};
		}

		#endregion Constructors

		#region Methods

		public static void RenderUnitBox<TEffect>(GraphicsDevice graphicsDevice, TEffect effect, float scale, Vector3 position, Quaternion rotation)
			where TEffect : Effect, IEffectMatrices
		{
			effect.World = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
			effect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ShapeRenderHelper.unitBoxRenderVertices, 0, ShapeRenderHelper.unitBoxRenderVertices.Length / 3);
		}

		public static void RenderOriginBox<TEffect>(GraphicsDevice graphicsDevice, TEffect effect, Vector3 boxScale, Matrix transform)
			where TEffect : Effect, IEffectMatrices
		{
			effect.World = Matrix.CreateScale(boxScale) * transform;
			effect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ShapeRenderHelper.boxRenderVertices, 0, ShapeRenderHelper.boxRenderVertices.Length / 3);
		}

		public static void RenderLine<TEffect>(GraphicsDevice graphicsDevice, TEffect effect, Vector3 start, Vector3 end)
			where TEffect : Effect, IEffectMatrices
		{
			effect.World = Matrix.Identity;
			ShapeRenderHelper.lineVertices[0].Position = start;
			ShapeRenderHelper.lineVertices[1].Position = end;
			effect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, ShapeRenderHelper.lineVertices, 0, 1);
		}

		#endregion Methods
	}
}
