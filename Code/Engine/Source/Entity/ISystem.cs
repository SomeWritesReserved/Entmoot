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
	public interface ISystem
	{
		#region Methods

		/// <summary>
		/// Processes this system over the given array of entities.
		/// </summary>
		void Update(EntityArray entityArray);

		#endregion Methods
	}

	public interface IClientCommandedSystem<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Methods
		
		void ProcessClientCommand(EntityArray entityArray, TCommandData commandData, Entity commandingEntity, EntitySnapshot lagCompensationSnapshot);

		#endregion Methods
	}
}
