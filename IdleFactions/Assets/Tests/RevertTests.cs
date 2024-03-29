using NUnit.Framework;

namespace IdleFactions.Tests
{
	public class RevertTests
	{
		private IRevertController _revertController;
		private ResourceController _resourceController;

		private const double Delta = 0.001d;

		[SetUp]
		public void Setup()
		{
			_revertController = new RevertController();
			var prestigeResourceData = new PrestigeResourceData();
			_resourceController = new ResourceController(prestigeResourceData, new PrestigeResourceController(prestigeResourceData));
			Upgrade.Setup(_revertController, _resourceController);
			Faction.Setup(_revertController, _resourceController);
			_resourceController.Add(ResourceType.Essence, double.MaxValue);
		}

		[TearDown]
		public void TearDown()
		{
			_revertController = null;
			_resourceController = null;
		}

		[Test]
		public void RevertMultiplier()
		{
			_resourceController.Add(ResourceType.Dark, 1d);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}));
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, 1));

			var upgrade = new Upgrade("TestMultiplier", new ResourceCost(ResourceType.Dark),
				upgradeActions: new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Light, 2d));
			upgrade.SetupFaction(faction);
			upgrade.TryBuy();

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, 3));

			_revertController.RevertLastAction();

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Light, 4));
		}

		[Test]
		public void RevertNewResource()
		{
			_resourceController.Add(ResourceType.Water, 1d);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}));
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, 0));

			var upgrade = new Upgrade("TestNewResource", new ResourceCost(ResourceType.Water),
				upgradeActions: new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded,
					new AddedResource(ResourceType.Dark, 1d)));
			upgrade.SetupFaction(faction);
			upgrade.TryBuy();

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, 1));

			_revertController.RevertLastAction();

			faction.Update(1f);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, 1));
		}

		[Test]
		public void RevertLastPopulationPurchase()
		{
			double usedResource = 0d;
			const double initialResource = 100d;
			_resourceController.Add(ResourceType.Dark, initialResource);
			var faction = new Faction(FactionType.Divinity, "", new FactionResources(new FactionResourceProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}));
			faction.Unlock();

			double multiplier = faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			usedResource += 1d * multiplier;
			Assert.AreEqual(1, faction.Population);
			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, initialResource - usedResource));

			multiplier = faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			usedResource += 1d * multiplier;

			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, initialResource - usedResource));
			Assert.AreEqual(2, faction.Population);

			multiplier = faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			usedResource += 1d * multiplier;

			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, initialResource - usedResource));
			Assert.AreEqual(3, faction.Population);

			_revertController.RevertLastAction();

			multiplier = faction.GetPopulationCostMultiplier(1);
			usedResource -= 1d * multiplier;
			Assert.True(_resourceController.ResourceEquals(ResourceType.Dark, initialResource - usedResource));
			Assert.AreEqual(2, faction.Population);
		}
	}
}