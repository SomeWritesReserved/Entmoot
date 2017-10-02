using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entmoot.Engine
{
	/// <summary>
	/// Defines properties for how to log data.
	/// </summary>
	public static class Log
	{
		#region Properties

		/// <summary>
		/// Gets or sets the maximum number of history items that will be stored for every log type.
		/// </summary>
		public static int HistoryLength { get; set; } = 120;

		#endregion Properties
	}

	/// <summary>
	/// Logs data of a specific type during a specific frame and stores the results in a historical buffer.
	/// </summary>
	/// <typeparam name="T">The type of data to log.</typeparam>
	public static class Log<T>
		where T : struct
	{
		#region Fields

		/// <summary>
		/// Gets the data to update during the current logging frame.
		/// </summary>
		public static T Data;

		/// <summary>The historical buffer of logged data.</summary>
		private static Queue<T> dataHistory;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Gets the history of logged data.
		/// </summary>
		public static Queue<T> History { get { return dataHistory; } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Starts logging new data while storing the previous data in the historical buffer.
		/// </summary>
		public static void StartNew()
		{
			if (dataHistory == null)
			{
				dataHistory = new Queue<T>(Log.HistoryLength);
			}
			else
			{
				while (dataHistory.Count >= Log.HistoryLength)
				{
					dataHistory.Dequeue();
				}
				dataHistory.Enqueue(Data);
			}
			Data = default(T);
		}

		#endregion Methods
	}
}
