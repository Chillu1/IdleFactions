using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	//Show all upgrades by default
	//If option checked, show only non-bought upgrades
	public sealed class UpgradeController : ISavable, ILoadable
	{
		public const string JSONKey = "Upgrades";

		private readonly IReadOnlyDictionary<FactionType, IReadOnlyList<IUpgrade>> _upgrades;

		public UpgradeController(UpgradeData upgradeData, FactionController factionController)
		{
			_upgrades = upgradeData.GetAll();

			foreach (var kvp in _upgrades)
			{
				var faction = factionController.Get(kvp.Key);
				foreach (var upgrade in kvp.Value)
					upgrade.SetupFaction(faction);
			}
		}

		public bool HasNewUpgrades(FactionType factionType)
		{
			if (!_upgrades.TryGetValue(factionType, out var factionUpgrades))
				return false;
			return factionUpgrades?.Any(upgrade => upgrade.IsUnlocked && upgrade.IsNew) == true;
		}

		public bool TryBuy(FactionType factionType, int index, bool showOnlyNonBought = false)
		{
			if (_upgrades.TryGetValue(factionType, out var upgrades) == false)
				return false;

			if (showOnlyNonBought)
				upgrades = upgrades.Where(x => x.IsBought == false).ToArray();

			if (index < 0 || index >= upgrades.Count)
			{
				Log.Error($"Upgrade with index {index} not found");
				return false;
			}

			return upgrades[index].TryBuy();
		}

		public bool TryBuy(FactionType factionType, string id)
		{
			if (!_upgrades.TryGetValue(factionType, out var factionUpgrades))
				return false;

			var upgrade = factionUpgrades.FirstOrDefault(x => x.Id == id);
			if (upgrade == null)
			{
				Log.Error($"Upgrade with id {id} not found. On faction {factionType}");
				return false;
			}

			return upgrade.TryBuy();
		}

		[CanBeNull]
		public IUpgrade Get(FactionType factionType, int index, bool showOnlyNonBought = false)
		{
			if (_upgrades.TryGetValue(factionType, out var upgrades) == false)
				return null;

			if (showOnlyNonBought)
				upgrades = upgrades.Where(x => x.IsBought == false).ToArray();

			if (index < 0 || index >= upgrades.Count)
			{
				//Log.Error($"Upgrade with index {index} not found");
				return null;
			}

			return upgrades[index];
		}

		[CanBeNull]
		public IUpgrade Get(FactionType factionType, string id)
		{
			return !_upgrades.TryGetValue(factionType, out var factionUpgrades)
				? null
				: factionUpgrades.FirstOrDefault(x => x.Id == id);
		}

		public string GetId(FactionType factionType, int index)
		{
			if (_upgrades.TryGetValue(factionType, out var factionUpgrades))
				return index >= factionUpgrades?.Count ? "Unknown" : factionUpgrades?[index].Id;

			return "Unknown";
		}

		public void Save(JsonTextWriter writer)
		{
			writer.WritePropertyName(JSONKey);
			writer.WriteStartArray();

			foreach (var upgradesList in _upgrades)
			{
				writer.WritePropertyName(upgradesList.Key.ToString());
				writer.WriteStartArray();
				foreach (var upgrade in upgradesList.Value)
					upgrade.Save(writer);
				writer.WriteEndArray();
			}

			writer.WriteEndArray();
		}

		public void Load(JObject data)
		{
			var upgrades = data.Value<JArray>(JSONKey);
			foreach (var factionUpgrades in upgrades!)
			{
				FactionType key = (FactionType)Enum.Parse(typeof(FactionType), factionUpgrades.Path);
				foreach (var upgrade in factionUpgrades)
					_upgrades[key].First(x => x.Id == upgrade.Value<string>(nameof(Upgrade.Id))).Load(upgrade.Value<JObject>());
			}
		}
	}
}