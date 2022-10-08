using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleFactions
{
	public class ProgressionData
	{
		private readonly IDictionary<ResourceType, Progression> _resourceProgressions;
		private readonly IDictionary<FactionType, Progression> _factionProgressions;
		private const double UpgradeProgressionUnlockCostMultiplier = 0.4;

		private readonly UpgradeData _upgradeData;

		public ProgressionData(UpgradeData upgradeData)
		{
			_upgradeData = upgradeData;
			_resourceProgressions = new Dictionary<ResourceType, Progression>();
			_factionProgressions = new Dictionary<FactionType, Progression>();

			SetupProgressions();
		}

		public IReadOnlyDictionary<ResourceType, Progression> GetResourceProgressions() =>
			_resourceProgressions.Select(x => new KeyValuePair<ResourceType, Progression>(x.Key, x.Value.ShallowClone()))
				.ToDictionary(x => x.Key, x => x.Value);

		public IReadOnlyDictionary<FactionType, Progression> GetFactionProgressions() =>
			_factionProgressions.Select(x => new KeyValuePair<FactionType, Progression>(x.Key, x.Value.ShallowClone()))
				.ToDictionary(x => x.Key, x => x.Value);

		private void SetupProgressions()
		{
			//TODO Right now only single resource based upgrades are supported.
			//If we want to support multiple resources, then we need to remove the action when it's completed
			//Maybe multi-key dictionary with two lookups?

			//FactionUnlocks still have to be manually set, same for faction choices
			var unlockFactionUpgrades = SetupUnlockFactionUpgradeProgressions();
			// All resource progression upgrades}
			SetupResourceUpgradeProgressions(unlockFactionUpgrades);

			_factionProgressions.Add(FactionType.Divinity, new Progression("Light population test", new IProgressionEntry[]
			{
				new ProgressionEntry(new ProgressionCondition.Faction(double.MaxValue), new ProgressionAction.TempUI()) //RELEASE Remove
			}));
		}

		private static Dictionary<ResourceType, IProgressionEntry[]> SetupUnlockFactionUpgradeProgressions()
		{
			return new Dictionary<ResourceType, IProgressionEntry[]>
			{
				{
					ResourceType.Essence, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionCondition.Resource(5),
							new ProgressionAction.DiscoverFaction(FactionType.Divinity))
					}
				},
				{
					ResourceType.Light, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionCondition.Resource(3),
							new ProgressionAction.DiscoverFaction(FactionType.Void)),
						new ProgressionEntry(
							new ProgressionCondition.Resource(5e3),
							new ProgressionAction.DiscoverFaction(FactionType.Heat)),
					}
				},
				{
					ResourceType.Dark, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionCondition.Resource(5e3),
							new ProgressionAction.DiscoverFaction(FactionType.Ocean))
					}
				},
				{
					ResourceType.Lava, new IProgressionEntry[]
					{
						//TODO Remove me
						new ProgressionEntry(
							new ProgressionCondition.Resource(150),
							new ProgressionAction.TempUI())
					}
				},
				{
					ResourceType.Water, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionCondition.Resource(1e3),
							new ProgressionAction.DiscoverFaction(FactionType.Golem)),
						new ProgressionEntry(
							new ProgressionCondition.Resource(1e12),
							new ProgressionAction.ChooseFaction(FactionType.Nature, FactionType.Treant))
					}
				},
				{
					ResourceType.Mana, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionCondition.Resource(200),
							new ProgressionAction.ChooseFaction(FactionType.Mage, FactionType.Warlock)),
					}
				},
				{
					ResourceType.Infinity, new IProgressionEntry[]
					{
						new ProgressionEntry(
							new ProgressionCondition.Resource(1),
							new ProgressionAction.ChooseFaction(FactionType.Dwarf, FactionType.Goblin)), //Ogres instead?
						new ProgressionEntry(
							new ProgressionCondition.Resource(1),
							new ProgressionAction.ChooseFaction(FactionType.Elf, FactionType.Elf)), //TODO
						new ProgressionEntry(
							new ProgressionCondition.Resource(1),
							new ProgressionAction.ChooseFaction(FactionType.Skeleton, FactionType.Human)),
						new ProgressionEntry(
							new ProgressionCondition.Resource(1),
							new ProgressionAction.ChooseFaction(FactionType.Necro, FactionType.Drowner)), //?
					}
				}
			};
		}

		private void SetupResourceUpgradeProgressions(IDictionary<ResourceType, IProgressionEntry[]> unlockFactionUpgrades)
		{
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
						new ProgressionCondition.Resource(upgrade.Costs[0].Value * UpgradeProgressionUnlockCostMultiplier),
						new ProgressionAction.UnlockUpgrade(upgrade.FactionType, upgrade.Id)));
				}

				var progression = new Progression(resourceType + "ResourceProgression",
					progressionEntries.OrderBy(e => e.Condition.OrderValue).ToArray());
				_resourceProgressions.Add(resourceType, progression);
			}
		}
	}
}