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

		#endregion Methods
	}

	public sealed class ClientCommandedSystemCollection<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Fields

		private IClientCommandedSystem<TCommandData>[] clientCommandedSystems;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ClientCommandedSystemCollection(IEnumerable<IClientCommandedSystem<TCommandData>> clientCommandedSystems)
		{
			this.clientCommandedSystems = clientCommandedSystems.ToArray();
		}

		#endregion Constructors

		#region Methods

		public void ProcessClientCommand(EntityArray entityArray, TCommandData commandData, Entity commandingEntity, EntitySnapshot lagCompensationSnapshot)
		{
			foreach (IClientCommandedSystem<TCommandData> clientCommandedSystem in this.clientCommandedSystems)
			{
				clientCommandedSystem.ProcessClientCommand(entityArray, commandData, commandingEntity, lagCompensationSnapshot);
			}
		}

		#endregion Methods
	}
}
