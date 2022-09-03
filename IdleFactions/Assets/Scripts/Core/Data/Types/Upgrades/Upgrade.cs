namespace IdleFactions
{
	public class Upgrade : IRevertible
	{
		public string Id { get; }

		private ResourceCost[] Costs { get; }
		private IUpgradeAction[] UpgradeActions { get; }

		public bool Bought { get; private set; }

		private Faction _faction;

		private static IRevertController _revertController;
		private static IResourceController _resourceController;

		public Upgrade(string id, ResourceCost cost, params IUpgradeAction[] upgradeActions) :
			this(id, new[] { cost }, upgradeActions)
		{
		}

		public Upgrade(string id, ResourceCost[] costs, params IUpgradeAction[] upgradeActions)
		{
			Id = id;
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
				_faction.ResourceNeeds.RevertUpgradeAction(action);
		}

		private void Buy()
		{
			foreach (var action in UpgradeActions)
				_faction.ResourceNeeds.ActivateUpgradeAction(action);

			Bought = true;
		}
	}
}