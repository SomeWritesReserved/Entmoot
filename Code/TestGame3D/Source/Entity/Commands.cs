using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.TestGame3D
{
	[Flags]
	public enum Commands : short
	{
		None,
		MoveForward = 1,
		MoveBackward = 2,
		MoveLeft = 4,
		MoveRight = 8,
		Jump = 16,
		Attack = 32,
	}

	public struct CommandData : ICommandData
	{
		#region Fields

		public const float MoveImpulse = 75.0f;

		public Commands Commands;
		public Vector2 LookAngles;

		#endregion Fields

		#region Methods

		public void Serialize(IWriter writer)
		{
			writer.Write((short)this.Commands);
			writer.Write(this.LookAngles.X);
			writer.Write(this.LookAngles.Y);
		}

		public void Deserialize(IReader reader)
		{
			this.Commands = (Commands)reader.ReadInt16();
			this.LookAngles.X = reader.ReadSingle();
			this.LookAngles.Y = reader.ReadSingle();
		}

		#endregion Methods
	}
}
