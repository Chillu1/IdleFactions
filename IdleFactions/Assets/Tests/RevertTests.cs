using BreakInfinity;
using NUnit.Framework;

namespace IdleFactions.Tests
{
	public class RevertTests
	{
		private IRevertController _revertController;
		private IResourceController _resourceController;

		private const double Delta = 0.001d;

		[SetUp]
		public void Setup()
		{
			_revertController = new RevertController();
			_resourceController = new ResourceController();
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
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.AreEqual(1, _resourceController.GetResource(ResourceType.Light)?.Value);

			var upgrade = new Upgrade("TestMultiplier", new ResourceCost(ResourceType.Dark),
				upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2d));
			upgrade.SetupFaction(faction);
			upgrade.TryBuy();

			faction.Update(1f);
			Assert.AreEqual(3, _resourceController.GetResource(ResourceType.Light)?.Value);

			_revertController.RevertLastAction();

			faction.Update(1f);
			Assert.AreEqual(4, _resourceController.GetResource(ResourceType.Light)?.Value);
		}

		[Test]
		public void RevertNewResource()
		{
			_resourceController.Add(ResourceType.Water, 1d);
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Light, 0d) }
			}), null);
			faction.Unlock();
			faction.TryBuyPopulation(1);

			faction.Update(1f);
			Assert.AreEqual(0, _resourceController.GetResource(ResourceType.Dark)?.Value);

			var upgrade = new Upgrade("TestNewResource", new ResourceCost(ResourceType.Water),
				upgradeActions: new UpgradeActionNewResource(ResourceNeedsType.GenerateAdded, new ResourceAdded(ResourceType.Dark, 1d)));
			upgrade.SetupFaction(faction);
			upgrade.TryBuy();

			faction.Update(1f);
			Assert.AreEqual(1, _resourceController.GetResource(ResourceType.Dark)?.Value);

			_revertController.RevertLastAction();

			faction.Update(1f);
			Assert.AreEqual(1, _resourceController.GetResource(ResourceType.Dark)?.Value);
		}

		[Test]
		public void RevertLastPopulationPurchase()
		{
			BigDouble usedResource = 0d;
			_resourceController.Add(ResourceType.Dark, 10d);
			var faction = new Faction(FactionType.Divinity, new ResourceNeeds(new ResourceNeedsProperties()
			{
				Generate = new[] { new ResourceCost(ResourceType.Light, 1d) },
				CreateCost = new[] { new ResourceCost(ResourceType.Dark, 1d) }
			}), null);
			faction.Unlock();

			double multiplier = (double)faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			usedResource += 1d * multiplier;
			Assert.AreEqual(1, faction.Population);
			Assert.AreEqual(10 - usedResource, _resourceController.GetResource(ResourceType.Dark)?.Value);

			multiplier = (double)faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			usedResource += 1d * multiplier;

			Assert.AreEqual(10d - usedResource, _resourceController.GetResource(ResourceType.Dark)?.Value);
			Assert.AreEqual(2, faction.Population);

			multiplier = (double)faction.GetPopulationCostMultiplier(1);
			faction.TryBuyPopulation(1);
			usedResource += 1d * multiplier;

			Assert.AreEqual((double)(10d - usedResource), (double)_resourceController.GetResource(ResourceType.Dark)?.Value, Delta);
			Assert.AreEqual(3, faction.Population);

			_revertController.RevertLastAction();

			multiplier = (double)faction.GetPopulationCostMultiplier(1);
			usedResource -= 1d * multiplier;
			Assert.AreEqual((double)(10 - usedResource), (double)_resourceController.GetResource(ResourceType.Dark)?.Value, Delta);
			Assert.AreEqual(2, faction.Population);
		}
	}
}