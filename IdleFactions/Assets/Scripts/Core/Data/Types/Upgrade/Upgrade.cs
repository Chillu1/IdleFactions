using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public class Upgrade : IProgressionUpgrade
	{
		public string Id { get; }

		public ResourceCost[] Costs { get; }
		private IUpgradeAction[] UpgradeActions { get; }

		public string NotificationText => $"Unlocked upg: {(FactionType == FactionType.None ? _faction.Type : FactionType)}, {Id}";

		public bool IsUnlocked { get; protected set; }
		public bool IsBought { get; private set; }

		public bool IsNew { get; private set; } = true;

		public FactionType FactionType { get; private set; }
		private Faction _faction;

		public static event Action<Upgrade> Bought;
		public static event Action<IUpgrade> Unlocked;

		private static IRevertController _revertController;
		private static IResourceController _resourceController;

		public Upgrade(string id, ResourceCost cost, params IUpgradeAction[] upgradeActions) : this(id, new[] { cost }, upgradeActions)
		{
		}

		public Upgrade(string id, ResourceCost[] costs, params IUpgradeAction[] upgradeActions)
		{
			Id = id;
			Costs = costs;
			UpgradeActions = upgradeActions;
		}

		protected Upgrade(string id, ResourceCost[] costs, bool isUnlocked, params IUpgradeAction[] upgradeActions)
		{
			Id = id;
			Costs = costs;
			IsUnlocked = isUnlocked;
			UpgradeActions = upgradeActions;
		}

		public static void Setup(IRevertController revertController, IResourceController resourceController)
		{
			_revertController = revertController;
			_resourceController = resourceController;
		}

		public void SetupFactionType(FactionType factionType)
		{
			FactionType = factionType;
		}

		public void SetupFaction(Faction faction)
		{
			_faction = faction;
		}

		public void SetNotNew()
		{
			if (!IsNew)
				return;
			IsNew = false;
		}

		public void Unlock()
		{
			IsUnlocked = true;
			if (_faction.IsDiscovered)
				Unlocked?.Invoke(this);
			if (Id.Contains("unlock", StringComparison.InvariantCultureIgnoreCase))
				Log.Error($"Unlock faction upgrades should always be unlocked, id: {Id}, factionType: {FactionType}");
		}

		public bool TryBuy()
		{
			if (!_resourceController.TryUseResource(Costs))
				return false;

			Buy();
			_revertController.AddAction(this);
			return true;
		}

		public void Revert()
		{
			_resourceController.Add(Costs);
			IsBought = false;
			foreach (var action in UpgradeActions)
				_faction.RevertUpgradeAction(action);
		}

		private void Buy()
		{
			foreach (var action in UpgradeActions)
				_faction.ActivateUpgradeAction(action);

			IsBought = true;
			Bought?.Invoke(this);
		}

		public static void CleanUp()
		{
			Bought = null;
			Unlocked = null;
		}

		public string GetDataString()
		{
			string data = "Upgrade: ";
			data += IsBought ? "Bought" : IsUnlocked ? "Unlocked" : "Locked";
			data += "\nEffects:\n";
			data += string.Join(", ", UpgradeActions.Select(action => action.ToString()));
			return data;
		}

		public string GetCostsString() => "Cost: " + string.Join(", ", Costs.Select(cost => cost.ToString()));

		public void Save(JsonTextWriter writer)
		{
			if (!IsUnlocked && !IsBought) //Don't save if it's unnecessary, no delta
				return;

			writer.WriteStartObject();
			writer.WritePropertyName(nameof(Id));
			writer.WriteValue(Id);
			writer.WritePropertyName(nameof(IsUnlocked));
			writer.WriteValue(IsUnlocked);
			writer.WritePropertyName(nameof(IsBought));
			writer.WriteValue(IsBought);
			writer.WriteEndObject();
		}

		public void Load(JObject data)
		{
			IsUnlocked = data.Value<bool>(nameof(IsUnlocked));
			IsBought = data.Value<bool>(nameof(IsBought));

			if (IsBought)
				Buy();
		}

		public Upgrade ShallowClone()
		{
			return new Upgrade(Id, Costs, IsUnlocked, UpgradeActions);
		}
	}
}