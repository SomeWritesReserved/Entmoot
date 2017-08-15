using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public interface IEntitySystem
	{
		#region Methods

		void Update(IEntityCollection entityCollection);

		#endregion Methods
	}
}
