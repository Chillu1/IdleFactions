namespace IdleFactions
{
	public static partial class PrestigeUpgradeAction
	{
		public readonly struct UnlockUpgrade : IPrestigeUpgradeAction
		{
			public string UpgradeId { get; }

			public UnlockUpgrade(string upgradeId)
			{
				UpgradeId = upgradeId;
			}

			public override string ToString()
			{
				return $"Unlock upgrade: {UpgradeId}";
			}
		}
	}
}