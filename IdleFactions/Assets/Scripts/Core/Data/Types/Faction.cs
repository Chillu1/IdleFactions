using System;
using System.Collections.Generic;
using IdleFactions.Utils;
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
		public const double MaxLiveBonusMultiplier = 1.2d;

		private IReadOnlyList<Upgrade> Upgrades { get; }

		private static IRevertController _revertController;
		private static IResourceController _resourceController;

		public Faction(FactionType type, ResourceNeeds resourceNeeds, IReadOnlyList<Upgrade> upgrades)
		{
			Type = type;
			ResourceNeeds = resourceNeeds;

			foreach (var upgrade in upgrades.EmptyIfNull())
				upgrade.SetupFaction(this);

			Upgrades = upgrades;
		}

		public static void Setup(IRevertController revertController, IResourceController resourceController)
		{
			_revertController = revertController;
			_resourceController = resourceController;
		}

		public void Update(float delta)
		{
			if (!IsUnlocked || Population <= 0)
				return;

			double usedLiveMultiplier = 1d;
			if (ResourceNeeds.LiveCost != null &&
			    _resourceController.TryUsePartialLiveResource(ResourceNeeds.LiveCost.Values, Population * delta, out usedLiveMultiplier))
			{
				Population -= PopulationDecay * (1d - usedLiveMultiplier) * delta;
				if (Population < MinPopulation)
					Population = MinPopulation;
			}

			if (!IsGenerationOn)
				return;

			double usedGenMultiplier = 1d;
			if (ResourceNeeds.GenerateCost != null)
				_resourceController.TryUsePartialResource(ResourceNeeds.GenerateCost.Values, Population * delta,
					out usedGenMultiplier);

			if (usedLiveMultiplier < MinLiveMultiplier)
				usedLiveMultiplier = MinLiveMultiplier;
			_resourceController.Add(ResourceNeeds.Generate.Values, Population * usedLiveMultiplier * usedGenMultiplier * delta);
		}

		public void Unlock()
		{
			IsUnlocked = true;
		}

		public bool TryBuyPopulation(double amount)
		{
			if (!IsUnlocked)
				return false;

			double multiplier = GetPopulationCostMultiplier(amount);

			if (!_resourceController.TryUseResource(ResourceNeeds.CreateCost.Values, multiplier))
				return false;

			_revertController.AddAction(new PopulationPurchase(this, amount, multiplier));
			ChangePopulation(amount);
			return true;
		}

		public void RevertPopulation(double amount, double multiplier)
		{
			_resourceController.Add(ResourceNeeds.CreateCost.Values, multiplier);
			ChangePopulation(-amount);
		}

		public bool TryBuyUpgrade(int index)
		{
			var upgrade = Upgrades[index];
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
			if (index < 0 || index >= Upgrades.Count)
				return null;

			return Upgrades[index];
		}

		public string GetUpgradeId(int i)
		{
			return i >= Upgrades?.Count ? "Id" : Upgrades?[i].Id;
		}

		public void ActivateUpgradeAction(IUpgradeAction action)
		{
			switch (action)
			{
				case UpgradeActionUnlock actionUnlock:
					Unlock();
					return;
			}

			ResourceNeeds.ActivateUpgradeAction(action);
		}

		public void RevertUpgradeAction(IUpgradeAction action)
		{
			switch (action)
			{
				case UpgradeActionUnlock actionUnlock:
					IsUnlocked = false;
					return;
			}

			ResourceNeeds.RevertUpgradeAction(action);
		}

		public void ChangePopulation(double amount)
		{
			Population += amount;
		}

		public void ToggleGeneration()
		{
			IsGenerationOn = !IsGenerationOn;
		}

		public double GetPopulationCostMultiplier(double amount)
		{
			return GetPopulationCostMultiplier(amount, Population);
		}

		private static double GetPopulationCostMultiplier(double amount, double population)
		{
			if (amount + population <= 1)
				return 1d;

			//population -= 1; //Scratch that, no need to offset since since only the first purchase should be 1 //Offset population, to make the first purchase 1X, and next to be scaled accordingly

			return GetScalingFormula((int)(population + amount)) - GetScalingFormula((int)population);
		}

		/// <summary>
		///		Sum of n ^ 0.15 for n = 0 to n
		/// </summary>
		private static double GetScalingFormula(int n)
		{
			const double fifth = 0.0001616362;
			const double fourth = 0.0049091246;
			const double third = 0.1112719416;
			const double second = 0.6244118593;
			//const double first = 0.0241495853;

			//Lower exponent removed, added simple 1 check. Scales like: Sum n ^ 0.15 for n = 0 to n
			return fifth * Math.Pow(n, 5) - fourth * Math.Pow(n, 4) + third * Math.Pow(n, 3) + second * Math.Pow(n, 2);
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}