using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public enum EntityState : byte
	{
		/// <summary>The entity does not exist (undefined state or not part of the game).</summary>
		NoEntity = 0,
		/// <summary>The entity is in the process of being created and hasn't been added to the game yet, it will be fully added at the end of the next update.</summary>
		Creating,
		/// <summary>The entity is actively part of the game.</summary>
		Active,
		/// <summary>The entity is still actively part of the game but is scheduled to be removed at the end of the next update.</summary>
		Removing,
	}
}
