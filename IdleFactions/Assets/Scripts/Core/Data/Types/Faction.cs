using System;
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

		public bool IsUnlocked { get; private set; }
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
			if (!IsUnlocked || Population <= 0)
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
			IsUnlocked = true;
		}

		public bool TryBuyPopulation(double amount)
		{
			if (!IsUnlocked)
				return false;

			double multiplier = CalculateFormula((int)(Population + amount)) - CalculateFormula((int)Population);

			if (!ResourceController.TryUseResource(ResourceNeeds.CreateCost.Values, multiplier))
				return false;

			ChangePopulation(amount);
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

		public void ChangePopulation(double amount)
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

		/// <summary>
		///		Sum of n ^ 0.15 for n = 0 to n
		/// </summary>
		public static double CalculateFormula(int n)
		{
			if (n == 1)
				return 1;

			const double fifth = 0.0001616362;
			const double fourth = 0.0049091246;
			const double third = 0.1112719416;
			const double second = 0.6244118593;
			//const double first = 0.0241495853;

			//Lower exponent removed, added simple 1 check. Scales like: Sum n ^ 0.15 for n = 0 to n
			return fifth * Math.Pow(n, 5) - fourth * Math.Pow(n, 4) + third * Math.Pow(n, 3) + second * Math.Pow(n, 2);
		}
	}
}