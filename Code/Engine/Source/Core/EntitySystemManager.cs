using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// The main class that manages entities, components, and systems.
	/// </summary>
	public sealed class EntitySystemManager
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntitySystemManager(int entityCapacity, IEnumerable<IEntitySystem> entitySystems)
		{
			this.SystemState = new SystemState(entityCapacity);
			this.EntitySystems = entitySystems.ToList().AsReadOnly();
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the overall system state of all managed entities and components.
		/// </summary>
		public SystemState SystemState { get; }

		/// <summary>
		/// Gets the collection of entity systems that will update the entities and components.
		/// </summary>
		public ReadOnlyCollection<IEntitySystem> EntitySystems { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates the systems, allowing for each <see cref="IEntitySystem"/> to run its logic.
		/// </summary>
		public void Update()
		{
			foreach (IEntitySystem entitySystem in this.EntitySystems)
			{
				entitySystem.Update(this.SystemState);
			}

			this.SystemState.CommitChanges();
		}

		#endregion Methods
	}
}
