using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents an individual entity, addressed by a unique ID. The entity and its components are managed
	/// by an owning <see cref="EntityArray"/>.
	/// </summary>
	public struct Entity
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		internal Entity(int id)
		{
			this.ID = id;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the unique identifier for the entity.
		/// </summary>
		public int ID { get; }

		#endregion Properties
	}
}
