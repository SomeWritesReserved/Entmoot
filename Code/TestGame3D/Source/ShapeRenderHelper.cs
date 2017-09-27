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

		private static readonly VertexPositionTexture[] boxRenderVertices;

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
			ShapeRenderHelper.boxRenderVertices = new VertexPositionTexture[]
			{
				new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(x0, y2)),
				new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(x0, y1)),
				new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(x1, y1)),
				new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(x1, y1)),
				new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(x1, y2)),
				new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(x0, y2)),

				new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(x1, y1)),
				new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(x1, y0)),
				new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(x2, y0)),
				new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(x2, y0)),
				new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(x2, y1)),
				new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(x1, y1)),

				new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(x1, y2)),
				new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(x1, y1)),
				new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(x2, y1)),
				new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(x2, y1)),
				new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(x2, y2)),
				new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(x1, y2)),

				new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(x1, y3)),
				new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(x1, y2)),
				new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(x2, y2)),
				new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(x2, y2)),
				new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(x2, y3)),
				new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(x1, y3)),

				new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(x2, y2)),
				new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(x2, y1)),
				new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(x3, y1)),
				new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(x3, y1)),
				new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(x3, y2)),
				new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(x2, y2)),

				new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(x3, y2)),
				new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(x3, y1)),
				new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(x4, y1)),
				new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(x4, y1)),
				new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(x4, y2)),
				new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(x3, y2)),
			};
		}

		#endregion Constructors

		#region Methods

		public static void DrawBox(GraphicsDevice graphicsDevice, BasicEffect basicEffect, Vector3 position, float rotation)
		{
			basicEffect.World = Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(position);
			basicEffect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ShapeRenderHelper.boxRenderVertices, 0, ShapeRenderHelper.boxRenderVertices.Length / 3);

		}

		#endregion Methods
	}
}
