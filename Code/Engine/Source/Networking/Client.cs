using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine.Client
{
	public class Client
	{
		#region Fields

		private int frameTick;

		#endregion Fields

		#region Methods

		public void Update()
		{
			this.frameTick++;
		}

		#endregion Methods
	}
}
