using BreakInfinity;
using NUnit.Framework;

namespace IdleFactions.Tests
{
	public class FactionTests
	{
		private IResourceController _resourceController;

		//Resource Tests

		[SetUp]
		public void Setup()
		{
			_resourceController = new ResourceController();
			Faction.Setup(new RevertController(), _resourceController);
		}

		[TearDown]
		public void TearDown()
		{
			_resourceController = null;
		}

		[Test]
		public void LessPopulation()
		{
			BigDouble startPopulation = 5;

			_resourceController.Add(ResourceType.Essence, double.MaxValue);
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}), null);
			faction.Unlock();

			faction.TryBuyPopulation(startPopulation);
			Assert.AreEqual(startPopulation, faction.Population);

			faction.Update((float)(1d * faction.PopulationDecay));
			Assert.Less(faction.Population, startPopulation);
		}

		[Test]
		public void MinPopulation()
		{
			BigDouble startPopulation = 5;

			_resourceController.Add(ResourceType.Essence, double.MaxValue);
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}), null);
			faction.Unlock();

			faction.TryBuyPopulation(startPopulation);
			Assert.AreEqual(startPopulation, faction.Population);

			faction.Update((float)(1d * faction.PopulationDecay));
			Assert.Greater(startPopulation, faction.Population);

			faction.Update((float)(10d * faction.PopulationDecay));
			Assert.AreEqual(faction.Population, Faction.MinPopulation);
		}
	}
}