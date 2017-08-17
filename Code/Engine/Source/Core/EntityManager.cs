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
		private List<Entity> createdEntities = new List<Entity>(16);
		private List<Entity> removedEntities = new List<Entity>(16);

		#endregion Fields

		#region Constructors

		public EntityManager(int entityCapacity, IEnumerable<IEntitySystem> entitySystems)
		{
			this.entities = new Entity[entityCapacity];
			this.entitySystems = entitySystems.ToList().AsReadOnly();
		}

		#endregion Constructors

		#region Properties

		public ReadOnlyCollection<Entity> Entities
		{
			get { return this.entities.Where((entity) => entity != null && entity.EntityState != EntityState.Creating).ToList().AsReadOnly(); }
		}

		#endregion Properties

		#region Methods

		public TEntity CreateEntity<TEntity>()
			where TEntity : Entity, new()
		{
			int nextEntityIndex = Array.IndexOf(this.entities, null);
			if (nextEntityIndex < 0) { return null; }

			TEntity newEntity = new TEntity();
			newEntity.ID = nextEntityIndex;
			newEntity.EntityState = EntityState.Creating;
			this.entities[nextEntityIndex] = newEntity;
			this.createdEntities.Add(newEntity);
			return newEntity;
		}

		public void RemoveEntity(Entity entity)
		{
			if (entity == null || entity.EntityState == EntityState.NoState) { throw new ArgumentNullException(nameof(entity)); }
			if (this.entities[entity.ID] != null && this.entities[entity.ID] != entity) { throw new InvalidOperationException("Bad removing entity; removing entity doesn't match existing entity."); }
			entity.EntityState = EntityState.Removing;
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
				createdEntity.EntityState = EntityState.Active;
			}
			this.createdEntities.Clear();
			foreach (Entity removedEntity in this.removedEntities)
			{
				this.entities[removedEntity.ID] = null;
				removedEntity.EntityState = EntityState.NoState;
			}
			this.removedEntities.Clear();
		}

		#endregion Methods
	}
}
