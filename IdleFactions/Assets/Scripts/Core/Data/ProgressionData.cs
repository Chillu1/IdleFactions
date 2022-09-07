using System.Collections.Generic;

namespace IdleFactions
{
	public class ProgressionData
	{
		private readonly IDictionary<ResourceType, Progression> _resourceProgressions;

		public ProgressionData()
		{
			_resourceProgressions = new Dictionary<ResourceType, Progression>();

			SetupProgressions();
		}

		public IDictionary<ResourceType, Progression> GetProgressions()
		{
			return _resourceProgressions;
		}

		private void SetupProgressions()
		{
			_resourceProgressions.Add(ResourceType.Essence, new Progression(
				new ProgressionEntry(
					new ProgressionResourceCondition(1), //BALANCE 10
					new ProgressionDiscoverFactionAction(FactionType.Divinity))
			));
			_resourceProgressions.Add(ResourceType.Light, new Progression(
				new ProgressionEntry(
					new ProgressionResourceCondition(2),
					new ProgressionDiscoverFactionAction(FactionType.Void)),
				new ProgressionEntry(
					new ProgressionResourceCondition(100),
					new ProgressionDiscoverUpgradeAction(FactionType.Divinity, "More light")),
				new ProgressionEntry(
					new ProgressionResourceCondition(500),
					new ProgressionDiscoverUpgradeAction(FactionType.Divinity, "More light 2"))
			));
		}
	}
}