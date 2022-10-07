namespace IdleFactions
{
	public static partial class UpgradeAction
	{
		//TODO Either unlock factions with upgrades, or with events that trigger on X. Or both...
		//TODO Is a faction supposed to unlock another one with upgrades?
		public readonly struct UnlockFaction : IUpgradeAction
		{
			/*public FactionType FactionType { get; }
	
			private static FactionController _factionController;
	
			public UpgradeActionUnlock(FactionType factionType)
			{
				FactionType = factionType;
			}
	
			public static void Setup(FactionController factionController)
			{
				_factionController = factionController;
			}
	
			public void UnlockFaction()
			{
				_factionController.GetFaction(FactionType)?.Unlock();
			}*/

			public override string ToString()
			{
				return "Unlock Faction";
			}
		}
	}
}