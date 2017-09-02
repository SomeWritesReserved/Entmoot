using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public sealed class EntitySystemManager
	{
		#region Fields

		private EntityState[] entityStates;
		private ReadOnlyCollection<IEntitySystem> entitySystems;

		#endregion Fields

		#region Constructors

		public EntitySystemManager(int entityCapacity, IEnumerable<IEntitySystem> entitySystems)
		{
			this.EntityCapacity = entityCapacity;
			this.entityStates = new EntityState[this.EntityCapacity];
			this.entitySystems = entitySystems.ToList().AsReadOnly();
		}

		#endregion Constructors

		#region Properties

		public int EntityCapacity { get; }

		#endregion Properties

		#region Methods

		public bool TryCreateEntity(out Entity entity)
		{
			entity = default(Entity);
			int nextEntityIndex = Array.IndexOf(this.entityStates, EntityState.NoEntity);
			if (nextEntityIndex < 0) { return false; }

			this.entityStates[nextEntityIndex] = EntityState.Creating;
			entity = new Entity(nextEntityIndex);
			return true;
		}

		public void RemoveEntity(Entity entity)
		{
			this.entityStates[entity.ID] = EntityState.Removing;
		}

		public void Update()
		{
			foreach (IEntitySystem entitySystem in this.entitySystems)
			{
				entitySystem.Update();
			}

			// Post-process all entity states, completing creations and removals
			foreach (int entityID in Enumerable.Range(0, this.EntityCapacity))
			{
				if (this.entityStates[entityID] == EntityState.Creating) { this.entityStates[entityID] = EntityState.Active; }
				if (this.entityStates[entityID] == EntityState.Removing) { this.entityStates[entityID] = EntityState.NoEntity; }
			}
		}

		#endregion Methods
	}
}
