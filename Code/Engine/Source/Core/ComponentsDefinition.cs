using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// A class to define the various types of components that will be used within the system.
	/// </summary>
	public class ComponentsDefinition
	{
		#region Fields

		/// <summary>Stores the list of delegates that will construct different types of component collections.</summary>
		private List<Func<int, IComponentCollection>> collectionCreators = new List<Func<int, IComponentCollection>>();

		#endregion Fields

		#region Methods

		/// <summary>
		/// Registers a specific type of component that will be used for by the system.
		/// </summary>
		public void RegisterComponentType<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			this.collectionCreators.Add((entityCapacity) => new ComponentCollection<TComponent>(entityCapacity));
		}

		/// <summary>
		/// Returns an array of collections for all the component types that have been registered.
		/// </summary>
		public IComponentCollection[] CreateComponentCollections(int entityCapacity)
		{
			return this.collectionCreators.Select((collectionCreator) => collectionCreator(entityCapacity))
				.ToArray();
		}

		#endregion Methods
	}
}
