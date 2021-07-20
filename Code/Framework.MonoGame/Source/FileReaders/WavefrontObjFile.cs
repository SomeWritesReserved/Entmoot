using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileFormatWavefront;
using FileFormatWavefront.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.Framework.MonoGame
{
	public static class WavefrontObjFile
	{
		#region Methods

		public static Model3D Read(GraphicsDevice graphiceDevice, string objFile, string relativeTextureDirectory)
		{
			FileLoadResult<Scene> loadedResult = FileFormatObj.Load(objFile, loadTextureImages: false);
			Scene objModel = loadedResult.Model;

			List<ModelMesh3D> meshes = new List<ModelMesh3D>();
			foreach (IGrouping<string, Face> textureFaces in objModel.Groups.SelectMany((group) => group.Faces)
				.Concat(objModel.UngroupedFaces)
				.Where((face) => face.Material != null && face.Material.TextureMapDiffuse != null)
				.GroupBy((face) => face.Material.TextureMapDiffuse.Path))
			{
				string texturePath = Path.Combine(Path.GetDirectoryName(objFile), relativeTextureDirectory, textureFaces.Key);

				Texture2D diffuseTexture;
				using (FileStream fileStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read))
				{
					diffuseTexture = Texture2D.FromStream(graphiceDevice, fileStream);
				}

				List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
				foreach (Face face in textureFaces)
				{
					for (int indicesIndex = 2; indicesIndex < face.Indices.Count; indicesIndex++)
					{
						{
							Vector3 position = objModel.Vertices[face.Indices[0].vertex].ToXna();
							Vector3 normal = objModel.Normals[face.Indices[0].normal.Value].ToXna();
							Vector2 textureCoordinate = objModel.Uvs[face.Indices[0].uv.Value].ToXna();
							vertices.Add(new VertexPositionNormalTexture(position, normal, textureCoordinate));
						}
						{
							Vector3 position = objModel.Vertices[face.Indices[indicesIndex].vertex].ToXna();
							Vector3 normal = objModel.Normals[face.Indices[indicesIndex].normal.Value].ToXna();
							Vector2 textureCoordinate = objModel.Uvs[face.Indices[indicesIndex].uv.Value].ToXna();
							vertices.Add(new VertexPositionNormalTexture(position, normal, textureCoordinate));
						}
						{
							Vector3 position = objModel.Vertices[face.Indices[indicesIndex - 1].vertex].ToXna();
							Vector3 normal = objModel.Normals[face.Indices[indicesIndex - 1].normal.Value].ToXna();
							Vector2 textureCoordinate = objModel.Uvs[face.Indices[indicesIndex - 1].uv.Value].ToXna();
							vertices.Add(new VertexPositionNormalTexture(position, normal, textureCoordinate));
						}
					}
				}

				meshes.Add(new ModelMesh3D() { DiffuseTexture = diffuseTexture, Vertices = vertices.ToArray() });
			}

			return new Model3D() { Meshes = meshes.ToArray() };
		}

		public static Vector2 ToXna(this UV uv)
		{
			return new Vector2(uv.u - 1, -uv.v);
		}

		public static Vector3 ToXna(this Vertex vertex)
		{
			return new Vector3(vertex.x, vertex.y, vertex.z);
		}

		#endregion Methods
	}
}
