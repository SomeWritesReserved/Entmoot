﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// A class to define the various types of components that can be added to entities in an <see cref="EntityArray"/>.
	/// </summary>
	public class ComponentsDefinition
	{
		#region Fields

		/// <summary>Stores the list of delegates that will construct the different component arrays.</summary>
		private readonly List<Func<int, IComponentArray>> componentArrayCreators = new List<Func<int, IComponentArray>>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// Returns the number of currently registered components in this definition.
		/// </summary>
		public int Count
		{
			get { return this.componentArrayCreators.Count; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Registers a specific type of component that can be applied to entities.
		/// </summary>
		public void RegisterComponentType<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			this.componentArrayCreators.Add((capacity) => new ComponentArray<TComponent>(capacity));
		}

		/// <summary>
		/// Returns a collection of component arrays for the different component types that have been registered.
		/// </summary>
		public IComponentArray[] CreateComponentArrays(int capacity)
		{
			return this.componentArrayCreators.Select((componentArrayCreator) => componentArrayCreator(capacity))
				.ToArray();
		}

		#endregion Methods
	}
}
