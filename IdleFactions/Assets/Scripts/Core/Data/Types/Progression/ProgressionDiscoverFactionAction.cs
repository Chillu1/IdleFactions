namespace IdleFactions
{
	public readonly struct ProgressionDiscoverFactionAction : IProgressionAction
	{
		public FactionType FactionType { get; }

		public ProgressionDiscoverFactionAction(FactionType factionType) => FactionType = factionType;
	}
}