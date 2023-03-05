using System;
using System.Collections.Generic;
using System.Diagnostics;
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

	/// <summary>
	/// A timer that could be used during logging to time blocks of code.
	/// </summary>
	public struct LogTimer
	{
		#region Fields

		/// <summary>
		/// The start timestamp of this timer (in arbitrary stopwatch units).
		/// </summary>
		private long startTimestamp;

		/// <summary>
		/// The total time, in milliseconds, this timer took.
		/// </summary>
		public double DurationMs;

		#endregion Fields

		#region Methods

		/// <summary>
		/// Starts (or restarts) this timer and clears any existing duration.
		/// </summary>
		public void Start()
		{
			this.startTimestamp = Stopwatch.GetTimestamp();
			this.DurationMs = 0;
		}

		/// <summary>
		/// Stops this timer and sets the duration.
		/// </summary>
		public void Stop()
		{
			this.DurationMs = Stopwatch.GetElapsedTime(this.startTimestamp).TotalMilliseconds;
		}

		#endregion Methods
	}
}
