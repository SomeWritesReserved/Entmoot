using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Defines the current state of an entity.
	/// </summary>
	public enum EntityState : byte
	{
		/// <summary>The entity does not exist (undefined state or not part of the system).</summary>
		NoEntity = 0,
		/// <summary>The entity is in the process of being created and hasn't been completely added to the system yet, it will be fully added at the end of the next update.</summary>
		Creating,
		/// <summary>The entity is actively part of the system.</summary>
		Active,
		/// <summary>The entity is still actively part of the system but is scheduled to be removed at the end of the next update.</summary>
		Removing,
	}
}
