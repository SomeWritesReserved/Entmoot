using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an array of entities and their components.
	/// </summary>
	public class EntityArray
	{
		#region Fields

		/// <summary>Stores the states for each available entity ID, defining whether an entity exists in an index or not.</summary>
		private readonly EntityState[] entityStates;
		/// <summary>Stores the arrays of different component types that define what components these entities can have (not all component types will be added to all entities).</summary>
		private readonly ReadOnlyCollection<IComponentArray> componentArrays;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="capacity">The maximum number of entities that can exist.</param>
		/// <param name="componentsDefinition">The definition for the various component types that can be added to entities.</param>
		public EntityArray(int capacity, ComponentsDefinition componentsDefinition)
		{
			this.Capacity = capacity;
			this.entityStates = new EntityState[this.Capacity];
			this.componentArrays = componentsDefinition.CreateComponentArrays(this.Capacity);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the maximum number of entities that can exist in this collection.
		/// </summary>
		public int Capacity { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Tries to find an existing entity, returning false if the entity does not exist or if it
		/// hasn't been fully created yet.
		/// </summary>
		public bool TryGetEntity(int entityID, out Entity entity)
		{
			entity = default(Entity);
			if (this.entityStates[entityID] == EntityState.NoEntity || this.entityStates[entityID] == EntityState.Creating) { return false; }

			entity = new Entity(this, entityID);
			return true;
		}

		/// <summary>
		/// Tries to create a new entity, if space allows. Returns false if the entity could not be created.
		/// The entity will not be fully active until the end of the current update.
		/// </summary>
		public bool TryCreateEntity(out Entity entity)
		{
			entity = default(Entity);
			int nextEntityIndex = Array.IndexOf(this.entityStates, EntityState.NoEntity);
			if (nextEntityIndex < 0) { return false; }

			entity = new Entity(this, nextEntityIndex);
			this.entityStates[nextEntityIndex] = EntityState.Creating;
			foreach (IComponentArray componentArray in this.componentArrays)
			{
				componentArray.RemoveComponent(entity);
			}
			return true;
		}

		/// <summary>
		/// Removes an entity from the system. The entity will not be fully removed until the end of the current update.
		/// </summary>
		public void RemoveEntity(Entity entity)
		{
			if (this.entityStates[entity.ID] != EntityState.NoEntity)
			{
				this.entityStates[entity.ID] = EntityState.Removing;
			}
		}

		/// <summary>
		/// Returns the component array for a specific type of component.
		/// </summary>
		public ComponentArray<TComponent> GetComponentArray<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			return this.componentArrays.OfType<ComponentArray<TComponent>>().Single();
		}

		/// <summary>
		/// Begins the update to this entity array.
		/// </summary>
		public void BeginUpdate()
		{
		}

		/// <summary>
		/// Ends the update to this entity array, completing creations and removals of entities.
		/// </summary>
		public void EndUpdate()
		{
			for (int entityID = 0; entityID < this.Capacity; entityID++)
			{
				if (this.entityStates[entityID] == EntityState.Creating) { this.entityStates[entityID] = EntityState.Active; }
				if (this.entityStates[entityID] == EntityState.Removing) { this.entityStates[entityID] = EntityState.NoEntity; }
			}
		}

		/// <summary>
		/// Copies all entity and component data to another entity array.
		/// </summary>
		public void CopyTo(EntityArray other)
		{
			Array.Copy(this.entityStates, other.entityStates, this.Capacity);
			for (int componentTypeID = 0; componentTypeID < this.componentArrays.Count; componentTypeID++)
			{
				this.componentArrays[componentTypeID].CopyTo(other.componentArrays[componentTypeID]);
			}
		}

		#endregion Methods

		#region Nested Types

		/// <summary>
		/// Defines the current state of an entity.
		/// </summary>
		private enum EntityState : byte
		{
			/// <summary>The entity does not exist (undefined state or not part of the system).</summary>
			NoEntity = 0,
			/// <summary>The entity is in the process of being created and hasn't been completely added yet, it will be fully added at the end of the next update.</summary>
			Creating,
			/// <summary>The entity is actively part of the overall state.</summary>
			Active,
			/// <summary>The entity is still active but is scheduled to be removed at the end of the next update.</summary>
			Removing,
		}

		#endregion Nested Types
	}
}
