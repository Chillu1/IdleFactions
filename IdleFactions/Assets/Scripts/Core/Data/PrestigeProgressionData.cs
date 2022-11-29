using System.Collections.Generic;

namespace IdleFactions
{
	public sealed class PrestigeProgressionData
	{
		private readonly IProgressionEntry[] _prestigeProgressions;

		public PrestigeProgressionData()
		{
			_prestigeProgressions = new IProgressionEntry[]
			{
				new ProgressionEntry(
					new ProgressionCondition.Prestige(1),
					new ProgressionAction.DiscoverFaction(FactionType.Golem)),
				new ProgressionEntry(
					new ProgressionCondition.Prestige(2),
					new ProgressionAction.ChooseFaction(FactionType.Nature, FactionType.Treant)),
			};
		}

		public IEnumerable<IProgressionEntry> GetPrestigeProgressions()
		{
			//No need to clone, it only contains conditions and actions
			return _prestigeProgressions;
		}
	}
}