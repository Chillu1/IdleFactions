namespace IdleFactions
{
	public class Upgrade
	{
		public string Id { get; }

		private ResourceCost[] Costs { get; }
		private UpgradeAction[] UpgradeActions { get; }

		public bool Bought { get; private set; }

		private static IResourceController _resourceController;
		private Faction _faction;

		public Upgrade(string id, ResourceCost cost, params UpgradeAction[] upgradeActions) :
			this(id, new[] { cost }, upgradeActions)
		{
		}

		public Upgrade(string id, ResourceCost[] costs, params UpgradeAction[] upgradeActions)
		{
			Id = id;
			Costs = costs;
			UpgradeActions = upgradeActions;
		}

		public static void Setup(IResourceController resourceController)
		{
			_resourceController = resourceController;
		}

		public void SetupFaction(Faction faction)
		{
			_faction = faction;
		}

		public bool TryBuy()
		{
			if (!_resourceController.TryUseResource(Costs))
				return false;

			Buy();
			return true;
		}

		private void Buy()
		{
			foreach (var upgradeAction in UpgradeActions)
				_faction.ResourceNeeds.ChangeMultiplier(upgradeAction);

			Bought = true;
		}
	}
}