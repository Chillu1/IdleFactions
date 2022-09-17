using System.Collections.Generic;
using System.Linq;

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

		public IDictionary<ResourceType, Progression> GetResourceProgressions()
		{
			return _resourceProgressions.Select(x => new KeyValuePair<ResourceType, Progression>(x.Key, x.Value.ShallowClone()))
				.ToDictionary(x => x.Key, x => x.Value);
		}

		private void SetupProgressions()
		{
			_resourceProgressions.Add(ResourceType.Essence, new Progression("DivinityResourceProgression",
				new ProgressionEntry(
					new ProgressionResourceCondition(5),
					new ProgressionDiscoverFactionAction(FactionType.Divinity))
			));
			_resourceProgressions.Add(ResourceType.Light, new Progression("LightResourceProgression",
				new ProgressionEntry(
					new ProgressionResourceCondition(5),
					new ProgressionDiscoverFactionAction(FactionType.Void)),
				new ProgressionEntry(
					new ProgressionResourceCondition(20),
					new ProgressionDiscoverUpgradeAction(FactionType.Divinity, "More light")),
				new ProgressionEntry(
					new ProgressionResourceCondition(50),
					new ProgressionDiscoverUpgradeAction(FactionType.Divinity, "More dark consumption, more light")),
				new ProgressionEntry(
					new ProgressionResourceCondition(1000),
					new ProgressionDiscoverUpgradeAction(FactionType.Divinity, "More light 2")),
				new ProgressionEntry(
					new ProgressionResourceCondition(5e3),
					new ProgressionDiscoverFactionAction(FactionType.Heat))
			));
			_resourceProgressions.Add(ResourceType.Dark, new Progression("DarkResourceProgression",
				new ProgressionEntry(
					new ProgressionResourceCondition(20),
					new ProgressionDiscoverUpgradeAction(FactionType.Void, "More dark")),
				new ProgressionEntry(
					new ProgressionResourceCondition(50),
					new ProgressionDiscoverUpgradeAction(FactionType.Void, "More light consumption, more dark")),
				new ProgressionEntry(
					new ProgressionResourceCondition(1000),
					new ProgressionDiscoverUpgradeAction(FactionType.Void, "More dark 2")),
				new ProgressionEntry(
					new ProgressionResourceCondition(50e3),
					new ProgressionDiscoverFactionAction(FactionType.Ocean))
			));
		}
	}
}