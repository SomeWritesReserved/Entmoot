using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Game.Sideswipe
{
	public struct LogGameRendering
	{
		#region Fields

		/// <summary>The number of frames that were slow to update.</summary>
		public int NumberOfSlowUpdateFrames;

		/// <summary>The number of frames that were slow to render.</summary>
		public int NumberOfSlowDrawFrames;

		#endregion Fields
	}
}
