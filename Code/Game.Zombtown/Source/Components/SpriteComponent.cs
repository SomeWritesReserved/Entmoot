using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;

namespace Entmoot.Game.Zombtown
{
	public struct SpriteComponent : IComponent<SpriteComponent>
	{
		#region Fields

		public int SpriteId;
		public byte SpriteDepth;

		#endregion Fields

		#region Methods

		public bool Equals(SpriteComponent other)
		{
			return (this.SpriteId == other.SpriteId &&
				this.SpriteDepth == other.SpriteDepth);
		}

		public void Interpolate(SpriteComponent otherA, SpriteComponent otherB, float amount)
		{
			this.SpriteId = otherB.SpriteId;
			this.SpriteDepth = otherB.SpriteDepth;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.SpriteId);
			writer.Write(this.SpriteDepth);
		}

		public void Deserialize(IReader reader)
		{
			this.SpriteId = reader.ReadInt32();
			this.SpriteDepth = reader.ReadByte();
		}

		public void ResetToDefaults()
		{
			this.SpriteId = 0;
			this.SpriteDepth = 0;
		}

		public void SetSprite(string name, byte spriteDepth)
		{
			this.SpriteId = name.GetHashCode();
			this.SpriteDepth = spriteDepth;
		}

		#endregion Methods
	}
}
