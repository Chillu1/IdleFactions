namespace IdleFactions
{
	public sealed class UnlockUpgrade : Upgrade
	{
		public UnlockUpgrade(string id, ResourceCost cost) : this(id, new[] { cost })
		{
		}

		public UnlockUpgrade(string id, params ResourceCost[] costs) : base(id, costs, new UpgradeActionUnlock())
		{
			IsUnlocked = true;
		}
	}
}