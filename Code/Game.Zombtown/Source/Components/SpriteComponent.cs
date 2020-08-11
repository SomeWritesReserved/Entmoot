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

		#endregion Fields

		#region Methods

		public bool Equals(SpriteComponent other)
		{
			return (this.SpriteId == other.SpriteId);
		}

		public void Interpolate(SpriteComponent otherA, SpriteComponent otherB, float amount)
		{
			this.SpriteId = otherB.SpriteId;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.SpriteId);
		}

		public void Deserialize(IReader reader)
		{
			this.SpriteId = reader.ReadInt32();
		}

		public void ResetToDefaults()
		{
			this.SpriteId = 0;
		}

		public void SetSprite(string name)
		{
			this.SpriteId = name.GetHashCode();
		}

		#endregion Methods
	}
}
