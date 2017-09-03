using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a collection of identically typed components that can be applied to entities in the system.
	/// </summary>
	public sealed class ComponentCollection<TComponent> : IComponentCollection
		where TComponent : struct, IComponent<TComponent>
	{
		#region Fields

		private TComponent[] components;
		private BitArray entityComponentStates;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ComponentCollection(int entityCapacity)
		{
			this.EntityCapacity = entityCapacity;
			this.components = new TComponent[this.EntityCapacity];
			this.entityComponentStates = new BitArray(this.EntityCapacity, defaultValue: false);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the maximum number of entities that can have this type of component.
		/// </summary>
		public int EntityCapacity { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns whether the given entity has this specific type of component.
		/// </summary>
		public bool HasComponent(Entity entity)
		{
			return this.entityComponentStates[entity.ID];
		}

		/// <summary>
		/// Returns a reference to a component that the given entity has (or could have). Be aware
		/// that this will return the component reference even if the entity isn't assigned this type
		/// of component.
		/// </summary>
		public ref TComponent GetComponent(Entity entity)
		{
			return ref this.components[entity.ID];
		}

		/// <summary>
		/// Adds this specific type of component to the given entity and returns a refernce to the component.
		/// </summary>
		public ref TComponent AddComponent(Entity entity)
		{
			this.components[entity.ID] = default(TComponent);
			this.entityComponentStates[entity.ID] = true;
			return ref this.components[entity.ID];
		}

		/// <summary>
		/// Removes this specific type of component from the given entity.
		/// </summary>
		public void RemoveComponent(Entity entity)
		{
			this.components[entity.ID] = default(TComponent);
			this.entityComponentStates[entity.ID] = false;
		}

		#endregion Methods
	}

	/// <summary>
	/// Represents a collection of identically typed components that can be applied to entities in the system.
	/// </summary>
	public interface IComponentCollection
	{
	}

	/// <summary>
	/// Represents a specific aspect or facet of data that entities can take on, to be stored in component collections
	/// and consumed/modified by entity systems.
	/// </summary>
	public interface IComponent<TComponent>
	{
		#region Methods

		void Interpolate(TComponent to, TComponent from, float amount);

		void Serialize();
		void Deserialize();

		#endregion Methods
	}
}
