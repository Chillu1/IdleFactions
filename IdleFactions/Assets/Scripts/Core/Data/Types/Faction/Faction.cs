using System;
using System.Collections.Generic;
using System.Linq;
using IdleFactions.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public class Faction : ISavable, ILoadable, IDeepClone<Faction>, INotification
	{
		public FactionType Type { get; }
		public FactionResources FactionResources { get; }

		public double Population { get; private set; }
		public double PopulationDecayFlat { get; private set; } = 0.1d;
		public double PopulationDecayPercentage { get; private set; } = 0.000069444d;
		private readonly Formulas.FormulaType _populationFormula = Formulas.FormulaType.Exponential15;

		public string NotificationText => "Discovered faction: " + Type;
		public bool IsNew { get; private set; } = true;
		public bool HasNewUpgrades => Upgrades.Any(upgrade => upgrade.IsUnlocked && upgrade.IsNew);
		public bool IsDiscovered { get; private set; }
		public bool IsUnlocked { get; private set; }
		public bool IsGenerationOn { get; private set; } = true;

		public const double MinPopulation = 1d;
		public const double MinLiveMultiplier = 0.1d;
		public const double MaxLiveBonusMultiplier = 1.2d;

		public ValueRate PopulationValueRate { get; private set; }

		public static event Action<Faction> Discovered;
		public static event Action<Faction> Unlocked;

		private readonly Dictionary<ResourceType, double> _resourceCostAddedMultipliers;

		private IReadOnlyList<IUpgrade> Upgrades { get; }

		private static IRevertController _revertController;
		private static IResourceController _resourceController;

		public Faction(FactionType type, FactionResources factionResources, IReadOnlyList<IUpgrade> upgrades)
		{
			Type = type;
			FactionResources = factionResources;

			foreach (var upgrade in upgrades.EmptyIfNull())
				upgrade.SetupFaction(this);

			Upgrades = upgrades;

			_resourceCostAddedMultipliers = new Dictionary<ResourceType, double>();
		}

		public static void Setup(IRevertController revertController, IResourceController resourceController)
		{
			_revertController = revertController;
			_resourceController = resourceController;
		}

		public void Update(float delta)
		{
			if (!IsDiscovered || !IsUnlocked || Population <= 0)
				return;

			double usedLiveMultiplier = 1d;
			if (FactionResources.LiveCost != null &&
			    _resourceController.TryUsePartialLiveResource(FactionResources.LiveCost, Population * delta, out usedLiveMultiplier))
			{
				double flatPopulationDecay = PopulationDecayFlat * (1d - usedLiveMultiplier);
				double percentagePopulationDecay = Population * PopulationDecayPercentage * (1d - usedLiveMultiplier);
				Population -= (flatPopulationDecay + percentagePopulationDecay) * delta;
				if (Population < MinPopulation)
					Population = MinPopulation;
			}

			if (!IsGenerationOn)
				return;

			double usedGenMultiplier = 1d;
			if (FactionResources.GenerateCost != null)
				_resourceController.TryUsePartialResource(FactionResources.GenerateCost, Population * delta,
					out usedGenMultiplier);

			PopulationValueRate = usedLiveMultiplier >= 1d ? ValueRate.Neutral : ValueRate.Negative;
			if (usedLiveMultiplier < MinLiveMultiplier)
				usedLiveMultiplier = MinLiveMultiplier;

			_resourceController.Add(FactionResources.Generate, Population * usedLiveMultiplier * usedGenMultiplier * delta);

			if (FactionResources.GenerateCostAdded != null)
				_resourceController.TryUsePartialResourceAdded(FactionResources.GenerateCostAdded, Population * delta,
					_resourceCostAddedMultipliers);
			_resourceController.Add(FactionResources.GenerateAdded, _resourceCostAddedMultipliers,
				Population * usedLiveMultiplier * usedGenMultiplier * delta);

			_resourceCostAddedMultipliers.Clear();
		}

		public void SetNotNew()
		{
			if (!IsNew)
				return;
			IsNew = false;
		}

		public void Discover()
		{
			if (IsDiscovered)
				return;
			IsDiscovered = true;
			Discovered?.Invoke(this);
		}

		public void Unlock()
		{
			Discover();
			if (IsUnlocked)
				return;
			IsUnlocked = true;
			Unlocked?.Invoke(this);
		}

		public bool TryBuyPopulation(int amount)
		{
			if (!IsDiscovered || !IsUnlocked)
				return false;

			double multiplier = GetPopulationCostMultiplier(amount);

			if (!_resourceController.TryUseManualResource(FactionResources.CreateCost, multiplier))
				return false;

			_revertController.AddAction(new PopulationPurchase(this, amount, multiplier));
			ChangePopulation(amount);
			return true;
		}

		public bool TryBuyPopulation(PurchaseType purchaseType)
		{
			if (!IsDiscovered || !IsUnlocked)
				return false;

			throw new NotImplementedException();
			/*int percent;

			switch (purchaseType)
			{
				case PurchaseType.One:
					return TryBuyPopulation(1);
				case PurchaseType.Ten:
					return TryBuyPopulation(10);
				case PurchaseType.Hundred:
					return TryBuyPopulation(100);
				case PurchaseType.TenPercent:
					percent = 10;
					break;
				case PurchaseType.TwentyFivePercent:
					percent = 25;
					break;
				case PurchaseType.FiftyPercent:
					percent = 50;
					break;
				case PurchaseType.HundredPercent:
					percent = 100;
					break;
			}

			if (!_resourceController.ContainsResources(FactionResources.CreateCost)) //Not enough resource for 1
				return false;

			//TODO Rethink this
			int amount = _resourceController.GetResourcesPopulationAmount(FactionResources.CreateCost, purchaseType);
			if (amount <= 0)
				return false;


			double multiplier = GetPopulationCostMultiplier(amount);*/
		}

		public void RevertPopulation(double amount, double multiplier)
		{
			_resourceController.Add(FactionResources.CreateCost, multiplier);
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
		public IUpgrade GetUpgrade(int index)
		{
			if (Upgrades == null)
				return null;
			if (index < 0 || index >= Upgrades.Count)
				return null;

			return Upgrades[index];
		}

		[CanBeNull]
		public IUpgrade GetUpgrade(string id)
		{
			return Upgrades == null ? null : Upgrades.FirstOrDefault(u => u.Id == id);
		}

		public string GetUpgradeId(int i)
		{
			return i >= Upgrades?.Count ? "Unknown" : Upgrades?[i].Id;
		}

		public void ActivateUpgradeAction(IUpgradeAction action)
		{
			switch (action)
			{
				case UpgradeActionUnlock actionUnlock:
					Unlock();
					return;
			}

			FactionResources.ActivateUpgradeAction(action);
		}

		public void RevertUpgradeAction(IUpgradeAction action)
		{
			switch (action)
			{
				case UpgradeActionUnlock actionUnlock:
					IsUnlocked = false;
					return;
			}

			FactionResources.RevertUpgradeAction(action);
		}

		public void ChangePopulation(double amount)
		{
			Population += amount;
		}

		public void ToggleGeneration()
		{
			IsGenerationOn = !IsGenerationOn;
		}

		public static void CleanUp()
		{
			Discovered = null;
			Unlocked = null;
		}

		public double GetPopulationCostMultiplier(int amount)
		{
			return Formulas.GetPopulationCostMultiplier(_populationFormula, amount, (int)Math.Ceiling(Population));
		}

		public void Save(JsonTextWriter writer)
		{
			if (!IsDiscovered && !IsUnlocked) //Don't save if it's unnecessary, no delta
				return;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(Type));
			writer.WriteValue(Type);

			writer.WritePropertyName(nameof(IsDiscovered));
			writer.WriteValue(IsDiscovered);

			writer.WritePropertyName(nameof(IsUnlocked));
			writer.WriteValue(IsUnlocked);

			writer.WritePropertyName(nameof(IsGenerationOn));
			writer.WriteValue(IsGenerationOn);

			writer.WritePropertyName(nameof(Population));
			writer.WriteValue(Population);

			writer.WritePropertyName(nameof(Upgrades));
			writer.WriteStartArray();
			foreach (var upgrade in Upgrades.EmptyIfNull())
				upgrade.Save(writer);
			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public void Load(JObject data)
		{
			//IsDiscovered = data.Value<bool>(nameof(IsDiscovered));
			//IsUnlocked = data.Value<bool>(nameof(IsUnlocked));
			//Will trigger UI notifications later on, unless we subscribe with them after loading
			if (data.Value<bool>(nameof(IsDiscovered)))
				Discover();
			if (data.Value<bool>(nameof(IsUnlocked)))
				Unlock();

			IsGenerationOn = data.Value<bool>(nameof(IsGenerationOn));
			Population = data.Value<double>(nameof(Population));

			var upgradesData = data.Value<JArray>(nameof(Upgrades));
			foreach (var upgradeData in upgradesData.EmptyIfNull())
				Upgrades.First(u => u.Id == upgradeData.Value<string>(nameof(Upgrade.Id))).Load(upgradeData as JObject);
		}

		public Faction DeepClone()
		{
			return new Faction(Type, FactionResources.DeepClone(), Upgrades?.Select(u => u.ShallowClone()).ToList());
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}

		private static class Formulas
		{
			public enum FormulaType
			{
				None = 0,
				Exponential15,
				Exponential2,
				Exponential25,
				SummedExponential15,
			}

			public static double GetPopulationCostMultiplier(FormulaType type, int amount, int population)
			{
				if (amount + population <= 1)
					return 1d;

				switch (type)
				{
					//TODO Fix correct calculations
					case FormulaType.Exponential15:
						return Math.Pow(population + amount, 1.5d);
					case FormulaType.Exponential2:
						return Math.Pow(population + amount, 2);
					case FormulaType.Exponential25:
						return Math.Pow(population + amount, 2.5d);
					case FormulaType.SummedExponential15:
						return GetSummedExponential15Formula(population + amount) - GetSummedExponential15Formula(population);
					default:
						Log.Error(type + " formula not implemented");
						break;
				}

				return 1d;
			}

			/// <summary>
			///		Sum of n ^ 1.5 for n = 0 to n
			/// </summary>
			private static double GetSummedExponential15Formula(int n)
			{
				const double fifth = 0.0001616362;
				const double fourth = 0.0049091246;
				const double third = 0.1112719416;
				const double second = 0.6244118593;
				//const double first = 0.0241495853;

				//Lower exponent removed, added simple 1 check. Scales like: Sum n ^ 1.5 for n = 0 to n
				return fifth * Math.Pow(n, 5) - fourth * Math.Pow(n, 4) + third * Math.Pow(n, 3) + second * Math.Pow(n, 2);
			}
		}
	}
}