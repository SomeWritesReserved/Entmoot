using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public sealed class ComponentCollection<TComponent> : IComponentCollection
		where TComponent : struct, IComponent<TComponent>
	{
		#region Fields

		private TComponent[] components;
		private BitArray entityComponentStates;

		#endregion Fields

		#region Constructors

		public ComponentCollection(int entityCapacity)
		{
			this.EntityCapacity = entityCapacity;
			this.components = new TComponent[this.EntityCapacity];
			this.entityComponentStates = new BitArray(this.EntityCapacity, defaultValue: false);
		}

		#endregion Constructors

		#region Properties

		public int EntityCapacity { get; }

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
	}

	public interface IComponent<TComponent>
	{
		#region Methods

		void Interpolate(TComponent to, TComponent from, float amount);

		void Serialize();
		void Deserialize();

		#endregion Methods
	}
}
