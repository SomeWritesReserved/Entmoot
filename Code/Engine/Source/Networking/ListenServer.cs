using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// A server that is also a local client to itself (for local hosting).
	/// </summary>
	/// <typeparam name="TCommandData">The type of data expected from clients as a command.</typeparam>
	public class ListenServer<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ListenServer(int maxMessageSize, int maxEntityHistory, int entityCapacity, ComponentsDefinition componentsDefinition, IEnumerable<ISystem> serverSystems, IEnumerable<ISystem> clientSystems)
		{
			LocalNetworkConnection clientNetworkConnection = new LocalNetworkConnection(maxMessageSize);
			LocalNetworkConnection serverNetworkConnection = clientNetworkConnection.GetPairedNetworkConnection();
			this.GameServer = new GameServer<TCommandData>(new[] { clientNetworkConnection }, maxEntityHistory, entityCapacity, componentsDefinition, serverSystems);
			this.GameClient = new GameClient<TCommandData>(serverNetworkConnection, maxEntityHistory, entityCapacity, componentsDefinition, clientSystems);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the underlying server object for this listen server.
		/// </summary>
		public GameServer<TCommandData> GameServer { get; }

		/// <summary>
		/// Gets the underlying client object for this listen server.
		/// </summary>
		public GameClient<TCommandData> GameClient { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates the server state by processing client input (and local client input), updating entities, and sending state to clients.
		/// Also updates the local client state by updating rendered frame (with interpolation and prediction).
		/// </summary>
		public void Update(TCommandData commandData)
		{
			this.GameServer.Update();
			this.GameClient.Update(commandData);
		}

		#endregion Methods
	}
}
