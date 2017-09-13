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
		/// <summary>The history of entity state snapshots received from the server, note that these are not in any order at all.</summary>
		private readonly EntitySnapshot[] entitySnapshotHistory;
		/// <summary>The history of client commands recoreded and sent to the server.</summary>
		private readonly Queue<ClientCommand<TCommandData>> clientCommandHistory;
		/// <summary>The collection of systems that will update entities.</summary>
		private readonly SystemCollection systemCollection;
		/// <summary>The snapshot to use purely for deserializing incoming packets, gets reused/overwritten for every new packet.</summary>
		private readonly EntitySnapshot deserializedEntitySnapshot;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Client(INetworkConnection serverNetworkConnection, int maxHistory, int entityCapacity, ComponentsDefinition componentsDefinition, IEnumerable<ISystem> systems)
		{
			this.serverNetworkConnection = serverNetworkConnection;
			this.systemCollection = new SystemCollection(systems);

			// Create the snapshots that will need to be mutated/updates, these need to be separately created to avoid accidentally mutating another snapshot reference
			this.InterpolationStartSnapshot = new EntitySnapshot(entityCapacity, componentsDefinition);
			this.InterpolationEndSnapshot = new EntitySnapshot(entityCapacity, componentsDefinition);
			this.RenderedSnapshot = new EntitySnapshot(entityCapacity, componentsDefinition);
			this.deserializedEntitySnapshot = new EntitySnapshot(entityCapacity, componentsDefinition);

			// Populate the whole history buffer with data that will be overwritten as needed
			this.entitySnapshotHistory = new EntitySnapshot[maxHistory];
			this.clientCommandHistory = new Queue<ClientCommand<TCommandData>>(maxHistory);
			for (int i = 0; i < maxHistory; i++)
			{
				this.entitySnapshotHistory[i] = new EntitySnapshot(entityCapacity, componentsDefinition);
				this.clientCommandHistory.Enqueue(new ClientCommand<TCommandData>());
			}
		}

		#endregion Constructors

		#region Properties

		/// <summary>Gets or sets whether the client should interpolate sent server state for a smoother rendered experience.</summary>
		public bool ShouldInterpolate { get; set; } = true;
		/// <summary>Gets or sets whether to perform client-side prediction on the user's input.</summary>
		public bool ShouldPredictInput { get; set; } = true;
		/// <summary>Gets or sets the delay in frames that the client will use to render interpolated data. This should be at least as large as the server's update rate plus client latency.</summary>
		public int InterpolationRenderDelay { get; set; } = 10;
		/// <summary>Gets or sets the maximum number of ticks that the client can extrapolate for (in the event of packet loss).</summary>
		public int MaxExtrapolationTicks { get; set; } = 10;

		/// <summary>Gets the current frame tick of the client (which may or may not be ahead of the tick that is currently being rendered).</summary>
		public int FrameTick { get; private set; }
		/// <summary>Gets the last server tick that was actually received from the server.</summary>
		public int LatestServerTickAcknowledgedByClient { get; private set; } = -1;
		/// <summary>Gets the last client tick that was acknowledged by the server.</summary>
		public int LatestClientTickAcknowledgedByServer { get; private set; } = -1;
		/// <summary>Gets the entity that is currently owned by this client (and might take part in client-side prediction).</summary>
		public int OwnedEntity { get; private set; } = -1;

		/// <summary>Gets whether or not the client has enough data from the server to start interpolation and that indeed interpolation has begun.</summary>
		public bool HasInterpolationStarted { get { return (this.InterpolationStartSnapshot.ServerFrameTick != -1 && this.InterpolationEndSnapshot.ServerFrameTick != -1); } }
		/// <summary>Gets the number of total frames (over the course of the entire session) that had to be extrapolated (instead of interpolated) due to packet loss.</summary>
		public int NumberOfExtrapolatedFrames { get; private set; }
		/// <summary>Gets the number of total frames (over the course of the entire session) that had no interpolation or extrapolation due to severe packet loss and <see cref="MaxExtrapolationTicks"/>.</summary>
		public int NumberOfNoInterpolationFrames { get; private set; }

		/// <summary>Gets the entity snapshot that is currently being used as the starting interpolation tick (where we are coming from).</summary>
		public EntitySnapshot InterpolationStartSnapshot { get; }
		/// <summary>Gets the entity snapshot that is currently being used as the ending interpolation tick (where we are going to).</summary>
		public EntitySnapshot InterpolationEndSnapshot { get; }
		/// <summary>Gets the entity snapshot that is currently the actively rendered state.</summary>
		public EntitySnapshot RenderedSnapshot { get; }

		#endregion Properties

		#region Methods

		public void Update(TCommandData commandData)
		{
			if (this.LatestServerTickAcknowledgedByClient >= 0)
			{
				// Only tick the client if we started getting info from the server
				this.FrameTick++;
			}

			byte[] packet;
			while ((packet = this.serverNetworkConnection.GetNextIncomingPacket()) != null)
			{
				int newLatestClientTickAcknowledgedByServer;
				int newOwnedEntity;
				using (MemoryStream memoryStream = new MemoryStream(packet))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						newLatestClientTickAcknowledgedByServer = binaryReader.ReadInt32();
						newOwnedEntity = binaryReader.ReadInt32();
						this.deserializedEntitySnapshot.Deserialize(binaryReader);
					}
				}

				// Get the oldest entity snapshot in the buffer that will be overwritten with the new incoming data
				EntitySnapshot oldestHistoryEntitySnapshot = this.getOldestHistoryEntitySnapshot();

				// If the new packet isn't actually new, ignore it
				if (this.deserializedEntitySnapshot.ServerFrameTick <= oldestHistoryEntitySnapshot.ServerFrameTick) { continue; }

				// Replace the snapshot in the history buffer with what we just got from the server
				oldestHistoryEntitySnapshot.CopyFrom(this.deserializedEntitySnapshot);

				if (this.LatestServerTickAcknowledgedByClient < 0)
				{
					// Sync our ticks with the server's ticks once we start getting some data from the server
					this.FrameTick = this.deserializedEntitySnapshot.ServerFrameTick;
				}

				if (this.LatestServerTickAcknowledgedByClient < this.deserializedEntitySnapshot.ServerFrameTick)
				{
					// This snapshot is the latest update we've gotten from the server so update our state accordingly
					this.LatestServerTickAcknowledgedByClient = this.deserializedEntitySnapshot.ServerFrameTick;
					this.LatestClientTickAcknowledgedByServer = newLatestClientTickAcknowledgedByServer;
					this.OwnedEntity = newOwnedEntity;
				}
			}

			if (this.HasInterpolationStarted)
			{
				this.clientCommandHistory.Dequeue();
				this.clientCommandHistory.Enqueue(new ClientCommand<TCommandData>()
				{
					ClientFrameTick = this.FrameTick,
					AcknowledgedServerTick = this.LatestServerTickAcknowledgedByClient,
					InterpolationStartTick = this.InterpolationStartSnapshot.ServerFrameTick,
					InterpolationEndTick = this.InterpolationEndSnapshot.ServerFrameTick,
					RenderedFrameTick = this.FrameTick - this.InterpolationRenderDelay,
					CommandingEntity = this.OwnedEntity,
					CommandData = commandData,
				});
				this.serverNetworkConnection.SendPacket(ClientCommand<TCommandData>.SerializeCommands(this.clientCommandHistory.Where((cmd) => cmd.ClientFrameTick > this.LatestClientTickAcknowledgedByServer).ToArray()));
			}

			this.setupRenderSnapshot();

			// Client side prediction
			if (this.ShouldPredictInput && this.RenderedSnapshot != null && this.OwnedEntity != -1)
			{
				/*StateSnapshot latestStateSnapshot = this.ReceivedStateSnapshots.Last().Value;
				Entity predictedEntity = new Entity() { Position = latestStateSnapshot.Entities[this.CurrentOwnedEntity].Position };
				foreach (ClientCommand<TCommandData> clientCommandNotAckedByServer in this.SentClientCommands.Where((cmd) => cmd.ClientFrameTick > latestStateSnapshot.AcknowledgedClientTick))
				{
					// Don't use this command for prediction since its an old command that applied to some other owned entity
					if (clientCommandNotAckedByServer.CommandingEntity != this.CurrentOwnedEntity) { continue; }

					// Reapply all the commands we've sent that the server hasn't processed yet to get us back to where we predicted we should be, starting
					// from where the server last gave us an authoritative response
					clientCommandNotAckedByServer.CommandData.ApplyToEntity(predictedEntity);
				}
				this.RenderedState.Entities[this.CurrentOwnedEntity].Position = predictedEntity.Position;
				this.predictedPositions.Add(this.FrameTick, predictedEntity.Position);*/
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
				EntitySnapshot newInterpolationStartSnapshot = null;
				EntitySnapshot newInterpolationEndSnapshot = null;
				foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
				{
					if (entitySnapshot.ServerFrameTick == -1) { continue; }

					// The snapshot history isn't in any order so we need to check every snapshot for the closest ticks in both directions (start and end)
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
				EntitySnapshot newInterpolationEndSnapshot = null;
				foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
				{
					// The snapshot history isn't in any order so we need to check every snapshot closest next tick
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
		private EntitySnapshot getOldestHistoryEntitySnapshot()
		{
			// Find the oldest entity snapshot
			EntitySnapshot oldestEntitySnapshot = this.entitySnapshotHistory[0];
			foreach (EntitySnapshot entitySnapshot in this.entitySnapshotHistory)
			{
				// The snapshot history isn't in any order so we need to check every snapshot
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
