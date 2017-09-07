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
		public EntitySystemManager(EntityArray entityArray, IEnumerable<ISystem> systems)
		{
			this.Entities = entityArray;
			this.Systems = systems.ToList().AsReadOnly();
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the system state that this is managing and manipulating.
		/// </summary>
		public EntityArray Entities { get; }

		/// <summary>
		/// Gets the collection of systems that will update the entities and components.
		/// </summary>
		public ReadOnlyCollection<ISystem> Systems { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates the systems, allowing for each <see cref="IEntitySystem"/> to run its logic.
		/// </summary>
		public void Update()
		{
			this.Entities.BeginUpdate();

			foreach (ISystem system in this.Systems)
			{
				system.Update(this.Entities);
			}

			this.Entities.EndUpdate();
		}

		#endregion Methods
	}
}
