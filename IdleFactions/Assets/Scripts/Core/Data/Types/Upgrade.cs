using System;

namespace IdleFactions
{
	public class Upgrade
	{
		public string Id { get; }
		public ResourceCost Cost { get; }
		private Action UpgradeAction { get; }

		public bool Bought { get; private set; }

		public Upgrade(string id, ResourceCost cost, Action upgradeAction) //TODO Preset set of upgrade actions
		{
			Id = id;
			Cost = cost;
			UpgradeAction = upgradeAction;
		}

		public void Apply()
		{
			UpgradeAction();
			Bought = true;
		}
	}
}