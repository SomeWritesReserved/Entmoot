using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a system with a single responsibility that cares about the state of entities on the server.
	/// </summary>
	public interface IServerSystem
	{
		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the server.
		/// </summary>
		void Update(EntityArray entityArray);

		#endregion Methods
	}

	/// <summary>
	/// Represents a system with a single responsibility that cares about the state of entities on the client.
	/// </summary>
	public interface IClientSystem
	{
		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the client.
		/// </summary>
		void Update(EntityArray entityArray, int commandingEntityID);

		/// <summary>
		/// Allows this system to perform any rendering.
		/// </summary>
		void Render(EntityArray entityArray, int commandingEntityID);

		#endregion Methods
	}

	/// <summary>
	/// Represents an array of <see cref="IServerSystem"/> objects that can update an <see cref="EntityArray"/>.
	/// </summary>
	public sealed class ServerSystemArray
	{
		#region Fields

		/// <summary>The collection of systems that will update entities and components.</summary>
		private IServerSystem[] systems;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ServerSystemArray(IEnumerable<IServerSystem> systems)
		{
			this.systems = systems.ToArray();
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Updates the systems, allowing for each <see cref="IServerSystem"/> to run its logic on the given <see cref="EntityArray"/>.
		/// </summary>
		public void Update(EntityArray entityArray)
		{
			entityArray.BeginUpdate();
			foreach (IServerSystem system in this.systems)
			{
				system.Update(entityArray);
			}
			entityArray.EndUpdate();
		}

		#endregion Methods
	}

	/// <summary>
	/// Represents an array of <see cref="IClientSystem"/> objects that can update and/or render an <see cref="EntityArray"/>.
	/// </summary>
	public sealed class ClientSystemArray
	{
		#region Fields

		/// <summary>The collection of systems that will update entities.</summary>
		private IClientSystem[] systems;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ClientSystemArray(IEnumerable<IClientSystem> systems)
		{
			this.systems = systems.ToArray();
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Updates the systems, allowing for each <see cref="IClientSystem"/> to run its logic on the given <see cref="EntityArray"/>.
		/// </summary>
		public void Update(EntityArray entityArray, int commandingEntityID)
		{
			entityArray.BeginUpdate();
			foreach (IClientSystem system in this.systems)
			{
				system.Update(entityArray, commandingEntityID);
			}
			entityArray.EndUpdate();
		}

		/// <summary>
		/// Allows for each <see cref="IClientSystem"/> to render the given <see cref="EntityArray"/>.
		/// </summary>
		public void Render(EntityArray entityArray, int commandingEntityID)
		{
			foreach (IClientSystem system in this.systems)
			{
				system.Render(entityArray, commandingEntityID);
			}
		}

		#endregion Methods
	}
}
