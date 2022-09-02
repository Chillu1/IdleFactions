using NUnit.Framework;

namespace IdleFactions.Tests
{
	public class ResourceNeedsTests
	{
		private IResourceController _resourceController;
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
		
		//Scaling CreateCost
		[Test]
		public void BuyPopulationScalingCost()
		{
			double resourceUsed = 0d;
			_resourceController.Add(ResourceType.Dark, 10);

			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}));
			faction.Unlock();

			Assert.AreEqual(0, faction.Population);
			faction.TryBuyPopulation(1);
			Assert.AreEqual(1, faction.Population);

			double multiplier = Faction.GetPopulationCostMultiplier(1, 0);
			resourceUsed += 1d * multiplier;
			Assert.AreEqual(resourceUsed, 10d - _resourceController.GetResource(ResourceType.Dark)?.Value);
			
			faction.TryBuyPopulation(1);
			Assert.AreEqual(2, faction.Population);
			
			multiplier = Faction.GetPopulationCostMultiplier(1, 1);
			resourceUsed += 1d * multiplier;

			Assert.AreEqual(resourceUsed, 10d - _resourceController.GetResource(ResourceType.Dark)?.Value);
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

		[Test]
		public void LiveCost()
		{
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}));
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);

			Assert.AreNotEqual(1, faction.Population);
		}

		//LiveCost = 1d = buff
		[Test]
		public void LiveGenerationBonusMultiplier()
		{
			_resourceController.Add(ResourceType.Dark, 5d);
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) },
				LiveCost = new[] { new ResourceCost(ResourceType.Dark, 5d) }
			}));
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);

			Assert.AreEqual(1d * Faction.MaxLiveBonusMultiplier, _resourceController.GetResource(ResourceType.Light)?.Value);
		}
	}
}