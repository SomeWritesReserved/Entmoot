using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Entmoot.FpsGame
{
	public static class VectorHelper
	{
		#region Methods

		public static Vector2 ToXna(this System.Numerics.Vector2 vector)
		{
			return new Vector2(vector.X, vector.Y);
		}

		public static Vector3 ToXna(this System.Numerics.Vector3 vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}

		#endregion Methods
	}
}
