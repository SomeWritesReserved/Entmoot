using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an array of identically typed components that define which entities have a specific component type.
	/// </summary>
	public interface IComponentArray
	{
		#region Properties

		/// <summary>
		/// Gets the maximum number of entities that can have this type of component.
		/// </summary>
		int Capacity { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns whether the given entity has this specific type of component.
		/// </summary>
		bool HasComponent(Entity entity);

		#endregion Methods
	}

	/// <summary>
	/// Represents an array of identically typed components that define which entities have a <see cref="TComponent"/>.
	/// </summary>
	public sealed class ComponentArray<TComponent> : IComponentArray
		where TComponent : struct, IComponent<TComponent>
	{
		#region Fields

		/// <summary>Stores the array of individual components that will be indexed into by entity ID to get the component value.</summary>
		private TComponent[] components;
		/// <summary>Stores whether or not a given entity ID has been assigned this component type.</summary>
		private BitArray entityComponentStates;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ComponentArray(int capacity)
		{
			this.Capacity = capacity;
			this.components = new TComponent[this.Capacity];
			this.entityComponentStates = new BitArray(this.Capacity, defaultValue: false);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the maximum number of entities that can have this type of component.
		/// </summary>
		public int Capacity { get; }

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
		/// Adds this specific type of component to the given entity and returns a reference to the component.
		/// </summary>
		public ref TComponent AddComponent(Entity entity)
		{
			// Don't default the entity state here so consumers can call this as an easy way to get a reference
			// to the component and make sure its added to the entity (without having to check manually and then do GetComponent)
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
	/// Represents a specific aspect or facet of data that entities can take on, to be  managed by a component array.
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
