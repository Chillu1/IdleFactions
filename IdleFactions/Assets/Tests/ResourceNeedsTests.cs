using NUnit.Framework;

namespace IdleFactions.Tests
{
	public class ResourceNeedsTests
	{
		private ResourceController _resourceController;
		//NeedsTests
		//Generate - Generate no cost 
		//CreateCost - Create no cost, Cost, Not Enough 
		//GenerateCost - Generate cost, generate not enough
		//LiveCost - Cost, Not Enough - , Zero

		[SetUp]
		public void Setup()
		{
			_resourceController = new ResourceController();
			Faction.Setup(_resourceController);
		}

		[TearDown]
		public void TearDown()
		{
			_resourceController = null;
		}

		//Generate Tests

		[Test]
		public void Generate()
		{
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}));
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.AreEqual(1, _resourceController.GetResource(ResourceType.Light)?.Value);
		}

		//CreateCost Tests

		[Test]
		public void BuyPopulationNoResources()
		{
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}));
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(1, faction.Population);
		}

		[Test]
		public void BuyPopulationEnoughResources()
		{
			_resourceController.Add(ResourceType.Dark, 5);

			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}));
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(1, faction.Population);
		}

		[Test]
		public void BuyPopulationNotEnoughResources()
		{
			_resourceController.Add(ResourceType.Dark, 5);

			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 10d) }
			}));
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(0, faction.Population);
		}

		//Generate Cost Tests

		[Test]
		public void GenerateCost()
		{
			_resourceController.Add(ResourceType.Dark, 5d);
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				GenerateCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}));
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.AreEqual(1, _resourceController.GetResource(ResourceType.Light)?.Value);
		}

		[Test]
		public void GenerateCostNotEnough()
		{
			_resourceController.Add(ResourceType.Dark, 3d);
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				GenerateCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}));
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.Greater(1, _resourceController.GetResource(ResourceType.Dark)?.Value);
		}

		//LiveCost Tests

		//LiveCost = 1d = buff
	}
}