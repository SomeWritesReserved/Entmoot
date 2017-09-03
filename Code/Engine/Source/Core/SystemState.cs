﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public class SystemState
	{
		#region Fields

		/// <summary>Stores the states for each available entity slot, defining whether an entity exists in a slot or not (as well as any other state info).</summary>
		private EntityState[] entityStates;

		#endregion Fields

		#region Constructors

		public SystemState(int entityCapacity)
		{
			this.EntityCapacity = entityCapacity;
			this.entityStates = new EntityState[this.EntityCapacity];
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
		/// 
		/// </summary>
		public void CommitChanges()
		{
			// Process all entity states, completing creations and removals
			foreach (int entityID in Enumerable.Range(0, this.EntityCapacity))
			{
				if (this.entityStates[entityID] == EntityState.Creating) { this.entityStates[entityID] = EntityState.Active; }
				if (this.entityStates[entityID] == EntityState.Removing) { this.entityStates[entityID] = EntityState.NoEntity; }
			}
		}

		#endregion Methods
	}
}