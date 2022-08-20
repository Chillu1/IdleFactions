using System;

namespace IdleFactions
{
	public class Upgrade
	{
		public string Id { get; }
		public ResourceCost[] Costs { get; }
		private Action UpgradeAction { get; }

		public bool Bought { get; private set; }

		private static ResourceController _resourceController;

		public Upgrade(string id, ResourceCost cost, Action upgradeAction) :
			this(id, new[] { cost }, upgradeAction) //TODO Preset set of upgrade actions
		{
		}

		public Upgrade(string id, ResourceCost[] costs, Action upgradeAction) //TODO Preset set of upgrade actions
		{
			Id = id;
			Costs = costs;
			UpgradeAction = upgradeAction;
		}

		public static void Setup(ResourceController resourceController)
		{
			_resourceController = resourceController;
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
			UpgradeAction();
			Bought = true;
		}
	}
}