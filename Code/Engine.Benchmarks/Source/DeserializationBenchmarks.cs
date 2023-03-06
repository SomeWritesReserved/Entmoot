using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;

namespace Engine.Benchmarks
{
	public class DeserializationBenchmarks
	{
		#region Methods

		[Benchmark]
		public TimeSpan Deserialize_NoEntities()
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

			EntityArray previousEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray previousEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);

			OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[100_000]);
			currentEntityArray_serialize.Serialize(previousEntityArray_serialize, outgoingMessage);
			IncomingMessage incomingMessage = new IncomingMessage(outgoingMessage.MessageData);
			incomingMessage.Length = outgoingMessage.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				currentEntityArray_deserialize.Deserialize(previousEntityArray_deserialize, incomingMessage);
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Entity count (before)", currentEntityArray_serialize.Count());
			BenchmarkMetadata.Add("Entity count (after) ", currentEntityArray_deserialize.Count());
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Serialized size (bytes)", outgoingMessage.Length);
			return stopwatch.Elapsed;
		}

		[Benchmark]
		public TimeSpan Deserialize_AllEntities_NoComponents_NoChanges()
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
			
			EntityArray previousEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray previousEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);

			previousEntityArray_serialize.BeginUpdate();
			currentEntityArray_serialize.BeginUpdate();
			previousEntityArray_deserialize.BeginUpdate();
			currentEntityArray_deserialize.BeginUpdate();
			for (int entityID = 0; entityID < entityCapacity; entityID++)
			{
				if (!previousEntityArray_serialize.TryCreateEntity(out Entity previousEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				if (!currentEntityArray_serialize.TryCreateEntity(out Entity currentEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				if (!previousEntityArray_deserialize.TryCreateEntity(out Entity previousEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
				if (!currentEntityArray_deserialize.TryCreateEntity(out Entity currentEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
			}
			previousEntityArray_serialize.EndUpdate();
			currentEntityArray_serialize.EndUpdate();
			previousEntityArray_deserialize.EndUpdate();
			currentEntityArray_deserialize.EndUpdate();

			OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[100_000]);
			currentEntityArray_serialize.Serialize(previousEntityArray_serialize, outgoingMessage);
			IncomingMessage incomingMessage = new IncomingMessage(outgoingMessage.MessageData);
			incomingMessage.Length = outgoingMessage.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				currentEntityArray_deserialize.Deserialize(previousEntityArray_deserialize, incomingMessage);
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Entity count (before)", currentEntityArray_serialize.Count());
			BenchmarkMetadata.Add("Entity count (after) ", currentEntityArray_deserialize.Count());
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Serialized size (bytes)", outgoingMessage.Length);
			return stopwatch.Elapsed;
		}

		[Benchmark]
		public TimeSpan Deserialize_AllEntities_NoComponents_AllNewEntities()
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

			EntityArray previousEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray previousEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			
			previousEntityArray_serialize.BeginUpdate();
			currentEntityArray_serialize.BeginUpdate();
			previousEntityArray_deserialize.BeginUpdate();
			currentEntityArray_deserialize.BeginUpdate();
			for (int entityID = 0; entityID < entityCapacity; entityID++)
			{
				if (!currentEntityArray_serialize.TryCreateEntity(out Entity currentEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
			}
			previousEntityArray_serialize.EndUpdate();
			currentEntityArray_serialize.EndUpdate();
			previousEntityArray_deserialize.EndUpdate();
			currentEntityArray_deserialize.EndUpdate();

			OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[100_000]);
			currentEntityArray_serialize.Serialize(previousEntityArray_serialize, outgoingMessage);
			IncomingMessage incomingMessage = new IncomingMessage(outgoingMessage.MessageData);
			incomingMessage.Length = outgoingMessage.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				currentEntityArray_deserialize.Deserialize(previousEntityArray_deserialize, incomingMessage);
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Entity count (before)", currentEntityArray_serialize.Count());
			BenchmarkMetadata.Add("Entity count (after) ", currentEntityArray_deserialize.Count());
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Serialized size (bytes)", outgoingMessage.Length);
			return stopwatch.Elapsed;
		}

		[Benchmark]
		public TimeSpan Deserialize_AllEntities_AllComponents_NoChanges()
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

			EntityArray previousEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray previousEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);

			previousEntityArray_serialize.BeginUpdate();
			currentEntityArray_serialize.BeginUpdate();
			previousEntityArray_deserialize.BeginUpdate();
			currentEntityArray_deserialize.BeginUpdate();
			for (int entityID = 0; entityID < entityCapacity; entityID++)
			{
				if (!previousEntityArray_serialize.TryCreateEntity(out Entity previousEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				previousEntity_serialize.AddComponent<TestComponent1>();
				previousEntity_serialize.AddComponent<TestComponent2>();
				previousEntity_serialize.AddComponent<TestComponent3>();
				previousEntity_serialize.AddComponent<TestComponent4>();
				previousEntity_serialize.AddComponent<TestComponent5>();
				previousEntity_serialize.AddComponent<TestComponent6>();
				previousEntity_serialize.AddComponent<TestComponent7>();
				previousEntity_serialize.AddComponent<TestComponent8>();
				previousEntity_serialize.AddComponent<TestComponent9>();
				previousEntity_serialize.AddComponent<TestComponent10>();
				previousEntity_serialize.AddComponent<TestComponent11>();
				previousEntity_serialize.AddComponent<TestComponent12>();
				if (!currentEntityArray_serialize.TryCreateEntity(out Entity currentEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				currentEntity_serialize.AddComponent<TestComponent1>();
				currentEntity_serialize.AddComponent<TestComponent2>();
				currentEntity_serialize.AddComponent<TestComponent3>();
				currentEntity_serialize.AddComponent<TestComponent4>();
				currentEntity_serialize.AddComponent<TestComponent5>();
				currentEntity_serialize.AddComponent<TestComponent6>();
				currentEntity_serialize.AddComponent<TestComponent7>();
				currentEntity_serialize.AddComponent<TestComponent8>();
				currentEntity_serialize.AddComponent<TestComponent9>();
				currentEntity_serialize.AddComponent<TestComponent10>();
				currentEntity_serialize.AddComponent<TestComponent11>();
				currentEntity_serialize.AddComponent<TestComponent12>();
				if (!previousEntityArray_deserialize.TryCreateEntity(out Entity previousEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
				previousEntity_deserialize.AddComponent<TestComponent1>();
				previousEntity_deserialize.AddComponent<TestComponent2>();
				previousEntity_deserialize.AddComponent<TestComponent3>();
				previousEntity_deserialize.AddComponent<TestComponent4>();
				previousEntity_deserialize.AddComponent<TestComponent5>();
				previousEntity_deserialize.AddComponent<TestComponent6>();
				previousEntity_deserialize.AddComponent<TestComponent7>();
				previousEntity_deserialize.AddComponent<TestComponent8>();
				previousEntity_deserialize.AddComponent<TestComponent9>();
				previousEntity_deserialize.AddComponent<TestComponent10>();
				previousEntity_deserialize.AddComponent<TestComponent11>();
				previousEntity_deserialize.AddComponent<TestComponent12>();
				if (!currentEntityArray_deserialize.TryCreateEntity(out Entity currentEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
				currentEntity_deserialize.AddComponent<TestComponent1>();
				currentEntity_deserialize.AddComponent<TestComponent2>();
				currentEntity_deserialize.AddComponent<TestComponent3>();
				currentEntity_deserialize.AddComponent<TestComponent4>();
				currentEntity_deserialize.AddComponent<TestComponent5>();
				currentEntity_deserialize.AddComponent<TestComponent6>();
				currentEntity_deserialize.AddComponent<TestComponent7>();
				currentEntity_deserialize.AddComponent<TestComponent8>();
				currentEntity_deserialize.AddComponent<TestComponent9>();
				currentEntity_deserialize.AddComponent<TestComponent10>();
				currentEntity_deserialize.AddComponent<TestComponent11>();
				currentEntity_deserialize.AddComponent<TestComponent12>();
			}
			previousEntityArray_serialize.EndUpdate();
			currentEntityArray_serialize.EndUpdate();
			previousEntityArray_deserialize.EndUpdate();
			currentEntityArray_deserialize.EndUpdate();

			OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[100_000]);
			currentEntityArray_serialize.Serialize(previousEntityArray_serialize, outgoingMessage);
			IncomingMessage incomingMessage = new IncomingMessage(outgoingMessage.MessageData);
			incomingMessage.Length = outgoingMessage.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				currentEntityArray_deserialize.Deserialize(previousEntityArray_deserialize, incomingMessage);
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Entity count (before)", currentEntityArray_serialize.Count());
			BenchmarkMetadata.Add("Entity count (after) ", currentEntityArray_deserialize.Count());
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Serialized size (bytes)", outgoingMessage.Length);
			return stopwatch.Elapsed;
		}

		[Benchmark]
		public TimeSpan Deserialize_AllEntities_AllComponents_AllNewEntities()
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

			EntityArray previousEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray previousEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);

			previousEntityArray_serialize.BeginUpdate();
			currentEntityArray_serialize.BeginUpdate();
			previousEntityArray_deserialize.BeginUpdate();
			currentEntityArray_deserialize.BeginUpdate();
			for (int entityID = 0; entityID < entityCapacity; entityID++)
			{
				if (!currentEntityArray_serialize.TryCreateEntity(out Entity currentEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				currentEntity_serialize.AddComponent<TestComponent1>();
				currentEntity_serialize.AddComponent<TestComponent2>();
				currentEntity_serialize.AddComponent<TestComponent3>();
				currentEntity_serialize.AddComponent<TestComponent4>();
				currentEntity_serialize.AddComponent<TestComponent5>();
				currentEntity_serialize.AddComponent<TestComponent6>();
				currentEntity_serialize.AddComponent<TestComponent7>();
				currentEntity_serialize.AddComponent<TestComponent8>();
				currentEntity_serialize.AddComponent<TestComponent9>();
				currentEntity_serialize.AddComponent<TestComponent10>();
				currentEntity_serialize.AddComponent<TestComponent11>();
				currentEntity_serialize.AddComponent<TestComponent12>();
			}
			previousEntityArray_serialize.EndUpdate();
			currentEntityArray_serialize.EndUpdate();
			previousEntityArray_deserialize.EndUpdate();
			currentEntityArray_deserialize.EndUpdate();

			OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[100_000]);
			currentEntityArray_serialize.Serialize(previousEntityArray_serialize, outgoingMessage);
			IncomingMessage incomingMessage = new IncomingMessage(outgoingMessage.MessageData);
			incomingMessage.Length = outgoingMessage.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				currentEntityArray_deserialize.Deserialize(previousEntityArray_deserialize, incomingMessage);
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Entity count (before)", currentEntityArray_serialize.Count());
			BenchmarkMetadata.Add("Entity count (after) ", currentEntityArray_deserialize.Count());
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Serialized size (bytes)", outgoingMessage.Length);
			return stopwatch.Elapsed;
		}

		[Benchmark]
		public TimeSpan Deserialize_AllEntities_AllComponents_25Changed()
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

			EntityArray previousEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray previousEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);

			previousEntityArray_serialize.BeginUpdate();
			currentEntityArray_serialize.BeginUpdate();
			previousEntityArray_deserialize.BeginUpdate();
			currentEntityArray_deserialize.BeginUpdate();
			for (int entityID = 0; entityID < entityCapacity; entityID++)
			{
				if (!previousEntityArray_serialize.TryCreateEntity(out Entity previousEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				previousEntity_serialize.AddComponent<TestComponent1>();
				previousEntity_serialize.AddComponent<TestComponent2>();
				previousEntity_serialize.AddComponent<TestComponent3>();
				previousEntity_serialize.AddComponent<TestComponent4>();
				previousEntity_serialize.AddComponent<TestComponent5>();
				previousEntity_serialize.AddComponent<TestComponent6>();
				previousEntity_serialize.AddComponent<TestComponent7>();
				previousEntity_serialize.AddComponent<TestComponent8>();
				previousEntity_serialize.AddComponent<TestComponent9>();
				previousEntity_serialize.AddComponent<TestComponent10>();
				previousEntity_serialize.AddComponent<TestComponent11>();
				previousEntity_serialize.AddComponent<TestComponent12>();
				if (!currentEntityArray_serialize.TryCreateEntity(out Entity currentEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				if (entityID < 25)
				{
					currentEntity_serialize.AddComponent<TestComponent1>().AccelerationX = 10;
					currentEntity_serialize.AddComponent<TestComponent2>().AccelerationX = 10;
					currentEntity_serialize.AddComponent<TestComponent3>();
					currentEntity_serialize.AddComponent<TestComponent4>();
					currentEntity_serialize.AddComponent<TestComponent5>();
					currentEntity_serialize.AddComponent<TestComponent6>();
					currentEntity_serialize.AddComponent<TestComponent7>();
					currentEntity_serialize.AddComponent<TestComponent8>();
					currentEntity_serialize.AddComponent<TestComponent9>();
					currentEntity_serialize.AddComponent<TestComponent10>();
					currentEntity_serialize.AddComponent<TestComponent11>();
					currentEntity_serialize.AddComponent<TestComponent12>();
				}
				else
				{
					currentEntity_serialize.AddComponent<TestComponent1>();
					currentEntity_serialize.AddComponent<TestComponent2>();
					currentEntity_serialize.AddComponent<TestComponent3>();
					currentEntity_serialize.AddComponent<TestComponent4>();
					currentEntity_serialize.AddComponent<TestComponent5>();
					currentEntity_serialize.AddComponent<TestComponent6>();
					currentEntity_serialize.AddComponent<TestComponent7>();
					currentEntity_serialize.AddComponent<TestComponent8>();
					currentEntity_serialize.AddComponent<TestComponent9>();
					currentEntity_serialize.AddComponent<TestComponent10>();
					currentEntity_serialize.AddComponent<TestComponent11>();
					currentEntity_serialize.AddComponent<TestComponent12>();
				}
				if (!previousEntityArray_deserialize.TryCreateEntity(out Entity previousEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
				previousEntity_deserialize.AddComponent<TestComponent1>();
				previousEntity_deserialize.AddComponent<TestComponent2>();
				previousEntity_deserialize.AddComponent<TestComponent3>();
				previousEntity_deserialize.AddComponent<TestComponent4>();
				previousEntity_deserialize.AddComponent<TestComponent5>();
				previousEntity_deserialize.AddComponent<TestComponent6>();
				previousEntity_deserialize.AddComponent<TestComponent7>();
				previousEntity_deserialize.AddComponent<TestComponent8>();
				previousEntity_deserialize.AddComponent<TestComponent9>();
				previousEntity_deserialize.AddComponent<TestComponent10>();
				previousEntity_deserialize.AddComponent<TestComponent11>();
				previousEntity_deserialize.AddComponent<TestComponent12>();
				if (!currentEntityArray_deserialize.TryCreateEntity(out Entity currentEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
				currentEntity_deserialize.AddComponent<TestComponent1>();
				currentEntity_deserialize.AddComponent<TestComponent2>();
				currentEntity_deserialize.AddComponent<TestComponent3>();
				currentEntity_deserialize.AddComponent<TestComponent4>();
				currentEntity_deserialize.AddComponent<TestComponent5>();
				currentEntity_deserialize.AddComponent<TestComponent6>();
				currentEntity_deserialize.AddComponent<TestComponent7>();
				currentEntity_deserialize.AddComponent<TestComponent8>();
				currentEntity_deserialize.AddComponent<TestComponent9>();
				currentEntity_deserialize.AddComponent<TestComponent10>();
				currentEntity_deserialize.AddComponent<TestComponent11>();
				currentEntity_deserialize.AddComponent<TestComponent12>();
			}
			previousEntityArray_serialize.EndUpdate();
			currentEntityArray_serialize.EndUpdate();
			previousEntityArray_deserialize.EndUpdate();
			currentEntityArray_deserialize.EndUpdate();

			OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[100_000]);
			currentEntityArray_serialize.Serialize(previousEntityArray_serialize, outgoingMessage);
			IncomingMessage incomingMessage = new IncomingMessage(outgoingMessage.MessageData);
			incomingMessage.Length = outgoingMessage.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				currentEntityArray_deserialize.Deserialize(previousEntityArray_deserialize, incomingMessage);
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Entity count (before)", currentEntityArray_serialize.Count());
			BenchmarkMetadata.Add("Entity count (after) ", currentEntityArray_deserialize.Count());
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Serialized size (bytes)", outgoingMessage.Length);
			return stopwatch.Elapsed;
		}

		[Benchmark]
		public TimeSpan Deserialize_AllEntities_AllComponents_AllChanged()
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

			EntityArray previousEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_serialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray previousEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);
			EntityArray currentEntityArray_deserialize = new EntityArray(entityCapacity, componentsDefinition);

			previousEntityArray_serialize.BeginUpdate();
			currentEntityArray_serialize.BeginUpdate();
			previousEntityArray_deserialize.BeginUpdate();
			currentEntityArray_deserialize.BeginUpdate();
			for (int entityID = 0; entityID < entityCapacity; entityID++)
			{
				if (!previousEntityArray_serialize.TryCreateEntity(out Entity previousEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				previousEntity_serialize.AddComponent<TestComponent1>();
				previousEntity_serialize.AddComponent<TestComponent2>();
				previousEntity_serialize.AddComponent<TestComponent3>();
				previousEntity_serialize.AddComponent<TestComponent4>();
				previousEntity_serialize.AddComponent<TestComponent5>();
				previousEntity_serialize.AddComponent<TestComponent6>();
				previousEntity_serialize.AddComponent<TestComponent7>();
				previousEntity_serialize.AddComponent<TestComponent8>();
				previousEntity_serialize.AddComponent<TestComponent9>();
				previousEntity_serialize.AddComponent<TestComponent10>();
				previousEntity_serialize.AddComponent<TestComponent11>();
				previousEntity_serialize.AddComponent<TestComponent12>();
				if (!currentEntityArray_serialize.TryCreateEntity(out Entity currentEntity_serialize)) { throw new Exception($"Could not create entity {entityID}."); }
				currentEntity_serialize.AddComponent<TestComponent1>().AccelerationX = 10;
				currentEntity_serialize.AddComponent<TestComponent2>().AccelerationY = 10;
				currentEntity_serialize.AddComponent<TestComponent3>().AccelerationZ = 10;
				currentEntity_serialize.AddComponent<TestComponent4>().VelocityX = 10;
				currentEntity_serialize.AddComponent<TestComponent5>().VelocityY = 10;
				currentEntity_serialize.AddComponent<TestComponent6>().VelocityZ = 10;
				currentEntity_serialize.AddComponent<TestComponent7>().AccelerationX = 10;
				currentEntity_serialize.AddComponent<TestComponent8>().AccelerationY = 10;
				currentEntity_serialize.AddComponent<TestComponent9>().AccelerationZ = 10;
				currentEntity_serialize.AddComponent<TestComponent10>().VelocityX = 10;
				currentEntity_serialize.AddComponent<TestComponent11>().VelocityY = 10;
				currentEntity_serialize.AddComponent<TestComponent12>().VelocityZ = 10;
				if (!previousEntityArray_deserialize.TryCreateEntity(out Entity previousEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
				previousEntity_deserialize.AddComponent<TestComponent1>();
				previousEntity_deserialize.AddComponent<TestComponent2>();
				previousEntity_deserialize.AddComponent<TestComponent3>();
				previousEntity_deserialize.AddComponent<TestComponent4>();
				previousEntity_deserialize.AddComponent<TestComponent5>();
				previousEntity_deserialize.AddComponent<TestComponent6>();
				previousEntity_deserialize.AddComponent<TestComponent7>();
				previousEntity_deserialize.AddComponent<TestComponent8>();
				previousEntity_deserialize.AddComponent<TestComponent9>();
				previousEntity_deserialize.AddComponent<TestComponent10>();
				previousEntity_deserialize.AddComponent<TestComponent11>();
				previousEntity_deserialize.AddComponent<TestComponent12>();
				if (!currentEntityArray_deserialize.TryCreateEntity(out Entity currentEntity_deserialize)) { throw new Exception($"Could not create entity {entityID}."); }
				currentEntity_deserialize.AddComponent<TestComponent1>();
				currentEntity_deserialize.AddComponent<TestComponent2>();
				currentEntity_deserialize.AddComponent<TestComponent3>();
				currentEntity_deserialize.AddComponent<TestComponent4>();
				currentEntity_deserialize.AddComponent<TestComponent5>();
				currentEntity_deserialize.AddComponent<TestComponent6>();
				currentEntity_deserialize.AddComponent<TestComponent7>();
				currentEntity_deserialize.AddComponent<TestComponent8>();
				currentEntity_deserialize.AddComponent<TestComponent9>();
				currentEntity_deserialize.AddComponent<TestComponent10>();
				currentEntity_deserialize.AddComponent<TestComponent11>();
				currentEntity_deserialize.AddComponent<TestComponent12>();
			}
			previousEntityArray_serialize.EndUpdate();
			currentEntityArray_serialize.EndUpdate();
			previousEntityArray_deserialize.EndUpdate();
			currentEntityArray_deserialize.EndUpdate();

			OutgoingMessage outgoingMessage = new OutgoingMessage(new byte[500_000]);
			currentEntityArray_serialize.Serialize(previousEntityArray_serialize, outgoingMessage);
			IncomingMessage incomingMessage = new IncomingMessage(outgoingMessage.MessageData);
			incomingMessage.Length = outgoingMessage.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();
			{
				currentEntityArray_deserialize.Deserialize(previousEntityArray_deserialize, incomingMessage);
			}
			stopwatch.Stop();

			BenchmarkMetadata.Add("Entity capacity", entityCapacity);
			BenchmarkMetadata.Add("Entity count (before)", currentEntityArray_serialize.Count());
			BenchmarkMetadata.Add("Entity count (after) ", currentEntityArray_deserialize.Count());
			BenchmarkMetadata.Add("Component count", componentsDefinition.Count);
			BenchmarkMetadata.Add("Serialized size (bytes)", outgoingMessage.Length);
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
