namespace IdleFactions
{
	public static partial class ProgressionAction
	{
		public readonly struct DiscoverFaction : IProgressionAction
		{
			public FactionType FactionType { get; }

			public DiscoverFaction(FactionType factionType) => FactionType = factionType;
		}
	}
}