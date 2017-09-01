using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public sealed class ComponentCollection<TComponent> : IComponentCollection
		where TComponent : struct
	{
		#region Fields

		private TComponent[] components;

		#endregion Fields

		#region Constructors

		public ComponentCollection(int capacity)
		{
			this.capacity = capacity;
			this.components = new TComponent[this.capacity];
		}

		#endregion Constructors

		#region Properties

		private readonly int capacity;
		public int Capacity
		{
			get { return this.capacity; }
		}

		#endregion Properties

		#region Methods

		public ref TComponent GetComponent(int entityID)
		{
			return ref this.components[entityID];
		}

		public void ResetComponent(int entityID)
		{
			this.components[entityID] = default(TComponent);
		}

		#endregion Methods
	}

	public interface IComponentCollection
	{
		#region Properties

		int Capacity { get; }

		#endregion Properties

		#region Methods

		void ResetComponent(int entityID);

		#endregion Methods
	}
}
