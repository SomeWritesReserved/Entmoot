using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a collection of <see cref="ISystem"/> objects that can update an <see cref="EntityArray"/>.
	/// </summary>
	public sealed class SystemCollection
	{
		#region Fields

		/// <summary>The collection of systems that will update entities and components.</summary>
		private ISystem[] systems;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public SystemCollection(IEnumerable<ISystem> systems)
		{
			this.systems = systems.ToArray();
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Updates the systems, allowing for each <see cref="ISystem"/> to run its logic on the given <see cref="EntityArray"/>.
		/// </summary>
		public void Update(EntityArray entityArray)
		{
			entityArray.BeginUpdate();
			foreach (ISystem system in this.systems)
			{
				system.Update(entityArray);
			}
			entityArray.EndUpdate();
		}

		/// <summary>
		/// Updates the IClientCommandedSystem systems, allowing for each to process the given client command on the given commanding entity.
		/// </summary>
		/// <typeparam name="TCommandData">The type of data that is used as a command.</typeparam>
		public void ProcessClientCommand<TCommandData>(EntityArray currentEntityArray, TCommandData commandData, Entity commandingEntity, EntityArray lagCompensatedEntityArray)
			where TCommandData : struct, ICommandData
		{
			foreach (ISystem system in this.systems)
			{
				IClientCommandedSystem<TCommandData> clientCommandedSystem = system as IClientCommandedSystem<TCommandData>;
				if (clientCommandedSystem != null)
				{
					clientCommandedSystem.ProcessClientCommand(currentEntityArray, commandData, commandingEntity, lagCompensatedEntityArray);
				}
			}
		}

		/// <summary>
		/// Updates the IClientCommandedSystem systems, allowing for each to predict the result of the given client command on the given commanding.
		/// </summary>
		/// <typeparam name="TCommandData">The type of data that is used as a command.</typeparam>
		public void PredictClientCommand<TCommandData>(EntityArray entityArray, TCommandData commandData, Entity commandingEntity)
			where TCommandData : struct, ICommandData
		{
			foreach (ISystem system in this.systems)
			{
				IClientCommandedSystem<TCommandData> clientCommandedSystem = system as IClientCommandedSystem<TCommandData>;
				if (clientCommandedSystem != null)
				{
					clientCommandedSystem.PredictClientCommand(entityArray, commandData, commandingEntity);
				}
			}
		}

		#endregion Methods
	}
}
