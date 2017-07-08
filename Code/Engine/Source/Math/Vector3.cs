using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public struct Vector3
	{
		#region Fields

		public float X;
		public float Y;
		public float Z;

		#endregion Fields

		#region Constructors

		public Vector3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		#endregion Constructors
	}
}
