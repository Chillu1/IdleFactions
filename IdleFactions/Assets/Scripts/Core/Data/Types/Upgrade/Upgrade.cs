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

		public bool Unlocked { get; protected set; }
		public bool Bought { get; private set; }

		public FactionType FactionType { get; private set; }
		private Faction _faction;

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

		protected Upgrade(string id, ResourceCost[] costs, bool unlocked, params IUpgradeAction[] upgradeActions)
		{
			Id = id;
			Costs = costs;
			Unlocked = unlocked;
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

		public void Unlock()
		{
			Unlocked = true;
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
			Bought = false;
			foreach (var action in UpgradeActions)
				_faction.RevertUpgradeAction(action);
		}

		private void Buy()
		{
			foreach (var action in UpgradeActions)
				_faction.ActivateUpgradeAction(action);

			Bought = true;
		}

		public string GetDataString()
		{
			string data = "Upgrade: ";
			data += Bought ? "Bought" : Unlocked ? "Unlocked" : "Locked";
			data += "\nEffects:\n";
			data += string.Join(", ", UpgradeActions.Select(action => action.ToString()));
			return data;
		}

		public string GetCostsString() => "Cost: " + string.Join(", ", Costs.Select(cost => cost.ToString()));

		public void Save(JsonTextWriter writer)
		{
			if (!Unlocked && !Bought) //Don't save if it's unnecessary, no delta
				return;

			writer.WriteStartObject();
			writer.WritePropertyName(nameof(Id));
			writer.WriteValue(Id);
			writer.WritePropertyName(nameof(Unlocked));
			writer.WriteValue(Unlocked);
			writer.WritePropertyName(nameof(Bought));
			writer.WriteValue(Bought);
			writer.WriteEndObject();
		}

		public void Load(JObject data)
		{
			Unlocked = data.Value<bool>(nameof(Unlocked));
			Bought = data.Value<bool>(nameof(Bought));

			if (Bought)
				Buy();
		}

		public Upgrade ShallowClone()
		{
			return new Upgrade(Id, Costs, Unlocked, UpgradeActions);
		}
	}
}