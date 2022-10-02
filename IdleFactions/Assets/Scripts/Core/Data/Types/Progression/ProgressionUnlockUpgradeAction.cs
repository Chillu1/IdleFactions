namespace IdleFactions
{
	public readonly struct ProgressionUnlockUpgradeAction : IProgressionAction
	{
		public FactionType FactionType { get; }
		public string UpgradeId { get; }

		public ProgressionUnlockUpgradeAction(FactionType factionType, string upgradeId)
		{
			FactionType = factionType;
			UpgradeId = upgradeId;
		}
	}
}