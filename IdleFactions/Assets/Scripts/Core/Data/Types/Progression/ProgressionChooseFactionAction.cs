namespace IdleFactions
{
	public readonly struct ProgressionChooseFactionAction : IProgressionAction
	{
		public FactionType[] FactionTypes { get; }

		public ProgressionChooseFactionAction(params FactionType[] factionTypes) => FactionTypes = factionTypes;
	}
}