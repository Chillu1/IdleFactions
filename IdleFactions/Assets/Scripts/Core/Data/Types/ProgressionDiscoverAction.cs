namespace IdleFactions
{
	public struct ProgressionDiscoverAction : IProgressionAction
	{
		public FactionType FactionType { get; }

		public ProgressionDiscoverAction(FactionType factionType)
		{
			FactionType = factionType;
		}
	}
}