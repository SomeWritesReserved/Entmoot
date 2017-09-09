using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a collection of <see cref="ISystem"/> objects that can update an <see cref="EntityArray"/>.
	/// </summary>
	public sealed class SystemCollection
	{
		#region Fields

		/// <summary>The collection of systems that will update entities and components.</summary>
		private ReadOnlyCollection<ISystem> systems;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public SystemCollection(IEnumerable<ISystem> systems)
		{
			this.systems = systems.ToList().AsReadOnly();
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Updates the systems, allowing for each <see cref="ISystem"/> to run its logic on the given <see cref="EntityArray"/>.
		/// </summary>
		public void Update(EntityArray entityArray)
		{
			entityArray.BeginUpdate();
			foreach (ISystem system in this.systems)
			{
				system.Update(entityArray);
			}
			entityArray.EndUpdate();
		}

		#endregion Methods
	}
}
