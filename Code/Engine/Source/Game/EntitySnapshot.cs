﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Represents a snapshot in time of entity state on the server.
	/// </summary>
	public class EntitySnapshot
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntitySnapshot(int entityCapacity, ComponentsDefinition componentsDefinition)
		{
			this.ServerFrameTick = -1;
			this.EntityArray = new EntityArray(entityCapacity, componentsDefinition);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the frame tick on the server that this snapshot was taken at.
		/// </summary>
		public int ServerFrameTick { get; private set; }

		/// <summary>
		/// Gets the array of entities as they existed at the point in time at <see cref="ServerFrameTick"/>.
		/// </summary>
		public EntityArray EntityArray { get; }

		/// <summary>
		/// Gets whether or not this entity snapshot has been loaded with data.
		/// </summary>
		public bool HasData { get { return (this.ServerFrameTick >= 0); } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates this entity snapshot to be a new snapshot at a new point in time, given the new server frame tick and new entities.
		/// </summary>
		public void Update(int serverFrameTick, EntityArray other)
		{
			this.ServerFrameTick = serverFrameTick;
			other.CopyTo(this.EntityArray);
		}

		/// <summary>
		/// Copies all entity and meta-data to another entity snapshot.
		/// </summary>
		public void CopyTo(EntitySnapshot other)
		{
			other.Update(this.ServerFrameTick, this.EntityArray);
		}

		/// <summary>
		/// Updates this entity snapshot to be a new snapshot that is an interpolated state between two other snapshots.
		/// </summary>
		public void Interpolate(EntitySnapshot otherA, EntitySnapshot otherB, int interpolationFrameTick, int serverFrameTick)
		{
			float amount = ((float)interpolationFrameTick - otherA.ServerFrameTick) / ((float)otherB.ServerFrameTick - otherA.ServerFrameTick);
			this.EntityArray.Interpolate(otherA.EntityArray, otherB.EntityArray, amount);
			this.ServerFrameTick = serverFrameTick;
		}

		/// <summary>
		/// Writes this entity snapshot's data to a binary source, only writing data that has changed from a previous snapshot.
		/// </summary>
		public void Serialize(EntitySnapshot previousEntitySnapshot, IWriter writer)
		{
			writer.Write(this.ServerFrameTick);
			this.EntityArray.Serialize(previousEntitySnapshot?.EntityArray, writer);
		}

		/// <summary>
		/// Reads and overwrites this entity snapshot with data from a binary source, basing incoming data on a previous snapshot's data.
		/// </summary>
		public void Deserialize(EntitySnapshot previousEntitySnapshot, IReader reader)
		{
			this.ServerFrameTick = reader.ReadInt32();
			this.EntityArray.Deserialize(previousEntitySnapshot?.EntityArray, reader);
		}

		/// <summary>
		/// Reads and overwrites this entity snapshot with data from a binary source, but only if the binary source represents a newer snapshot.
		/// Returns true if this snapshot was actually updated from the deserialized binary source.
		/// </summary>
		public bool DeserializeIfNewer(EntitySnapshot previousEntitySnapshot, IReader reader)
		{
			int newServerFrameTick = reader.ReadInt32();
			if (newServerFrameTick <= this.ServerFrameTick) { return false; }

			this.ServerFrameTick = newServerFrameTick;
			this.EntityArray.Deserialize(previousEntitySnapshot?.EntityArray, reader);
			return true;
		}

		#endregion Methods
	}
}
