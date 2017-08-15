using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public sealed class EntityManager : IEntityCollection
	{
		#region Fields

		private Entity[] entities;
		private ReadOnlyCollection<IEntitySystem> entitySystems;
		private List<Entity> createdEntities;
		private List<Entity> removedEntities;

		#endregion Fields

		#region Constructors

		public EntityManager(int entityCapacity, IEnumerable<IEntitySystem> entitySystems)
		{
			this.entities = new Entity[entityCapacity];
			this.entitiesAsReadOnly = new ReadOnlyCollection<Entity>(this.entities);
			this.entitySystems = entitySystems.ToList().AsReadOnly();
			this.createdEntities = new List<Entity>(64);
			this.removedEntities = new List<Entity>(64);
		}

		#endregion Constructors

		#region Properties

		private ReadOnlyCollection<Entity> entitiesAsReadOnly;
		public ReadOnlyCollection<Entity> Entities
		{
			get { return this.entitiesAsReadOnly; }
		}

		#endregion Properties

		#region Methods

		public TEntity CreateEntity<TEntity>()
			where TEntity : Entity, new()
		{
			// Todo: should we specifically not reuse entity IDs to avoid networking confusion?
			int nextEntityIndex = Array.IndexOf(this.entities, null);
			if (nextEntityIndex < 0) { return null; }

			TEntity newEntity = new TEntity();
			newEntity.ID = nextEntityIndex;
			this.createdEntities.Add(newEntity);
			return newEntity;
		}

		public void RemoveEntity(Entity entity)
		{
			this.removedEntities.Add(entity);
		}

		public void Update()
		{
			foreach (IEntitySystem entitySystem in this.entitySystems)
			{
				entitySystem.Update(this);
			}

			foreach (Entity createdEntity in this.createdEntities)
			{
				this.entities[createdEntity.ID] = createdEntity;
			}
			this.createdEntities.Clear();
			foreach (Entity removedEntity in this.removedEntities)
			{
				this.entities[removedEntity.ID] = null;
			}
			this.removedEntities.Clear();
		}

		#endregion Methods
	}

	public interface IEntityCollection
	{
		ReadOnlyCollection<Entity> Entities { get; }
	}
}
