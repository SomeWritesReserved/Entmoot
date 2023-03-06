using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Engine.Benchmarks
{
	/// <summary>
	/// The main class for the Engine benchmarks program.
	/// </summary>
	public class Program
	{
		#region Methods

		/// <summary>
		/// The main entry point for the Engine benchmarks program. This methods will call the benchmarks in other classes/files.
		/// </summary>
		public static void Main(string[] args)
		{
			try
			{
				IReadOnlyList<MethodInfo> benchmarkMethods = Assembly.GetExecutingAssembly().GetTypes().SelectMany((type) => type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
					.Where((method) => method.GetCustomAttribute<BenchmarkAttribute>() != null)
					.Where((method) => method.ReturnType == typeof(TimeSpan))
					.ToList();

				foreach (MethodInfo benchmarkMethod in benchmarkMethods)
				{
					Console.WriteLine($"{benchmarkMethod.DeclaringType.Name}.{benchmarkMethod.Name}");
					try
					{
						List<TimeSpan> benchmarkTimeSpans = new List<TimeSpan>();
						List<Dictionary<string, double>> benchmarkMetaDatas = new List<Dictionary<string, double>>();
						for (int i = 0; i < 50; i++)
						{
							object benchmarkinClassInstance = Activator.CreateInstance(benchmarkMethod.DeclaringType);
							TimeSpan benchmarkTimeSpan = (TimeSpan)benchmarkMethod.Invoke(benchmarkinClassInstance, null);
							benchmarkTimeSpans.Add(benchmarkTimeSpan);
							benchmarkMetaDatas.Add(BenchmarkMetadata.Reset());
						}

						// Skip the first benchmark run to avoid any start up costs affecting performance
						benchmarkTimeSpans = benchmarkTimeSpans.Skip(1).ToList();
						benchmarkMetaDatas = benchmarkMetaDatas.Skip(1).ToList();

						double timeAverageMs = benchmarkTimeSpans.Select((time) => time.TotalMilliseconds).Average();
						double timeMinMs = benchmarkTimeSpans.Select((time) => time.TotalMilliseconds).Min();
						double timeMaxMs = benchmarkTimeSpans.Select((time) => time.TotalMilliseconds).Max();
						Console.WriteLine($"  Time (ms): {timeAverageMs:0.000} average ({timeMinMs:0.000} min / {timeMaxMs:0.000} max)");

						if (benchmarkMetaDatas.Any((dict) => dict.Any()))
						{
							string[] metadataKeys = benchmarkMetaDatas.SelectMany((dict) => dict.Keys).Distinct().ToArray();
							foreach (string metadataKey in metadataKeys)
							{
								double metadataAverage = benchmarkMetaDatas.Select((dict) => dict[metadataKey]).Average();
								double metadataMin = benchmarkMetaDatas.Select((dict) => dict[metadataKey]).Min();
								double metadataMax = benchmarkMetaDatas.Select((dict) => dict[metadataKey]).Max();
								if (metadataAverage != metadataMin || metadataAverage != metadataMax)
								{
									Console.WriteLine($"  {metadataKey}: {metadataAverage:0} average ({metadataMin:0} min / {metadataMax:0} max)");
								}
								else
								{
									Console.WriteLine($"  {metadataKey}: {metadataAverage:0}");
								}
							}
						}
					}
					catch (Exception exception)
					{
						Console.WriteLine($"  ...failed with exception {exception.GetType().Name}.");
					}
					Console.WriteLine();
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("EXCEPTION:");
				Console.WriteLine(exception);
			}

			Console.WriteLine();
			Console.WriteLine("Done");
			Console.ReadKey(true);
		}

		#endregion Methods
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class BenchmarkAttribute : Attribute
	{
	}

	public static class BenchmarkMetadata
	{
		#region Fields

		private static Dictionary<string, double> metadata = new Dictionary<string, double>();

		#endregion Fields

		#region Methods

		public static void Add(string name, double data)
		{
			BenchmarkMetadata.metadata[name] = data;
		}

		public static Dictionary<string, double> Reset()
		{
			try
			{
				return BenchmarkMetadata.metadata;
			}
			finally
			{
				BenchmarkMetadata.metadata = new Dictionary<string, double>();
			}
		}

		#endregion Methods
	}
}