﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a specific aspect or facet of data that entities can take on, to be  managed by a component array.
	/// </summary>
	public interface IComponent<TComponent>
	{
		#region Methods

		/// <summary>
		/// Updates this component to an interpolated value between two other components.
		/// </summary>
		void Interpolate(TComponent otherA, TComponent otherB, float amount);

		/// <summary>
		/// Writes the state of this specific component to a binary source.
		/// </summary>
		void Serialize(BinaryWriter binaryWriter);

		/// <summary>
		/// Reads and overwrites the current state of this specific component from a binary source.
		/// </summary>
		void Deserialize(BinaryReader binaryReader);

		#endregion Methods
	}

	/// <summary>
	/// Represents an array of identically typed components that define which entities have a specific component type.
	/// </summary>
	public interface IComponentArray
	{
		#region Properties

		/// <summary>
		/// Gets the maximum number of entities that can have this type of component.
		/// </summary>
		int Capacity { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns whether the given entity has this specific type of component.
		/// </summary>
		bool HasComponent(Entity entity);

		/// <summary>
		/// Removes this specific type of component from the given entity.
		/// </summary>
		void RemoveComponent(Entity entity);

		/// <summary>
		/// Copies all component data to another component array.
		/// </summary>
		void CopyTo(IComponentArray other);

		/// <summary>
		/// Copies a single entity's component data to another entity's components in another component array.
		/// </summary>
		void CopyTo(int thisEntityID, IComponentArray otherComponentArray, int otherEntityID);

		/// <summary>
		/// Updates all component data to interpolated values between two other components.
		/// </summary>
		void Interpolate(IComponentArray otherA, IComponentArray otherB, float amount);

		/// <summary>
		/// Writes the state of all component data to a binary source.
		/// </summary>
		void Serialize(BinaryWriter binaryWriter);

		/// <summary>
		/// Reads and overwrites the current state of all component data from a binary source.
		/// </summary>
		void Deserialize(BinaryReader binaryReader);

		#endregion Methods
	}

	/// <summary>
	/// Represents an array of identically typed components that define which entities have a <see cref="TComponent"/>.
	/// </summary>
	public sealed class ComponentArray<TComponent> : IComponentArray
		where TComponent : struct, IComponent<TComponent>
	{
		#region Fields

		/// <summary>Stores the array of individual components that will be indexed into by entity ID to get the component value.</summary>
		private readonly TComponent[] components;
		/// <summary>Stores whether or not this component type has been added to a given entity ID.</summary>
		private readonly StateArray componentStates;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ComponentArray(int capacity)
		{
			this.Capacity = capacity;
			this.components = new TComponent[this.Capacity];
			this.componentStates = new StateArray(this.Capacity);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the maximum number of entities that can have this type of component.
		/// </summary>
		public int Capacity { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns whether the given entity has this specific type of component.
		/// </summary>
		public bool HasComponent(Entity entity)
		{
			return this.componentStates[entity.ID];
		}

		/// <summary>
		/// Returns a reference to a component that the given entity has (or could have). Be aware
		/// that this will return a component reference even if this component type hasn't been
		/// added to the entity.
		/// </summary>
		public ref TComponent GetComponent(Entity entity)
		{
			return ref this.components[entity.ID];
		}

		/// <summary>
		/// Adds this specific type of component to the given entity (if it doesn't already have one) and
		/// returns a reference to the component. This is safe to call even if this component type has
		/// already been added to the entity.
		/// </summary>
		public ref TComponent AddComponent(Entity entity)
		{
			// Don't default the entity state here so consumers can call this as an easy way to get a reference
			// to the component and make sure its added to the entity (without having to check manually if the 
			// component doesn't exit to then call GetComponent)
			this.componentStates[entity.ID] = true;
			return ref this.components[entity.ID];
		}

		/// <summary>
		/// Removes this specific type of component from the given entity.
		/// </summary>
		public void RemoveComponent(Entity entity)
		{
			this.components[entity.ID] = default(TComponent);
			this.componentStates[entity.ID] = false;
		}

		/// <summary>
		/// Copies all <see cref="TComponent"/> data to another component array.
		/// </summary>
		public void CopyTo(ComponentArray<TComponent> other)
		{
			Array.Copy(this.components, other.components, this.Capacity);
			this.componentStates.CopyTo(other.componentStates);
		}

		/// <summary>
		/// Copies all component data to another component array.
		/// </summary>
		void IComponentArray.CopyTo(IComponentArray other)
		{
			this.CopyTo((ComponentArray<TComponent>)other);
		}

		/// <summary>
		/// Copies a single entity's <see cref="TComponent"/> data to another entity's components in another component array.
		/// </summary>
		public void CopyTo(int thisEntityID, ComponentArray<TComponent> otherComponentArray, int otherEntityID)
		{
			otherComponentArray.components[otherEntityID] = this.components[thisEntityID];
			otherComponentArray.componentStates[otherEntityID] = this.componentStates[thisEntityID];
		}

		/// <summary>
		/// Copies a single entity's component data to another entity's components in another component array.
		/// </summary>
		void IComponentArray.CopyTo(int thisEntityID, IComponentArray otherComponentArray, int otherEntityID)
		{
			this.CopyTo(thisEntityID, (ComponentArray<TComponent>)otherComponentArray, otherEntityID);
		}

		/// <summary>
		/// Updates all <see cref="TComponent"/> data to interpolated values between two other components.
		/// </summary>
		public void Interpolate(ComponentArray<TComponent> otherA, ComponentArray<TComponent> otherB, float amount)
		{
			Array.Copy(otherA.components, this.components, this.Capacity);
			otherA.componentStates.CopyTo(this.componentStates);
			for (int componentIndex = 0; componentIndex < this.Capacity; componentIndex++)
			{
				this.components[componentIndex].Interpolate(otherA.components[componentIndex], otherB.components[componentIndex], amount);
			}
		}

		/// <summary>
		/// Updates all component data to interpolated values between two other components.
		/// </summary>
		void IComponentArray.Interpolate(IComponentArray otherA, IComponentArray otherB, float amount)
		{
			this.Interpolate((ComponentArray<TComponent>)otherA, (ComponentArray<TComponent>)otherB, amount);
		}

		/// <summary>
		/// Writes the state of all component data to a binary source.
		/// </summary>
		public void Serialize(BinaryWriter binaryWriter)
		{
			this.componentStates.Serialize(binaryWriter);
			for (int componentIndex = 0; componentIndex < this.Capacity; componentIndex++)
			{
				this.components[componentIndex].Serialize(binaryWriter);
			}
		}

		/// <summary>
		/// Reads and overwrites the current state of all component data from a binary source.
		/// </summary>
		public void Deserialize(BinaryReader binaryReader)
		{
			this.componentStates.Deserialize(binaryReader);
			for (int componentIndex = 0; componentIndex < this.Capacity; componentIndex++)
			{
				this.components[componentIndex].Deserialize(binaryReader);
			}
		}

		#endregion Methods
	}
}
