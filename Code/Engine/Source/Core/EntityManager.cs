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
		private IList<IEntitySystem> entitySystem;
		private List<Entity> newEntities;

		#endregion Fields

		#region Constructors

		public EntityManager(int entityCapacity, IEntitySystem[] entitySystem)
		{
			this.entities = new Entity[entityCapacity];
			this.entitySystem = entitySystem.ToList().AsReadOnly();
			this.newEntities = new List<Entity>(64);
		}

		#endregion Constructors

		#region Methods

		public TEntity CreateEntity<TEntity>()
			where TEntity : Entity, new()
		{
			int entityIndex = Array.IndexOf(this.entities, null);
			if (entityIndex < 0) { return null; }

			// Todo: should we specifically not reuse entity IDs to avoid networking confusion?
			TEntity newEntity = new TEntity();
			newEntity.ID = entityIndex;
			this.entities[entityIndex] = newEntity;
			this.newEntities.Add(newEntity);
			return newEntity;
		}

		public void Update()
		{
			foreach (IEntitySystem entitySystem in this.entitySystem)
			{
				entitySystem.Update();
			}
			// Todo: add new entities to systems and remove entities
		}

		#endregion Methods
	}
}
