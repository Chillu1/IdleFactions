namespace IdleFactions
{
	public static partial class ProgressionAction
	{
		public readonly struct ChooseFaction : IProgressionAction
		{
			public FactionType[] FactionTypes { get; }

			public ChooseFaction(params FactionType[] factionTypes) => FactionTypes = factionTypes;
		}
	}
}