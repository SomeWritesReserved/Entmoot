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
		void ServerUpdate(EntityArray entityArray);

		#endregion Methods
	}

	/// <summary>
	/// Represents an <see cref="IServerSystem"/> that takes client commands and applies them to the client's commanding entity on the server.
	/// </summary>
	public interface IServerCommandProcessorSystem<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Methods

		/// <summary>
		/// Updates this system for a specific client's command on a commanding entity. The provided lag compensated entity array might be null
		/// but otherwise contains the rough state of the server at the time the client issued the command (the client's render frame).
		/// </summary>
		void ProcessClientCommand(EntityArray entityArray, Entity commandingEntity, TCommandData commandData, EntityArray lagCompensatedEntityArray);

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
		void ClientUpdate(EntityArray entityArray, Entity commandingEntity);

		/// <summary>
		/// Allows this system to perform any rendering.
		/// </summary>
		void ClientRender(EntityArray entityArray, Entity commandingEntity);

		#endregion Methods
	}

	/// <summary>
	/// Represents an <see cref="IClientSystem"/> that can actively participate in client-side prediction with a client's command on their commanded entity.
	/// </summary>
	public interface IClientPredictedSystem<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Methods

		/// <summary>
		/// Runs this system over the given array of entities on the client but only updates the commanding entity (for client-side prediction of a command).
		/// </summary>
		void PredictClientCommand(EntityArray entityArray, Entity commandingEntity, TCommandData commandData);

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
		public void ServerUpdate(EntityArray entityArray)
		{
			entityArray.BeginUpdate();
			foreach (IServerSystem system in this.systems)
			{
				system.ServerUpdate(entityArray);
			}
			entityArray.EndUpdate();
		}

		/// <summary>
		/// Updates the systems for a specific client's command on a commanding entity. The provided lag compensated entity array might be null
		/// but otherwise contains the rough state of the server at the time the client issued the command (the client's render frame).
		/// </summary>
		public void ProcessClientCommand<TCommandData>(EntityArray entityArray, Entity commandingEntity, TCommandData commandData, EntityArray lagCompensatedEntityArray)
			where TCommandData : struct, ICommandData
		{
			entityArray.BeginUpdate();
			foreach (IServerSystem system in this.systems)
			{
				if (system is IServerCommandProcessorSystem<TCommandData> serverCommandProcessorSystem)
				{
					serverCommandProcessorSystem.ProcessClientCommand(entityArray, commandingEntity, commandData, lagCompensatedEntityArray);
				}
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
		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
			entityArray.BeginUpdate();
			foreach (IClientSystem system in this.systems)
			{
				system.ClientUpdate(entityArray, commandingEntity);
			}
			entityArray.EndUpdate();
		}

		/// <summary>
		/// Allows for each <see cref="IClientSystem"/> to render the given <see cref="EntityArray"/>.
		/// </summary>
		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
			foreach (IClientSystem system in this.systems)
			{
				system.ClientRender(entityArray, commandingEntity);
			}
		}

		/// <summary>
		/// Updates the systems, allowing for each <see cref="IClientSystem"/> to run its logic to do client-side prediction on the commanding entity
		/// with the specified command.
		/// </summary>
		public void PredictClientCommand<TCommandData>(EntityArray entityArray, Entity commandingEntity, TCommandData commandData)
			where TCommandData : struct, ICommandData
		{
			foreach (IClientSystem system in this.systems)
			{
				if (system is IClientPredictedSystem<TCommandData> clientPredictedSystem)
				{
					clientPredictedSystem.PredictClientCommand(entityArray, commandingEntity, commandData);
				}
			}
		}

		#endregion Methods
	}
}
