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

		void OnEntityCreated(Entity entity);
		void OnEntityRemoved(Entity entity);

		void Update();

		#endregion Methods
	}
}
