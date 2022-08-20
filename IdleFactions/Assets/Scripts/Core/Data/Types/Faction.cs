using System.Collections.Generic;

namespace IdleFactions
{
	public class Faction
	{
		public FactionType Type { get; }
		public double Population { get; private set; }
		public double PopulationDecay { get; private set; } = 1d;

		public ResourceNeeds ResourceNeeds { get; }

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
			_upgrades = upgrades;
		}

		public void Update(float delta)
		{
			if (Population <= 0)
				return;

			if (ResourceNeeds.LiveCost != null &&
			    ResourceController.TryUseLiveResource(ResourceNeeds.LiveCost.Values, Population * delta, out double usedLiveMultiplier))
			{
				//Partial use
				if (usedLiveMultiplier < 1d)
					Population -= PopulationDecay * (1d - usedLiveMultiplier) * delta;
			}

			double usedGenMultiplier = 1d;
			if (ResourceNeeds.GenerateCost == null ||
			    ResourceController.TryUseLiveResource(ResourceNeeds.GenerateCost.Values, Population * delta, out usedGenMultiplier))
			{
				ResourceController.Add(ResourceNeeds.Generate.Values, Population * usedGenMultiplier * delta);
			}
		}

		public void TryUpgrade()
		{
			var upgrade = _upgrades[0];
			if (ResourceController.TryUseResource(upgrade.Cost.Type, upgrade.Cost.Value))
			{
				upgrade.Apply();
				_upgrades.RemoveAt(0);
			}
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