using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UkooLabs.FbxSharpie;
using UkooLabs.FbxSharpie.Extensions;
using UkooLabs.FbxSharpie.Tokens;

namespace Entmoot.FpsGame
{
	public class ModelLoader
	{
		#region Methods

		public static VertexPositionNormalTexture[] LoadFbx(string path, float userScale)
		{
			FbxDocument fbxDocument = FbxIO.Read(path);
			var mats = fbxDocument.GetMaterialIds();
			var geometryIds = fbxDocument.GetGeometryIds();
			var globalScale = (float)fbxDocument.GetScaleFactor() * userScale;

			List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
			foreach (long geometryId in geometryIds)
			{
				FbxNode geometryNode = fbxDocument.GetGeometry(geometryId);

				FbxNode geometryPropertiesNode = geometryNode.GetRelative(fbxDocument.PropertiesName);
				var preRotationProperty = fbxDocument.GetNodeWithValue(geometryPropertiesNode.Nodes, new StringToken("PreRotation"));
				var translationProperty = fbxDocument.GetNodeWithValue(geometryPropertiesNode.Nodes, new StringToken("Lcl Translation"));
				var rotationProperty = fbxDocument.GetNodeWithValue(geometryPropertiesNode.Nodes, new StringToken("Lcl Rotation"));
				var scaleProperty = fbxDocument.GetNodeWithValue(geometryPropertiesNode.Nodes, new StringToken("Lcl Scaling"));

				Vector3 preRotation = new Vector3(
					MathHelper.ToRadians(preRotationProperty.Properties[3].GetAsFloat()),
					MathHelper.ToRadians(preRotationProperty.Properties[4].GetAsFloat()),
					MathHelper.ToRadians(preRotationProperty.Properties[5].GetAsFloat()));
				Vector3 translation = new Vector3(
					translationProperty.Properties[3].GetAsFloat(),
					translationProperty.Properties[4].GetAsFloat(),
					translationProperty.Properties[5].GetAsFloat());
				Vector3 rotation = new Vector3(
					MathHelper.ToRadians(rotationProperty.Properties[3].GetAsFloat()),
					MathHelper.ToRadians(rotationProperty.Properties[4].GetAsFloat()),
					MathHelper.ToRadians(rotationProperty.Properties[5].GetAsFloat()));
				Vector3 scale = new Vector3(
					scaleProperty.Properties[3].GetAsFloat(),
					scaleProperty.Properties[4].GetAsFloat(),
					scaleProperty.Properties[5].GetAsFloat());

				//x, z, -y
				Matrix transform =
					//Matrix.CreateFromAxisAngle(Vector3.UnitZ, rotation.X)*
					Matrix.CreateFromAxisAngle(Vector3.UnitX, rotation.Y) *
					Matrix.CreateFromAxisAngle(Vector3.UnitY, rotation.Z) *
					Matrix.CreateTranslation(new Vector3(translation.X, translation.Y, translation.Z)) *
					//Matrix.CreateFromAxisAngle(Vector3.UnitZ, preRotation.Z)*
					//Matrix.CreateFromAxisAngle(Vector3.UnitY, preRotation.Y)*
					//Matrix.CreateFromAxisAngle(Vector3.UnitX, preRotation.X)*
					//Matrix.CreateFromAxisAngle(Vector3.UnitY, rotation.Y)*
					//Matrix.CreateFromAxisAngle(Vector3.UnitX, rotation.X)*
					//Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) *
					//Matrix.CreateFromYawPitchRoll(preRotation.X, preRotation.Y, preRotation.Z) *
					Matrix.CreateScale(new Vector3(scale.X, scale.Y, -scale.Z) * globalScale) *
					Matrix.Identity;

				var vertexIndices = fbxDocument.GetVertexIndices(geometryId);
				var positions = fbxDocument.GetPositions(geometryId, vertexIndices)
					.Select((position) => Vector3.Transform(position.ToXna(), transform))
					.ToArray();
				var normals = fbxDocument.GetNormals(geometryId, vertexIndices)
					.Select((normal) => Vector3.TransformNormal(normal.ToXna(), transform))
					.ToArray();
				var textureCoords = fbxDocument.GetTexCoords(geometryId, vertexIndices)
					.Select((uv) => uv.ToXna())
					.ToArray();

				for (int i = 2; i < positions.Length; i += 3)
				{
					vertices.Add(new VertexPositionNormalTexture(positions[i - 2], normals[i - 2], textureCoords[i - 2]));
					vertices.Add(new VertexPositionNormalTexture(positions[i - 1], normals[i - 1], textureCoords[i - 1]));
					vertices.Add(new VertexPositionNormalTexture(positions[i], normals[i], textureCoords[i]));
				}
			}
			return vertices.ToArray();
		}

		public static VertexPositionNormalTexture[] GetBoxModel()
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
			VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[]
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
			return vertices;
		}

		#endregion Methods
	}
}
