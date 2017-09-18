﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// A dumb client that connects to a server host which dictates the state of the simulation.
	/// </summary>
	/// <typeparam name="TCommandData">The type of data that will be sent to the server as a command.</typeparam>
	public class Client<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Fields

		/// <summary>The network connect to the host server.</summary>
		private readonly INetworkConnection serverNetworkConnection;
		/// <summary>The unordered history of entity state snapshots received from the server (note that these are not order but are always the most recent N snapshots).</summary>
		private readonly EntitySnapshot[] entitySnapshotHistory;
		/// <summary>The ordered history of client commands sent to the server.</summary>
		private readonly Queue<ClientCommand<TCommandData>> clientCommandHistory;
		/// <summary>The collection of systems that will update entities.</summary>
		private readonly SystemCollection systemCollection;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Client(INetworkConnection serverNetworkConnection, int maxEntityHistory, int entityCapacity, ComponentsDefinition componentsDefinition, IEnumerable<ISystem> systems)
		{
			this.serverNetworkConnection = serverNetworkConnection;
			this.systemCollection = new SystemCollection(systems);

			// Create the snapshots that will need to be mutated/updates, these need to be separately created to avoid accidentally mutating another snapshot reference
			this.InterpolationStartSnapshot = new EntitySnapshot(entityCapacity, componentsDefinition);
			this.InterpolationEndSnapshot = new EntitySnapshot(entityCapacity, componentsDefinition);
			this.RenderedSnapshot = new EntitySnapshot(entityCapacity, componentsDefinition);

			// Populate the entire history buffer with data that will be overwritten as needed
			this.entitySnapshotHistory = new EntitySnapshot[maxEntityHistory];
			for (int i = 0; i < this.entitySnapshotHistory.Length; i++)
			{
				this.entitySnapshotHistory[i] = new EntitySnapshot(entityCapacity, componentsDefinition);
			}
			this.clientCommandHistory = new Queue<ClientCommand<TCommandData>>();
			for (int i = 0; i < ClientCommand<TCommandData>.MaxClientCommandsPerUpdate; i++)
			{
				this.clientCommandHistory.Enqueue(new ClientCommand<TCommandData>());
			}
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets or sets the rate at which the client will send commands to the server (i.e. every Nth frame commands will be sent).</summary>
		public int NetworkSendRate { get; set; } = 1;
		/// <summary>Gets or sets whether the client should interpolate sent server state for a smoother rendered experience.</summary>
		public bool ShouldInterpolate { get; set; } = true;
		/// <summary>Gets or sets whether to perform client-side prediction on the user's input.</summary>
		public bool ShouldPredictInput { get; set; } = true;
		/// <summary>Gets or sets the delay in frames that the client will use to render interpolated data. This should be at least as large as the server's update rate plus client latency.</summary>
		public int InterpolationRenderDelay { get; set; } = 10;
		/// <summary>Gets or sets the maximum number of ticks that the client can extrapolate for (in the event of packet loss).</summary>
		public int MaxExtrapolationTicks { get; set; } = 10;

		/// <summary>Gets the current frame tick of the client (which may be ahead of the tick that is currently being rendered in <see cref="RenderedSnapshot"/>).</summary>
		public int FrameTick { get; private set; }
		/// <summary>Gets the most recent server tick that we got from the server.</summary>
		public int LatestServerTickReceived { get; private set; } = -1;
		/// <summary>Gets the most recent frame tick we sent that we know was received by the server.</summary>
		public int LatestFrameTickAcknowledgedByServer { get; private set; } = -1;
		/// <summary>Gets the entity that is currently owned by this client (and might take part in client-side prediction).</summary>
		public int CommandingEntityID { get; private set; } = -1;

		/// <summary>Gets whether or not the client has enough data from the server to start rendering and that indeed rendering has begun.</summary>
		public bool HasRenderingStarted { get { return this.RenderedSnapshot.HasData; } }
		/// <summary>Gets whether or not the client has enough data from the server to start interpolation and that indeed interpolation has begun.</summary>
		public bool HasInterpolationStarted { get { return this.InterpolationStartSnapshot.HasData && this.InterpolationEndSnapshot.HasData; } }
		/// <summary>Gets the number of total frames (over the course of the entire session) that had to be extrapolated (instead of interpolated) due to packet loss.</summary>
		public int NumberOfExtrapolatedFrames { get; private set; }
		/// <summary>Gets the number of total frames (over the course of the entire session) that had no interpolation or extrapolation due to severe packet loss and <see cref="MaxExtrapolationTicks"/>.</summary>
		public int NumberOfNoInterpolationFrames { get; private set; }

		/// <summary>Gets the entity snapshot that is currently the actively rendered state.</summary>
		public EntitySnapshot RenderedSnapshot { get; }
		/// <summary>Gets the entity snapshot that is currently being used as the starting interpolation tick (where we are coming from).</summary>
		public EntitySnapshot InterpolationStartSnapshot { get; }
		/// <summary>Gets the entity snapshot that is currently being used as the ending interpolation tick (where we are going to).</summary>
		public EntitySnapshot InterpolationEndSnapshot { get; }

		#endregion Properties

		#region Methods

		public void Update(TCommandData commandData)
		{
			if (this.LatestServerTickReceived >= 0)
			{
				// Only tick the client if we started getting data from the server
				this.FrameTick++;
			}

			byte[] packet;
			while ((packet = this.serverNetworkConnection.GetNextIncomingPacket()) != null)
			{
				// Get the oldest entity snapshot in the history that should be overwritten with the new incoming data, but only if the incoming data is actually newer
				EntitySnapshot newEntitySnapshot = this.getOldestHistoryEntitySnapshot();
				if (!ServerUpdateSerializer.DeserializeIfNewer(packet, newEntitySnapshot, out int newLatestClientTickAcknowledgedByServer, out int newCommandingEntityID)) { continue; }

				if (this.LatestServerTickReceived < 0)
				{
					// This must be our first update from the server, so sync our ticks with the server's ticks once we start getting data flow from the server
					this.FrameTick = newEntitySnapshot.ServerFrameTick;
				}

				if (this.LatestServerTickReceived < newEntitySnapshot.ServerFrameTick)
				{
					// This snapshot is the most recent, up-to-date server update we've gotten so update our state accordingly
					this.LatestServerTickReceived = newEntitySnapshot.ServerFrameTick;
					this.LatestFrameTickAcknowledgedByServer = newLatestClientTickAcknowledgedByServer;
					this.CommandingEntityID = newCommandingEntityID;
				}
			}

			if (this.HasRenderingStarted)
			{
				// Once we are rendering we can start taking user commands and sending them to the server
				// Take the latest command and add it to the command history buffer (overwritting an old command)
				ClientCommand<TCommandData> newClientCommand = this.clientCommandHistory.Dequeue();

				// Todo: handle the frame ticks whenever we aren't interpolating
				newClientCommand.UpdateFrom(this.FrameTick, this.RenderedSnapshot.ServerFrameTick, this.InterpolationStartSnapshot.ServerFrameTick, this.InterpolationEndSnapshot.ServerFrameTick, this.CommandingEntityID, commandData);
				this.clientCommandHistory.Enqueue(newClientCommand);

				if (this.FrameTick % this.NetworkSendRate == 0)
				{
					ClientUpdateSerializer<TCommandData>.Send(this.serverNetworkConnection, this.clientCommandHistory, this.LatestServerTickReceived);
				}
			}

			this.setupRenderSnapshot();

			// Client side prediction
			if (this.ShouldPredictInput && this.HasRenderingStarted && this.CommandingEntityID != -1 && this.RenderedSnapshot.EntityArray.TryGetEntity(this.CommandingEntityID, out Entity predictedEntity))
			{
				// Get the latest entity snapshot in the buffer we will start predicting from
				EntitySnapshot latestHistoryEntitySnapshot = this.getLatestHistoryEntitySnapshot();
				if (latestHistoryEntitySnapshot.EntityArray.TryGetEntity(this.CommandingEntityID, out Entity latestHistoryEntity))
				{
					latestHistoryEntity.CopyTo(predictedEntity);
					foreach (ClientCommand<TCommandData> clientCommand in this.clientCommandHistory)
					{
						// If the command was already received by the server then it doesn't need prediction (the results are in the update sent to us), if the command was for a different
						// entity than what we are currently commanding then we can't predict at all
						if (!clientCommand.HasData || clientCommand.ClientFrameTick <= this.LatestFrameTickAcknowledgedByServer || clientCommand.CommandingEntityID != this.CommandingEntityID) { continue; }

						// Reapply all the commands we've sent that the server hasn't processed yet to get us back to where we predicted we should be, starting
						// from where the server last gave us an authoritative response
						clientCommand.CommandData.ApplyToEntity(predictedEntity);
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void setupRenderSnapshot()
		{
			int renderedFrameTick = this.FrameTick - this.InterpolationRenderDelay;

			if (!this.HasInterpolationStarted)
			{
				// We haven't received enough data from the server yet to start interpolation rendering,
				// so keep polling until we get enough data, once we have enough data we can begin rendering.
				// The snapshot history isn't in any order so we need to check every snapshot for the closest ticks in both directions (start and end)
				EntitySnapshot newInterpolationStartSnapshot = null;
				EntitySnapshot newInterpolationEndSnapshot = null;
				foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
				{
					if (entitySnapshot.ServerFrameTick == -1) { continue; }

					if (entitySnapshot.ServerFrameTick <= renderedFrameTick && (newInterpolationStartSnapshot == null || entitySnapshot.ServerFrameTick > newInterpolationStartSnapshot.ServerFrameTick))
					{
						newInterpolationStartSnapshot = entitySnapshot;
					}
					if (entitySnapshot.ServerFrameTick > renderedFrameTick && (newInterpolationEndSnapshot == null || entitySnapshot.ServerFrameTick < newInterpolationEndSnapshot.ServerFrameTick))
					{
						newInterpolationEndSnapshot = entitySnapshot;
					}
				}
				if (newInterpolationStartSnapshot != null && newInterpolationEndSnapshot != null)
				{
					this.InterpolationStartSnapshot.CopyFrom(newInterpolationStartSnapshot);
					this.InterpolationEndSnapshot.CopyFrom(newInterpolationEndSnapshot);
				}
			}

			// Check to see if we still can't interpolate after going through the latest receieved updates
			if (!this.HasInterpolationStarted) { return; }

			if (renderedFrameTick > this.InterpolationEndSnapshot.ServerFrameTick)
			{
				// Find the next closest entity snapshot to start interpolating to
				// The snapshot history isn't in any order so we need to check every snapshot closest next tick
				EntitySnapshot newInterpolationEndSnapshot = null;
				foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
				{
					if (entitySnapshot.ServerFrameTick > renderedFrameTick && (newInterpolationEndSnapshot == null || entitySnapshot.ServerFrameTick < newInterpolationEndSnapshot.ServerFrameTick))
					{
						newInterpolationEndSnapshot = entitySnapshot;
					}
				}

				if (newInterpolationEndSnapshot != null)
				{
					this.InterpolationStartSnapshot.CopyFrom(this.RenderedSnapshot);
					this.InterpolationEndSnapshot.CopyFrom(newInterpolationEndSnapshot);
				}
			}

			if (this.ShouldInterpolate)
			{
				// Clamp the interpolation frame tick to the maximum number of frames we are allowed to extrapolate, then render that
				// Always make sure the rendered state has the correct rendered frame tick number, even if extrapolation was clamped
				int interpolationFrameTick = Math.Min(renderedFrameTick, this.InterpolationEndSnapshot.ServerFrameTick + this.MaxExtrapolationTicks);
				this.RenderedSnapshot.Interpolate(this.InterpolationStartSnapshot, this.InterpolationEndSnapshot, interpolationFrameTick, renderedFrameTick);

				if (interpolationFrameTick < renderedFrameTick) { this.NumberOfNoInterpolationFrames++; }
				else if (this.InterpolationEndSnapshot.ServerFrameTick < renderedFrameTick) { this.NumberOfExtrapolatedFrames++; }
			}
			else
			{
				// Even though we aren't interpolating, we still want to use whatever the user picked as the render delay so
				// use the end interpolation state and just snap to it
				this.RenderedSnapshot.CopyFrom(this.InterpolationEndSnapshot);
			}
		}

		/// <summary>
		/// Returns the oldest entity snapshot that exists in the history buffer.
		/// </summary>
		private EntitySnapshot getLatestHistoryEntitySnapshot()
		{
			// The snapshot history isn't in any order so we need to check every snapshot
			EntitySnapshot latestEntitySnapshot = this.entitySnapshotHistory[0];
			foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
			{
				if (entitySnapshot.ServerFrameTick > latestEntitySnapshot.ServerFrameTick)
				{
					latestEntitySnapshot = entitySnapshot;
				}
			}
			return latestEntitySnapshot;
		}

		/// <summary>
		/// Returns the oldest entity snapshot that exists in the history buffer.
		/// </summary>
		private EntitySnapshot getOldestHistoryEntitySnapshot()
		{
			// The snapshot history isn't in any order so we need to check every snapshot
			EntitySnapshot oldestEntitySnapshot = this.entitySnapshotHistory[0];
			foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
			{
				if (entitySnapshot.ServerFrameTick < oldestEntitySnapshot.ServerFrameTick)
				{
					oldestEntitySnapshot = entitySnapshot;
				}
			}
			return oldestEntitySnapshot;
		}

		#endregion Methods
	}
}
