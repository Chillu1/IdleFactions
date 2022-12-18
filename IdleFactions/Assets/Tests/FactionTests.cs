using NUnit.Framework;

namespace IdleFactions.Tests
{
	public class FactionTests
	{
		private ResourceController _resourceController;

		//Resource Tests

		[SetUp]
		public void Setup()
		{
			var prestigeResourceData = new PrestigeResourceData();
			_resourceController = new ResourceController(prestigeResourceData, new PrestigeResourceController(prestigeResourceData));
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
			const int startPopulation = 5;

			_resourceController.Add(ResourceType.Essence, double.MaxValue);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}));
			faction.Unlock();

			faction.TryBuyPopulation(startPopulation);
			Assert.AreEqual(startPopulation, faction.Population);

			faction.Update((float)(1d * faction.PopulationDecayFlat));
			Assert.Less(faction.Population, startPopulation);
		}

		[Test]
		public void MinPopulation()
		{
			const int startPopulation = 5;

			_resourceController.Add(ResourceType.Essence, double.MaxValue);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}));
			faction.Unlock();

			faction.TryBuyPopulation(startPopulation);
			Assert.AreEqual(startPopulation, faction.Population);

			faction.Update((float)(1d * faction.PopulationDecayFlat));
			Assert.Greater(startPopulation, faction.Population);

			faction.Update((float)(faction.Population / faction.PopulationDecayFlat));
			Assert.AreEqual(Faction.MinPopulation, faction.Population);
		}
	}
}