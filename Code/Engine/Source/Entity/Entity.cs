using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an individual entity, addressed by a unique ID. The entity and its components are managed
	/// by a parent <see cref="EntityArray"/>.
	/// </summary>
	public struct Entity
	{
		#region Fields

		/// <summary>Represents a reference to an entity that does not actually exist.</summary>
		public static readonly Entity NoEntity = new Entity(null, -1);

		/// <summary>The parent array that actually handles all of the logic for this and all other child entities.</summary>
		private readonly EntityArray parentEntityArray;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		internal Entity(EntityArray parentEntityArray, int id)
		{
			this.parentEntityArray = parentEntityArray;
			this.ID = id;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the unique identifier for the entity.
		/// </summary>
		public int ID { get; }

		/// <summary>
		/// Gets whether or not this entity is a valid reference to an entity in an entity array.
		/// </summary>
		public bool IsValid
		{
			get { return this.ID >= 0 && this.parentEntityArray != null; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns whether this entity has the specific type of component.
		/// </summary>
		public bool HasComponent<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			return this.parentEntityArray.GetComponentArray<TComponent>().HasComponent(this);
		}

		/// <summary>
		/// Returns a reference to a component that this entity has (or could have). Be aware
		/// that this will return a component reference even if the component type hasn't been
		/// added to this entity.
		/// </summary>
		public ref TComponent GetComponent<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			return ref this.parentEntityArray.GetComponentArray<TComponent>().GetComponent(this);
		}

		/// <summary>
		/// Adds the specific type of component to this entity (if it doesn't already have one) and
		/// returns a reference to the component. This is safe to call even if the component type has
		/// already been added to this entity.
		/// </summary>
		public ref TComponent AddComponent<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			return ref this.parentEntityArray.GetComponentArray<TComponent>().AddComponent(this);
		}

		/// <summary>
		/// Removes the specific type of component from this entity.
		/// </summary>
		public void RemoveComponent<TComponent>()
			where TComponent : struct, IComponent<TComponent>
		{
			this.parentEntityArray.GetComponentArray<TComponent>().RemoveComponent(this);
		}

		/// <summary>
		/// Copies this entity and its component data to another entity (the other entity need not be
		/// in the same entity array).
		/// </summary>
		public void CopyTo(Entity other)
		{
			this.parentEntityArray.CopyEntityTo(this.ID, other.parentEntityArray, other.ID);
		}

		#endregion Methods
	}
}
