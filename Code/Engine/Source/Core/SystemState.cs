﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a specific state that the system (entities and components) may be in.
	/// </summary>
	public class SystemState
	{
		#region Fields

		/// <summary>Stores the states for each available entity slot, defining whether an entity exists in a slot or not (as well as any other state info).</summary>
		private EntityState[] entityStates;
		/// <summary>Stores the collections of different component types that define what data entities have (not all entities have all component types).</summary>
		private IComponentCollection[] componentCollections;

		#endregion Fields

		#region Constructors

		public SystemState(int entityCapacity, ComponentsDefinition componentsDefinition)
		{
			this.EntityCapacity = entityCapacity;
			this.entityStates = new EntityState[this.EntityCapacity];
			this.componentCollections = componentsDefinition.CreateComponentCollections(this.EntityCapacity);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the maximum number of entities that can exist.
		/// </summary>
		public int EntityCapacity { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Tries to create a new entity, if space allows. Returns false if the entity could not be created.
		/// The entity will not be fully active until the end of the current update.
		/// </summary>
		public bool TryCreateEntity(out Entity entity)
		{
			entity = default(Entity);
			int nextEntityIndex = Array.IndexOf(this.entityStates, EntityState.NoEntity);
			if (nextEntityIndex < 0) { return false; }

			this.entityStates[nextEntityIndex] = EntityState.Creating;
			entity = new Entity(nextEntityIndex);
			return true;
		}

		/// <summary>
		/// Removes an entity from the system. The entity will not be fully removed until the end of the current update.
		/// </summary>
		public void RemoveEntity(Entity entity)
		{
			this.entityStates[entity.ID] = EntityState.Removing;
		}

		/// <summary>
		/// Returns the component collection for a specific type of component.
		/// </summary>
		public ComponentCollection<TComponent> GetComponentCollection<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			return this.componentCollections.OfType<ComponentCollection<TComponent>>().Single();
		}

		/// <summary>
		/// Begins the update to this state.
		/// </summary>
		public void BeginUpdate()
		{
		}

		/// <summary>
		/// Ends the update to this state, completing creations and removals of entities.
		/// </summary>
		public void EndUpdate()
		{
			foreach (int entityID in Enumerable.Range(0, this.EntityCapacity))
			{
				if (this.entityStates[entityID] == EntityState.Creating) { this.entityStates[entityID] = EntityState.Active; }
				if (this.entityStates[entityID] == EntityState.Removing) { this.entityStates[entityID] = EntityState.NoEntity; }
			}
		}

		#endregion Methods
	}
}
