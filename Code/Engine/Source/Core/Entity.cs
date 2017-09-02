using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public struct Entity
	{
		#region Constructors

		internal Entity(int id)
		{
			this.ID = id;
		}

		#endregion Constructors

		#region Properties

		public int ID { get; }

		#endregion Properties
	}
}
