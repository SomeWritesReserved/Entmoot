using System;
using System.Collections;
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
		private BitArray entityComponentStates;

		#endregion Fields

		#region Constructors

		public ComponentCollection(int capacity)
		{
			this.Capacity = capacity;
			this.components = new TComponent[this.Capacity];
			this.entityComponentStates = new BitArray(this.Capacity, defaultValue: false);
		}

		#endregion Constructors

		#region Properties

		public int Capacity { get; }

		#endregion Properties

		#region Methods

		public bool HasComponent(Entity entity)
		{
			return this.entityComponentStates[entity.ID];
		}

		public ref TComponent GetComponent(Entity entity)
		{
			return ref this.components[entity.ID];
		}

		public ref TComponent AddComponent(Entity entity)
		{
			this.components[entity.ID] = default(TComponent);
			this.entityComponentStates[entity.ID] = true;
			return ref this.components[entity.ID];
		}

		public void RemoveComponent(Entity entity)
		{
			this.components[entity.ID] = default(TComponent);
			this.entityComponentStates[entity.ID] = false;
		}

		#endregion Methods
	}

	public interface IComponentCollection
	{
		#region Properties

		int Capacity { get; }

		#endregion Properties

		#region Methods

		bool HasComponent(Entity entity);

		#endregion Methods
	}
}
