using System;

namespace IdleFactions
{
	public sealed class PrestigeUpgrade : IPrestigeUpgrade
	{
		public string Id { get; }
		public PrestigeResourceCost[] Costs { get; }
		public FactionType FactionType { get; }
		private IPrestigeUpgradeAction Action { get; }

		public bool IsBought { get; private set; }

		private PrestigeResourceController _prestigeResourceController;
		private Faction _faction;

		public static event Action<PrestigeUpgrade> Bought;

		public PrestigeUpgrade(string id, FactionType factionType, PrestigeResourceCost cost, IPrestigeUpgradeAction action)
			: this(id, factionType, new[] { cost }, action)
		{
		}

		public PrestigeUpgrade(string id, FactionType factionType, PrestigeResourceCost[] costs, IPrestigeUpgradeAction action)
		{
			Id = id;
			FactionType = factionType;
			Costs = costs;
			Action = action;
		}

		public void Setup(PrestigeResourceController prestigeResourceController)
		{
			_prestigeResourceController = prestigeResourceController;
		}

		public void SetupFaction(Faction faction)
		{
			if (faction.Type != FactionType)
				return;

			_faction = faction;
		}

		public bool TryBuy()
		{
			if (!_prestigeResourceController.TrySpend(Costs))
				return false;

			Buy();
			//_revertController.AddAction(this);

			return true;
		}

		private void Buy()
		{
			switch (Action)
			{
				case PrestigeUpgradeAction.UnlockUpgrade action:
					_faction.GetUpgrade(action.UpgradeId)?.Unlock();
					break;
				default:
					Log.Error($"Unknown action type: {Action.GetType()}");
					break;
			}

			//_faction.ActivateUpgradeAction(Action);

			IsBought = true;
			Bought?.Invoke(this);
		}

		//Load if (IsBought)
		//Buy();
	}
}