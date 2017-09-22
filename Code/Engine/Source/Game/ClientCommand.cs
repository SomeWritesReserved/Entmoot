using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// An interface that defines the actual data that a client command will send to the server.
	/// </summary>
	public interface ICommandData
	{
		#region Methods

		/// <summary>
		/// Writes the state of this command data to a binary source.
		/// </summary>
		void Serialize(IWriter writer);

		/// <summary>
		/// Reads and overwrites the current state of this command data from a binary source.
		/// </summary>
		void Deserialize(BinaryReader binaryReader);

		/// <summary>
		/// Applies this command data to a given entity (whatever that may mean for the type of command).
		/// </summary>
		void ApplyToEntity(Entity entity);

		#endregion Methods
	}

	/// <summary>
	/// Represents a command from a client sent to and processed by the server (and used in client-side prediction).
	/// </summary>
	/// <typeparam name="TCommandData">The type of data that will be sent to the server as a command.</typeparam>
	public class ClientCommand<TCommandData>
		where TCommandData : struct, ICommandData
	{
		#region Properties

		/// <summary>
		/// Gets the maximum number of clinet commands that will be sent to the server, per single update.
		/// </summary>
		public static int MaxClientCommandsPerUpdate { get { return 30; } }

		/// <summary>
		/// Gets the frame tick on the client that this command was taken at (which may be ahead of <see cref="RenderedTick"/>).
		/// </summary>
		public int ClientFrameTick { get; private set; } = -1;

		/// <summary>
		/// Gets the frame tick for what the client is rendering when this command was taken (which may be behind <see cref="ClientFrameTick"/>).
		/// </summary>
		public int RenderedTick { get; private set; } = -1;

		/// <summary>
		/// Gets the starting frame tick that the client is using to interpolate to the rendered frame (if interpolation was active);
		/// </summary>
		public int InterpolationStartTick { get; private set; } = -1;

		/// <summary>
		/// Gets the ending frame tick that the client is using to interpolate to the rendered frame (if interpolation was active);
		/// </summary>
		public int InterpolationEndTick { get; private set; } = -1;

		/// <summary>
		/// Gets the entity that the client was commanding when this command was taken (which the server may ignore if the client is out-of-date
		/// with what entity it should be commanding).
		/// </summary>
		public int CommandingEntityID { get; private set; } = -1;

		private TCommandData commandData;
		/// <summary>
		/// Gets the packaged data that the client command will send to the server.
		/// </summary>
		/// <remarks>
		/// This has a manually declared backing field (since its a struct) which you should always use, if not then the property getter creates a copy
		/// of the backing field which breaks deserialization (data gets deserialized into the copy, not into the backing field).
		/// </remarks>
		public TCommandData CommandData { get { return this.commandData; } }

		/// <summary>
		/// Gets whether or not this client command has been loaded with data.
		/// </summary>
		public bool HasData { get { return (this.ClientFrameTick >= 0); } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates this entity snapshot to be a new snapshot at a new point in time, given the new server frame tick and new entities.
		/// </summary>
		public void Update(int clientFrameTick, int renderedTick, int interpolationStartTick, int interpolationEndTick, bool isInterpolating, int commandingEntityID, TCommandData commandData)
		{
			this.ClientFrameTick = clientFrameTick;
			this.RenderedTick = renderedTick;
			this.InterpolationStartTick = isInterpolating ? interpolationStartTick : -1;
			this.InterpolationEndTick = isInterpolating ? interpolationEndTick : -1;
			this.CommandingEntityID = commandingEntityID;
			this.commandData = commandData;
		}

		/// <summary>
		/// Writes the state of this command to a binary source.
		/// </summary>
		public void Serialize(IWriter writer)
		{
			writer.Write(this.ClientFrameTick);
			writer.Write(this.RenderedTick);
			writer.Write(this.InterpolationStartTick);
			writer.Write(this.InterpolationEndTick);
			writer.Write(this.CommandingEntityID);
			this.commandData.Serialize(writer);
		}

		/// <summary>
		/// Reads and overwrites the current state of this command from a binary source.
		/// </summary>
		public void Deserialize(BinaryReader binaryReader)
		{
			this.ClientFrameTick = binaryReader.ReadInt32();
			this.RenderedTick = binaryReader.ReadInt32();
			this.InterpolationStartTick = binaryReader.ReadInt32();
			this.InterpolationEndTick = binaryReader.ReadInt32();
			this.CommandingEntityID = binaryReader.ReadInt32();
			this.commandData.Deserialize(binaryReader);
		}

		#endregion Methods
	}
}
