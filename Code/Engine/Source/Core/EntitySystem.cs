using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a system with a single responsibility that will update the state of entities and components.
	/// </summary>
	public interface IEntitySystem
	{
		#region Methods

		void Update(SystemState systemState);

		#endregion Methods
	}
}
