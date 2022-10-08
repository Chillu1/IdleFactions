namespace IdleFactions
{
	public static partial class ProgressionAction
	{
		public readonly struct UnlockUpgrade : IProgressionAction
		{
			public FactionType FactionType { get; }
			public string UpgradeId { get; }

			public UnlockUpgrade(FactionType factionType, string upgradeId)
			{
				FactionType = factionType;
				UpgradeId = upgradeId;
			}
		}
	}
}