using System.Collections.Generic;
//using Collections.Pooled;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace IdleFactions.Tests
{
	public class Benchmarks
	{
		private double[] _randomNumbers;

		[OneTimeSetUp]
		public void Setup()
		{
			_randomNumbers = new double[ResourceTypeHelper.ResourceTypes.Length + 1];
			var random = new System.Random();
			for (int i = 0; i < _randomNumbers.Length; i++)
			{
				_randomNumbers[i] = random.NextDouble();
			}
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			_randomNumbers = null;
		}

		[Test, Performance]
		public void DynamicDictionaryBuiltIn()
		{
			var dict = new Dictionary<ResourceType, double>(100);

			Measure.Method(() =>
				{
					dict.Add(ResourceType.Infinity, 1);
					//foreach (var type in ResourceTypeHelper.ResourceTypes)
					//	dict.Add(type, _randomNumbers[(int)type]);
				})
				.WarmupCount(10)
				.MeasurementCount(20)
				.IterationsPerMeasurement(1000)
				.GC()
				.CleanUp(() => { dict.Clear(); })
				.Run()
				;
		}

		/*[Test, Performance]
		public void DynamicDictionaryPooledDefault()
		{
			var dict = new PooledDictionary<ResourceType, double>(100);

			Measure.Method(() => { dict.Add(ResourceType.Infinity, 1); })
				.WarmupCount(10)
				.MeasurementCount(20)
				.IterationsPerMeasurement(1000)
				.GC()
				.CleanUp(() => { dict.Clear(); })
				.Run()
				;
		}*/
	}
}