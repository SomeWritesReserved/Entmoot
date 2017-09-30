using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an array of entities and their components.
	/// </summary>
	public class EntityArray : IEnumerable<Entity>, IEnumerable
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
			entity = Entity.NoEntity;
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
			entity = Entity.NoEntity;
			int nextEntityID = Array.IndexOf(this.entityStates, EntityState.NoEntity);
			if (nextEntityID < 0) { return false; }

			entity = new Entity(this, nextEntityID);
			this.entityStates[nextEntityID] = EntityState.Creating;
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

		/// <summary>
		/// Copies the given entity and its component data to another entity in another entity array.
		/// </summary>
		public void CopyEntityTo(int thisEntityID, EntityArray otherEntityArray, int otherEntityID)
		{
			for (int componentTypeID = 0; componentTypeID < this.componentArrays.Count; componentTypeID++)
			{
				this.componentArrays[componentTypeID].CopyEntityTo(thisEntityID, otherEntityArray.componentArrays[componentTypeID], otherEntityID);
			}
		}

		/// <summary>
		/// Updates the state of all entities and components to represent an interpolation between two other entity arrays.
		/// </summary>
		public void Interpolate(EntityArray otherA, EntityArray otherB, float amount)
		{
			Array.Copy(otherB.entityStates, this.entityStates, this.Capacity);
			for (int componentTypeID = 0; componentTypeID < this.componentArrays.Count; componentTypeID++)
			{
				this.componentArrays[componentTypeID].Interpolate(otherA.componentArrays[componentTypeID], otherB.componentArrays[componentTypeID], amount);
			}
		}

		/// <summary>
		/// Writes all entity and component data to a binary source.
		/// </summary>
		public void Serialize(EntityArray previousEntityArray, IWriter writer)
		{
			for (int i = 0; i < this.entityStates.Length; i++) { writer.Write((byte)this.entityStates[i]); }
			for (int componentTypeID = 0; componentTypeID < this.componentArrays.Count; componentTypeID++)
			{
				this.componentArrays[componentTypeID].Serialize(previousEntityArray?.componentArrays[componentTypeID], writer);
			}
		}

		/// <summary>
		/// Reads and overwrites all current entity and component data from a binary source.
		/// </summary>
		public void Deserialize(EntityArray previousEntityArray, IReader reader)
		{
			for (int i = 0; i < this.entityStates.Length; i++) { this.entityStates[i] = (EntityState)reader.ReadByte(); }
			for (int componentTypeID = 0; componentTypeID < this.componentArrays.Count; componentTypeID++)
			{
				this.componentArrays[componentTypeID].Deserialize(previousEntityArray?.componentArrays[componentTypeID], reader);
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates only over the entities in this entity array that exist.
		/// </summary>
		public IEnumerator<Entity> GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		/// Returns an enumerator that iterates only over the entities in this entity array that exist.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
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

		/// <summary>
		/// An enumerator that will enumerate only over the entities that exist within an entity array.
		/// </summary>
		public struct Enumerator : IEnumerator<Entity>, IEnumerator
		{
			#region Fields

			/// <summary>The parent entity array this enumerates over.</summary>
			private readonly EntityArray parent;
			/// <summary>The current entity index the enumerator is sitting on.</summary>
			private int currentEntityID;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Constructor.
			/// </summary>
			public Enumerator(EntityArray parent)
			{
				this.parent = parent;
				this.currentEntityID = -1;
				this.Current = Entity.NoEntity;
			}

			#endregion Constructors

			#region Properties

			/// <summary>
			/// Gets the current entity in the entity array.
			/// </summary>
			public Entity Current { get; private set; }

			/// <summary>
			/// Gets the current element in the entity array.
			/// </summary>
			object IEnumerator.Current { get { return this.Current; } }

			#endregion Properties

			#region Methods

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first entity in the entity array.
			/// </summary>
			public void Reset()
			{
				this.currentEntityID = -1;
				this.Current = Entity.NoEntity;
			}

			/// <summary>
			/// Advances the enumerator to the next entity of the entity array.
			/// </summary>
			public bool MoveNext()
			{
				do
				{
					this.currentEntityID++;
					if (this.currentEntityID >= this.parent.Capacity)
					{
						this.Current = Entity.NoEntity;
						return false;
					}
					else if (this.parent.TryGetEntity(this.currentEntityID, out Entity entity))
					{
						this.Current = entity;
						return true;
					}
				} while (true);
			}

			/// <summary>
			/// Nothing to dispose.
			/// </summary>
			void IDisposable.Dispose()
			{
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
