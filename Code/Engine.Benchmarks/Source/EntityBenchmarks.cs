using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;

namespace Engine.Benchmarks
{
	public class EntityBenchmarks
	{
		#region Methods

		[Benchmark]
		public TimeSpan Entity_HasComponent_FirstComponentType()
		{
			const int entityCapacity = 1000;
			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<TestComponent1>();
			componentsDefinition.RegisterComponentType<TestComponent2>();
			componentsDefinition.RegisterComponentType<TestComponent3>();
			componentsDefinition.RegisterComponentType<TestComponent4>();
			componentsDefinition.RegisterComponentType<TestComponent5>();
			componentsDefinition.RegisterComponentType<TestComponent6>();
			componentsDefinition.RegisterComponentType<TestComponent7>();
			componentsDefinition.RegisterComponentType<TestComponent8>();
			componentsDefinition.RegisterComponentType<TestComponent9>();
			componentsDefinition.RegisterComponentType<TestComponent10>();
			componentsDefinition.RegisterComponentType<TestComponent11>();
			componentsDefinition.RegisterComponentType<TestComponent12>();

			EntityArray entityArray = new EntityArray(entityCapacity, componentsDefinition);
			entityArray.TryCreateEntity(out Entity entity);
			entity.AddComponent<TestComponent1>();
			entity.AddComponent<TestComponent2>();
			entity.AddComponent<TestComponent3>();
			entity.AddComponent<TestComponent4>();
			entity.AddComponent<TestComponent5>();
			entity.AddComponent<TestComponent6>();
			entity.AddComponent<TestComponent7>();
			entity.AddComponent<TestComponent8>();
			entity.AddComponent<TestComponent9>();
			entity.AddComponent<TestComponent10>();
			entity.AddComponent<TestComponent11>();
			entity.AddComponent<TestComponent12>();

			const int iterationCount = 1000;
			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				for (int i = 0; i < iterationCount; i++)
				{
					entity.HasComponent<TestComponent1>();
				}
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Iteration count", iterationCount);
			return stopwatch.Elapsed;
		}

		[Benchmark]
		public TimeSpan Entity_HasComponent_LastComponentType()
		{
			const int entityCapacity = 1000;
			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<TestComponent1>();
			componentsDefinition.RegisterComponentType<TestComponent2>();
			componentsDefinition.RegisterComponentType<TestComponent3>();
			componentsDefinition.RegisterComponentType<TestComponent4>();
			componentsDefinition.RegisterComponentType<TestComponent5>();
			componentsDefinition.RegisterComponentType<TestComponent6>();
			componentsDefinition.RegisterComponentType<TestComponent7>();
			componentsDefinition.RegisterComponentType<TestComponent8>();
			componentsDefinition.RegisterComponentType<TestComponent9>();
			componentsDefinition.RegisterComponentType<TestComponent10>();
			componentsDefinition.RegisterComponentType<TestComponent11>();
			componentsDefinition.RegisterComponentType<TestComponent12>();

			EntityArray entityArray = new EntityArray(entityCapacity, componentsDefinition);
			entityArray.TryCreateEntity(out Entity entity);
			entity.AddComponent<TestComponent1>();
			entity.AddComponent<TestComponent2>();
			entity.AddComponent<TestComponent3>();
			entity.AddComponent<TestComponent4>();
			entity.AddComponent<TestComponent5>();
			entity.AddComponent<TestComponent6>();
			entity.AddComponent<TestComponent7>();
			entity.AddComponent<TestComponent8>();
			entity.AddComponent<TestComponent9>();
			entity.AddComponent<TestComponent10>();
			entity.AddComponent<TestComponent11>();
			entity.AddComponent<TestComponent12>();

			const int iterationCount = 1000;
			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				for (int i = 0; i < iterationCount; i++)
				{
					entity.HasComponent<TestComponent12>();
				}
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Iteration count", iterationCount);
			return stopwatch.Elapsed;
		}

		#endregion Methods

		#region Nested Types

		private struct TestComponent1 : IComponent<TestComponent1>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent1 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent1 otherA, TestComponent1 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent2 : IComponent<TestComponent2>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent2 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent2 otherA, TestComponent2 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent3 : IComponent<TestComponent3>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent3 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent3 otherA, TestComponent3 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent4 : IComponent<TestComponent4>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent4 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent4 otherA, TestComponent4 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent5 : IComponent<TestComponent5>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent5 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent5 otherA, TestComponent5 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent6 : IComponent<TestComponent6>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent6 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent6 otherA, TestComponent6 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent7 : IComponent<TestComponent7>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent7 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent7 otherA, TestComponent7 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent8 : IComponent<TestComponent8>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent8 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent8 otherA, TestComponent8 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent9 : IComponent<TestComponent9>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent9 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent9 otherA, TestComponent9 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent10 : IComponent<TestComponent10>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent10 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent10 otherA, TestComponent10 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent11 : IComponent<TestComponent11>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent11 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent11 otherA, TestComponent11 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		private struct TestComponent12 : IComponent<TestComponent12>
		{
			#region Fields

			public float VelocityX;
			public float VelocityY;
			public float VelocityZ;
			public float AccelerationX;
			public float AccelerationY;
			public float AccelerationZ;

			#endregion Fields

			#region Methods

			public bool Equals(TestComponent12 other)
			{
				return this.VelocityX == other.VelocityX &&
					this.VelocityY == other.VelocityY &&
					this.VelocityZ == other.VelocityZ &&
					this.AccelerationX == other.AccelerationX &&
					this.AccelerationY == other.AccelerationY &&
					this.AccelerationZ == other.AccelerationZ;
			}

			public void Interpolate(TestComponent12 otherA, TestComponent12 otherB, float amount)
			{
				this.VelocityX = otherA.VelocityX + (this.VelocityX - otherA.VelocityX) * amount;
				this.VelocityY = otherA.VelocityY + (this.VelocityY - otherA.VelocityY) * amount;
				this.VelocityZ = otherA.VelocityZ + (this.VelocityZ - otherA.VelocityZ) * amount;
				this.AccelerationX = otherA.AccelerationX + (this.AccelerationX - otherA.AccelerationX) * amount;
				this.AccelerationY = otherA.AccelerationY + (this.AccelerationY - otherA.AccelerationY) * amount;
				this.AccelerationZ = otherA.AccelerationZ + (this.AccelerationZ - otherA.AccelerationZ) * amount;
			}

			public void Serialize(IWriter writer)
			{
				writer.Write(this.VelocityX);
				writer.Write(this.VelocityY);
				writer.Write(this.VelocityZ);
				writer.Write(this.AccelerationX);
				writer.Write(this.AccelerationY);
				writer.Write(this.AccelerationZ);
			}

			public void Deserialize(IReader reader)
			{
				this.VelocityX = reader.ReadSingle();
				this.VelocityY = reader.ReadSingle();
				this.VelocityZ = reader.ReadSingle();
				this.AccelerationX = reader.ReadSingle();
				this.AccelerationY = reader.ReadSingle();
				this.AccelerationZ = reader.ReadSingle();
			}

			public void ResetToDefaults()
			{
				this.VelocityX = 0;
				this.VelocityY = 0;
				this.VelocityZ = 0;
				this.AccelerationX = 0;
				this.AccelerationY = 0;
				this.AccelerationZ = 0;
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}
