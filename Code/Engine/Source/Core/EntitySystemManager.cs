using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// The main class for the entire system that manages entities, components, and entity systems.
	/// </summary>
	public sealed class EntitySystemManager
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntitySystemManager(SystemState systemState, IEnumerable<IEntitySystem> entitySystems)
		{
			this.SystemState = systemState;
			this.EntitySystems = entitySystems.ToList().AsReadOnly();
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the system state that this is managing and manipulating.
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
			this.SystemState.BeginUpdate();

			foreach (IEntitySystem entitySystem in this.EntitySystems)
			{
				entitySystem.Update(this.SystemState);
			}

			this.SystemState.EndUpdate();
		}

		#endregion Methods
	}
}
