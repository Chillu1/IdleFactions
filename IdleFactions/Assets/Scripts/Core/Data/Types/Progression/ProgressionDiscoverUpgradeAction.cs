namespace IdleFactions
{
	public readonly struct ProgressionDiscoverUpgradeAction : IProgressionAction
	{
		public FactionType FactionType { get; }
		public string UpgradeId { get; }

		public ProgressionDiscoverUpgradeAction(FactionType factionType, string upgradeId)
		{
			FactionType = factionType;
			UpgradeId = upgradeId;
		}
	}
}