using System.Collections.Generic;

namespace IdleFactions
{
	public class Faction
	{
		public FactionType Type { get; }
		public ResourceNeeds ResourceNeeds { get; }

		public double Population { get; private set; }
		public double PopulationDecay { get; private set; } = 1d;
		public bool Unlocked { get; private set; }

		public const double MinPopulation = 1d;
		public const double MinLiveMultiplier = 0.1d;

		private List<Upgrade> _upgrades;

		private static ResourceController ResourceController { get; set; }

		public Faction(FactionType type, ResourceNeeds resourceNeeds)
		{
			Type = type;
			ResourceNeeds = resourceNeeds;
		}

		public static void Setup(ResourceController resourceController)
		{
			ResourceController = resourceController;
		}

		public void SetupUpgrades(List<Upgrade> upgrades)
		{
			foreach (var upgrade in upgrades)
				upgrade.SetupFaction(this);

			_upgrades = upgrades;
		}

		public void Update(float delta)
		{
			if (!Unlocked || Population <= 0)
				return;

			double usedLiveMultiplier = 1d;
			if (ResourceNeeds.LiveCost != null &&
			    ResourceController.TryUseLiveResource(ResourceNeeds.LiveCost.Values, Population * delta, out usedLiveMultiplier))
			{
				//Partial use
				if (usedLiveMultiplier < 1d)
					Population -= PopulationDecay * (1d - usedLiveMultiplier) * delta;
				if (Population < MinPopulation)
					Population = MinPopulation;
			}

			double usedGenMultiplier = 1d;
			if (ResourceNeeds.GenerateCost == null ||
			    ResourceController.TryUseLiveResource(ResourceNeeds.GenerateCost.Values, Population * delta, out usedGenMultiplier))
			{
				if (usedLiveMultiplier < MinLiveMultiplier)
					usedLiveMultiplier = MinLiveMultiplier;
				ResourceController.Add(ResourceNeeds.Generate.Values, Population * usedLiveMultiplier * usedGenMultiplier * delta);
			}
		}

		public void Unlock()
		{
			Unlocked = true;
		}

		public void TryUpgrade(int index)
		{
			var upgrade = _upgrades[index];
			if (upgrade.TryBuy())
				_upgrades.RemoveAt(index);
		}

		public string GetUpgradeId(int i)
		{
			return i >= _upgrades?.Count ? "Id" : _upgrades?[i].Id;
		}

		public void ChangePopulation(int amount)
		{
			Population += amount;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}