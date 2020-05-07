using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileFormatWavefront;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.FpsGame
{
	public static class ModelLoader
	{
		#region Methods

		public static Model LoadObj(GraphicsDevice graphiceDevice, string file, float userScale)
		{
			var loadResult = FileFormatObj.Load(file, loadTextureImages: false);

			var model = loadResult.Model;
			List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
			foreach (var face in model.Groups.SelectMany((group) => group.Faces)
				.Concat(model.UngroupedFaces))
			{
				{
					Vector3 position = model.Vertices[face.Indices[0].vertex].ToXna();
					Vector3 normal = face.Indices[0].normal.HasValue ? model.Normals[face.Indices[0].normal.Value].ToXna() : Vector3.Zero;
					Vector2 textureCoordinate = face.Indices[0].uv.HasValue ? model.Uvs[face.Indices[0].uv.Value].ToXna() : Vector2.Zero;
					vertices.Add(new VertexPositionNormalTexture(position * userScale, normal, textureCoordinate));
				}
				{
					Vector3 position = model.Vertices[face.Indices[2].vertex].ToXna();
					Vector3 normal = face.Indices[2].normal.HasValue ? model.Normals[face.Indices[2].normal.Value].ToXna() : Vector3.Zero;
					Vector2 textureCoordinate = face.Indices[2].uv.HasValue ? model.Uvs[face.Indices[2].uv.Value].ToXna() : Vector2.Zero;
					vertices.Add(new VertexPositionNormalTexture(position * userScale, normal, textureCoordinate));
				}
				{
					Vector3 position = model.Vertices[face.Indices[1].vertex].ToXna();
					Vector3 normal = face.Indices[1].normal.HasValue ? model.Normals[face.Indices[1].normal.Value].ToXna() : Vector3.Zero;
					Vector2 textureCoordinate = face.Indices[1].uv.HasValue ? model.Uvs[face.Indices[1].uv.Value].ToXna() : Vector2.Zero;
					vertices.Add(new VertexPositionNormalTexture(position * userScale, normal, textureCoordinate));
				}

				if (face.Indices.Count > 3)
				{
					{
						Vector3 position = model.Vertices[face.Indices[3].vertex].ToXna();
						Vector3 normal = face.Indices[3].normal.HasValue ? model.Normals[face.Indices[3].normal.Value].ToXna() : Vector3.Zero;
						Vector2 textureCoordinate = face.Indices[3].uv.HasValue ? model.Uvs[face.Indices[3].uv.Value].ToXna() : Vector2.Zero;
						vertices.Add(new VertexPositionNormalTexture(position * userScale, normal, textureCoordinate));
					}
					{
						Vector3 position = model.Vertices[face.Indices[2].vertex].ToXna();
						Vector3 normal = face.Indices[2].normal.HasValue ? model.Normals[face.Indices[2].normal.Value].ToXna() : Vector3.Zero;
						Vector2 textureCoordinate = face.Indices[2].uv.HasValue ? model.Uvs[face.Indices[2].uv.Value].ToXna() : Vector2.Zero;
						vertices.Add(new VertexPositionNormalTexture(position * userScale, normal, textureCoordinate));
					}
					{
						Vector3 position = model.Vertices[face.Indices[0].vertex].ToXna();
						Vector3 normal = face.Indices[0].normal.HasValue ? model.Normals[face.Indices[0].normal.Value].ToXna() : Vector3.Zero;
						Vector2 textureCoordinate = face.Indices[0].uv.HasValue ? model.Uvs[face.Indices[0].uv.Value].ToXna() : Vector2.Zero;
						vertices.Add(new VertexPositionNormalTexture(position * userScale, normal, textureCoordinate));
					}
				}
			}

			Texture2D texture = null;
			if (model.Materials.Any() && model.Materials.First().TextureMapDiffuse != null)
			{
				string texturePath = Path.Combine(Path.GetDirectoryName(file), model.Materials[0].TextureMapDiffuse.Path);
				using (FileStream fileStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read))
				{
					texture = Texture2D.FromStream(graphiceDevice, fileStream);
				}
			}

			return new Model()
			{
				Vertices = vertices.ToArray(),
				Texture = texture,
			};
		}

		public static Model GetBoxModel()
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
			return new Model()
			{
				Vertices = vertices,
			};
		}

		public static Vector2 ToXna(this FileFormatWavefront.Model.UV uv)
		{
			return new Vector2(uv.u - 1, -uv.v);
		}

		public static Vector3 ToXna(this FileFormatWavefront.Model.Vertex vertex)
		{
			return new Vector3(vertex.x, vertex.y, vertex.z);
		}

		#endregion Methods
	}
}
