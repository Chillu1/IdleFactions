using NUnit.Framework;

namespace IdleFactions.Tests
{
	public class ResourceNeedsTests
	{
		private ResourceController _resourceController;

		[SetUp]
		public void Setup()
		{
			var prestigeResourceData = new PrestigeResourceData();
			_resourceController = new ResourceController(prestigeResourceData, new PrestigeResourceController(prestigeResourceData));
			Faction.Setup(new RevertController(), _resourceController);
			_resourceController.Add(ResourceType.Essence, double.MaxValue);
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
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, 1));
		}

		//CreateCost Tests

		[Test]
		public void BuyPopulationNoResources()
		{
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}), null);
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(1, faction.Population);
		}

		[Test]
		public void BuyPopulationEnoughResources()
		{
			_resourceController.Add(ResourceType.Dark, 5);

			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}), null);
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(1, faction.Population);
		}

		[Test]
		public void BuyPopulationNotEnoughResources()
		{
			_resourceController.Add(ResourceType.Dark, 5);

			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 10d) }
			}), null);
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(0, faction.Population);
		}

		//Scaling CreateCost
		[Test]
		public void BuyPopulationScalingCost()
		{
			double resourceUsed = 0d;
			_resourceController.Add(ResourceType.Dark, 10);

			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}), null);
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			double multiplier = faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(1, faction.Population);

			resourceUsed += 1d * multiplier;
			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, 10 - resourceUsed));

			multiplier = faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(2, faction.Population);

			resourceUsed += 1d * multiplier;

			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, 10 - resourceUsed));
		}

		//Generate Cost Tests

		[Test]
		public void GenerateCost()
		{
			_resourceController.Add(ResourceType.Dark, 5d);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				GenerateCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, 1));
		}

		[Test]
		public void GenerateCostNotEnough()
		{
			_resourceController.Add(ResourceType.Dark, 2.5d);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				GenerateCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);

			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, 0));
			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, 0.5));
		}

		//LiveCost Tests

		[Test]
		public void LiveCost()
		{
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(10);

			faction.Update(1f);

			Assert.Less(faction.Population, 10);
		}

		[Test]
		public void LiveCostHalf()
		{
			_resourceController.Add(ResourceType.Dark, 2.5d);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(5);

			faction.Update(1f);

			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, 0));
			double remainingPopulation = 5 - faction.PopulationDecayFlat * 0.5;
			Assert.AreEqual(remainingPopulation, faction.Population, 0.01d);
			//Not 2.5, because 0.05 population is gone, making it: (5 - faction.PopulationDecay * 0.5) * 0.5 * 1 * 1 = 2.475
			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, remainingPopulation * 0.5));
		}

		//LiveCost = 1d = buff
		[Test]
		public void LiveGenerationBonusMultiplier()
		{
			_resourceController.Add(ResourceType.Dark, 5d);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);

			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, 1d * Faction.MaxLiveBonusMultiplier));
		}
	}
}