using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Benchmarks
{
	public class CpuBenchmark
	{
		#region Methods

		[Benchmark]
		public TimeSpan GenericCpuScore()
		{
			double calc(int a, int b) => Math.Acos((double)a / b);

			List<double> results = new List<double>(50_000);

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				for (int i = 0; i < 200; i++)
				{
					for (int k = -100; k < 100; k++)
					{
						double value = calc(i, k);
						if (!results.Contains(value))
						{
							results.Add(value);
						}
					}
				}
			}
			return stopwatch.Elapsed;
		}

		#endregion Methods
	}
}
