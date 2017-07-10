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

		#region Methods

		public static Vector3 Interpolate(Vector3 vectorA, Vector3 vectorB, float amount)
		{
			return new Vector3(vectorA.X + (vectorB.X - vectorA.X) * amount,
				vectorA.Y + (vectorB.Y - vectorA.Y) * amount,
				vectorA.Z + (vectorB.Z - vectorA.Z) * amount);
		}

		#endregion Methods
	}
}
