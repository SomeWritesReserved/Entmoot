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

	/// <summary>
	/// Represents a system that will handle and process client commands.
	/// </summary>
	/// <remarks>
	/// Not all IClientCommandedSystem implementers care about both <see cref="ProcessClientCommand"/> and <see cref="PredictClientCommand"/>.
	/// </remarks>
	/// <typeparam name="TCommandData">The type of data that is used as a command.</typeparam>
	public interface IClientCommandedSystem<TCommandData> : ISystem
		where TCommandData : struct, ICommandData
	{
		#region Methods

		/// <summary>
		/// Processes the given client command on the given commanding entity. This happens server-side to execute a client's command.
		/// </summary>
		void ProcessClientCommand(EntityArray currentEntityArray, TCommandData commandData, Entity commandingEntity, EntityArray lagCompensatedEntityArray);

		/// <summary>
		/// Predicts the result of the the given client command on the given commanding entity. This happens client-side to predict the results of a client's command immediately.
		/// </summary>
		void PredictClientCommand(EntityArray entityArray, TCommandData commandData, Entity commandingEntity);

		#endregion Methods
	}
}
