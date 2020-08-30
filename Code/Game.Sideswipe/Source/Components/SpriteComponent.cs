using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.Game.Sideswipe
{
	/// <summary>
	/// Represents a renderable sprite. If an entity does not have this component it cannot be rendered.
	/// </summary>
	public struct SpriteComponent : IComponent<SpriteComponent>
	{
		#region Fields

		/// <summary>
		/// The string name of a sprite asset, the file name as it exists in the Assets folder. If this is left blank then the sprite
		/// will render with a solid block (no texture).
		/// </summary>
		public string SpriteAssetName;

		/// <summary>
		/// The color to tint the sprite (where White is no tinting).
		/// </summary>
		public Color SpriteColor;

		/// <summary>
		/// The Z ordering depth of the sprite to layer on top of or underneith other sprites.
		/// </summary>
		public byte ZOrder;

		#endregion Fields

		#region Methods

		public bool Equals(SpriteComponent other)
		{
			return (this.SpriteAssetName == other.SpriteAssetName &&
				this.SpriteColor == other.SpriteColor &&
				this.ZOrder == other.ZOrder);
		}

		public void Interpolate(SpriteComponent otherA, SpriteComponent otherB, float amount)
		{
			this.SpriteAssetName = otherB.SpriteAssetName;
			this.SpriteColor = Color.Lerp(otherA.SpriteColor, otherB.SpriteColor, amount);
			this.ZOrder = otherB.ZOrder;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.SpriteAssetName);
			writer.Write(this.SpriteColor.R);
			writer.Write(this.SpriteColor.G);
			writer.Write(this.SpriteColor.B);
			writer.Write(this.ZOrder);
		}

		public void Deserialize(IReader reader)
		{
			this.SpriteAssetName = reader.ReadString();
			this.SpriteColor.R = reader.ReadByte();
			this.SpriteColor.G = reader.ReadByte();
			this.SpriteColor.B = reader.ReadByte();
			this.ZOrder = reader.ReadByte();
		}

		public void ResetToDefaults()
		{
			this.SpriteAssetName = string.Empty;
			this.SpriteColor = Color.White;
			this.ZOrder = 0;
		}

		#endregion Methods
	}
}
