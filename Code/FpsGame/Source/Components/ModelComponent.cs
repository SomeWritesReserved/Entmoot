using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;

namespace Entmoot.FpsGame
{
	public struct ModelComponent : IComponent<ModelComponent>
	{
		#region Fields

		public string ModelName
		{
			get;
			set;
		}

		#endregion Fields

		#region Methods

		public bool Equals(ModelComponent other)
		{
			return this.ModelName.Equals(other.ModelName);
		}

		public void Interpolate(ModelComponent otherA, ModelComponent otherB, float amount)
		{
			// Models cannot interpolate, just use the last one
			this.ModelName = otherA.ModelName;
		}

		public void Serialize(IWriter writer)
		{
			writer.Write(this.ModelName);
		}

		public void Deserialize(IReader reader)
		{
			this.ModelName = reader.ReadString();
		}

		public void ResetToDefaults()
		{
			this.ModelName = string.Empty;
		}

		#endregion Methods
	}
}
