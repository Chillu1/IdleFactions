namespace IdleFactions
{
	public class Upgrade : IRevertible
	{
		public string Id { get; }

		private ResourceCost[] Costs { get; }
		private IUpgradeAction[] UpgradeActions { get; }

		public bool Unlocked { get; private set; }
		public bool Bought { get; private set; }

		private Faction _faction;

		private static IRevertController _revertController;
		private static IResourceController _resourceController;

		public Upgrade(string id, ResourceCost cost, bool unlocked = false, params IUpgradeAction[] upgradeActions) :
			this(id, new[] { cost }, unlocked, upgradeActions)
		{
		}

		public Upgrade(string id, ResourceCost[] costs, bool unlocked = false, params IUpgradeAction[] upgradeActions)
		{
			Id = id;
			Unlocked = unlocked;
			Costs = costs;
			UpgradeActions = upgradeActions;
		}

		public static void Setup(IRevertController revertController, IResourceController resourceController)
		{
			_revertController = revertController;
			_resourceController = resourceController;
		}

		public void SetupFaction(Faction faction)
		{
			_faction = faction;
		}

		public void Unlock()
		{
			Unlocked = true;
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
	}
}