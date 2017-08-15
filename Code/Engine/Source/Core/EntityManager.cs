using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public sealed class EntityManager
	{
		#region Fields

		private Entity[] entities;
		private IList<IEntitySystem> entitySystems;
		private List<Entity> createdEntities;
		private List<Entity> removedEntities;

		#endregion Fields

		#region Constructors

		public EntityManager(int entityCapacity, IEntitySystem[] entitySystems)
		{
			this.entities = new Entity[entityCapacity];
			this.entitySystems = entitySystems.ToList().AsReadOnly();
			this.createdEntities = new List<Entity>(64);
			this.removedEntities = new List<Entity>(64);
		}

		#endregion Constructors

		#region Methods

		public TEntity CreateEntity<TEntity>()
			where TEntity : Entity, new()
		{
			// Todo: should we specifically not reuse entity IDs to avoid networking confusion?
			int nextEntityIndex = Array.IndexOf(this.entities, null);
			if (nextEntityIndex < 0) { return null; }

			TEntity newEntity = new TEntity();
			newEntity.ID = nextEntityIndex;
			this.entities[nextEntityIndex] = newEntity;
			this.createdEntities.Add(newEntity);
			return newEntity;
		}

		public void RemoveEntity(Entity entity)
		{
			this.entities[entity.ID] = null;
			this.removedEntities.Add(entity);
		}

		public void Update()
		{
			foreach (IEntitySystem entitySystem in this.entitySystems)
			{
				entitySystem.Update();
			}
			foreach (Entity createdEntity in this.createdEntities)
			{
				foreach (IEntitySystem entitySystem in this.entitySystems)
				{
					entitySystem.OnEntityCreated(createdEntity);
				}
			}
			foreach (Entity removedEntity in this.removedEntities)
			{
				foreach (IEntitySystem entitySystem in this.entitySystems)
				{
					entitySystem.OnEntityRemoved(removedEntity);
				}
			}
			this.createdEntities.Clear();
			this.removedEntities.Clear();
		}

		#endregion Methods
	}
}
