using System.Collections.Generic;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class Faction
	{
		public FactionType Type { get; }
		public ResourceNeeds ResourceNeeds { get; }

		public double Population { get; private set; }
		public double PopulationDecay { get; private set; } = 1d;

		public bool Unlocked { get; private set; }
		public bool IsGenerationOn { get; private set; } = true;

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

			if (!IsGenerationOn)
				return;

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

		public bool TryBuyPopulation()
		{
			if (!Unlocked)
				return false;

			ChangePopulation(1);
			return true;
		}

		public bool TryBuyUpgrade(int index)
		{
			var upgrade = _upgrades[index];
			if (upgrade.TryBuy())
			{
				//_upgrades.RemoveAt(index);
				return true;
			}

			return false;
		}

		[CanBeNull]
		public Upgrade GetUpgrade(int index)
		{
			if (index < 0 || index >= _upgrades.Count)
				return null;

			return _upgrades[index];
		}

		public string GetUpgradeId(int i)
		{
			return i >= _upgrades?.Count ? "Id" : _upgrades?[i].Id;
		}

		public void ChangePopulation(int amount)
		{
			Population += amount;
		}

		public void ToggleGeneration()
		{
			IsGenerationOn = !IsGenerationOn;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}