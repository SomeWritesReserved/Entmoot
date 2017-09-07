﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		/// <summary>Stores the list of delegates that will construct the different component arrays.</summary>
		private List<Func<int, IComponentArray>> componentArrayCreators = new List<Func<int, IComponentArray>>();

		#endregion Fields

		#region Methods

		/// <summary>
		/// Registers a specific type of component that will be used for by the system.
		/// </summary>
		public void RegisterComponentType<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			this.componentArrayCreators.Add((entityCapacity) => new ComponentArray<TComponent>(entityCapacity));
		}

		/// <summary>
		/// Returns a collection of component arrays for the different component types that have been registered.
		/// </summary>
		public ReadOnlyCollection<IComponentArray> CreateComponentArrays(int entityCapacity)
		{
			return this.componentArrayCreators.Select((componentArrayCreator) => componentArrayCreator(entityCapacity))
				.ToList()
				.AsReadOnly();
		}

		#endregion Methods
	}
}
