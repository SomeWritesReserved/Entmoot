using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	public interface IEntityCollection
	{
		#region Properties

		ReadOnlyCollection<Entity> Entities { get; }

		#endregion Properties
	}

	public interface IEntitySystem
	{
		#region Methods

		void Update(IEntityCollection entityCollection);

		#endregion Methods
	}
}
