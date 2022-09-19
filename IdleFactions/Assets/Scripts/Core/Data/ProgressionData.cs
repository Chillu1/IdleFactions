using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleFactions
{
	public class ProgressionData
	{
		private readonly IDictionary<ResourceType, Progression> _resourceProgressions;
		private const double UpgradeProgressionUnlockCostMultiplier = 0.4;

		private readonly UpgradeData _upgradeData;

		public ProgressionData(UpgradeData upgradeData)
		{
			_upgradeData = upgradeData;
			_resourceProgressions = new Dictionary<ResourceType, Progression>();

			SetupProgressions();
		}

		public IReadOnlyDictionary<ResourceType, Progression> GetResourceProgressions()
		{
			return _resourceProgressions.Select(x => new KeyValuePair<ResourceType, Progression>(x.Key, x.Value.ShallowClone()))
				.ToDictionary(x => x.Key, x => x.Value);
		}

		private void SetupProgressions()
		{
			//TODO Right now only single resource based upgrades are supported.
			//If we want to support multiple resources, then we need to remove the action when it's completed
			//Maybe multi-key dictionary with two lookups?

			//FactionUnlocks still have to be manually set
			var unlockFactionUpgrades = new Dictionary<ResourceType, IProgressionEntry[]>
			{
				{
					ResourceType.Essence, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionResourceCondition(5),
							new ProgressionDiscoverFactionAction(FactionType.Divinity))
					}
				},
				{
					ResourceType.Light, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionResourceCondition(3),
							new ProgressionDiscoverFactionAction(FactionType.Void)),
						new ProgressionEntry(
							new ProgressionResourceCondition(5e3),
							new ProgressionDiscoverFactionAction(FactionType.Heat))
					}
				},
				{
					ResourceType.Dark, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionResourceCondition(5e3),
							new ProgressionDiscoverFactionAction(FactionType.Ocean))
					}
				},
				{
					ResourceType.Lava, new IProgressionEntry[]
					{
						//TODO Remove me
						new ProgressionEntry(
							new ProgressionResourceCondition(150),
							new TempUIAction())
					}
				}
			};

			// All resource progression upgrades
			var allProgressionUpgrades = _upgradeData.GetAllProgressionUpgrades();
			foreach (var resourceType in ResourceTypeHelper.ResourceTypes)
			{
				var progressionEntries = new List<IProgressionEntry>();

				if (unlockFactionUpgrades.ContainsKey(resourceType))
					progressionEntries.AddRange(unlockFactionUpgrades[resourceType]);

				var resourceProgressions = allProgressionUpgrades.Where(u => u.Costs[0].Type == resourceType).ToArray();
				if (resourceProgressions.Length == 0)
					continue;

				foreach (var upgrade in resourceProgressions)
				{
					//Skip unlock upgrades, they're always unlocked
					if (upgrade.Id.Contains("unlock", StringComparison.InvariantCultureIgnoreCase))
						continue;

					progressionEntries.Add(new ProgressionEntry(
						new ProgressionResourceCondition(upgrade.Costs[0].Value * UpgradeProgressionUnlockCostMultiplier),
						new ProgressionDiscoverUpgradeAction(upgrade.FactionType, upgrade.Id)));
				}

				var progression = new Progression(resourceType + "ResourceProgression",
					progressionEntries.OrderBy(e => e.Condition.OrderValue).ToArray());
				_resourceProgressions.Add(resourceType, progression);
			}
		}
	}
}