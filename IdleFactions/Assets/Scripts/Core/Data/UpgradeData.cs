using System.Collections.Generic;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class UpgradeData : IDataStore<FactionType, IReadOnlyList<Upgrade>>
	{
		private readonly Dictionary<FactionType, IReadOnlyList<Upgrade>> _upgrades;

		public UpgradeData()
		{
			_upgrades = new Dictionary<FactionType, IReadOnlyList<Upgrade>>();

			SetupUpgrades();
		}

		[CanBeNull]
		public IReadOnlyList<Upgrade> Get(FactionType factionType)
		{
			_upgrades.TryGetValue(factionType, out var upgrades);
			return upgrades;
		}

		private void SetupUpgrades()
		{
			_upgrades.Add(FactionType.Creation, new List<Upgrade>
			{
				new Upgrade("More essence",
					new ResourceCost(ResourceType.Infinity, 5), false,
					new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Essence, 2)),
			});
			_upgrades.Add(FactionType.Divinity, new List<Upgrade>
			{
				new Upgrade("Unlock the light", new ResourceCost(ResourceType.Essence), true,
					upgradeActions: new UpgradeActionUnlock()),

				new Upgrade("More light",
					new ResourceCost(ResourceType.Light, 5), false,
					new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)),
				new Upgrade("More light 2",
					new ResourceCost(ResourceType.Light, 10),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)),
				new Upgrade("More light 3",
					new ResourceCost(ResourceType.Light, 15),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)),
				new Upgrade("More dark consumption, more light",
					new ResourceCost(ResourceType.Light, 15), false,
					new UpgradeActionMultiplier(ResourceNeedsType.LiveCost, ResourceType.Dark, 2),
					new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2))
			});

			_upgrades.Add(FactionType.Void, new List<Upgrade>
			{
				new Upgrade("Unlock the void", new ResourceCost(ResourceType.Essence), true, new UpgradeActionUnlock()),

				new Upgrade("More dark",
					new ResourceCost(ResourceType.Light, 5),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)),
				new Upgrade("More dark 2",
					new ResourceCost(ResourceType.Light, 10),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)),
				new Upgrade("More dark 3",
					new ResourceCost(ResourceType.Light, 15),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)),
				new Upgrade("More light consumption, more dark",
					new ResourceCost(ResourceType.Dark, 100), false,
					new UpgradeActionMultiplier(ResourceNeedsType.LiveCost, ResourceType.Light, 2),
					new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)
				)
			});

			_upgrades.Add(FactionType.Heat, new List<Upgrade>
			{
				new Upgrade("Unlock lava",
					new[] { new ResourceCost(ResourceType.Light, 100), new ResourceCost(ResourceType.Dark, 100) }, true,
					new UpgradeActionUnlock()
				),
			});

			_upgrades.Add(FactionType.Nature, new List<Upgrade>
			{
				new Upgrade("More food", new ResourceCost(ResourceType.Light, 100d),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Food, 1.5)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Light, 100d),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.CreateCost, ResourceType.Food, 0.9))
			});

			_upgrades.Add(FactionType.Skeleton, new List<Upgrade>
			{
				new Upgrade("Unlock skeleton faction",
					new[]
					{
						new ResourceCost(ResourceType.Dark, 1000), new ResourceCost(ResourceType.Magic, 100),
						new ResourceCost(ResourceType.Bones, 100)
					},
					upgradeActions: new UpgradeActionUnlock()),

				new Upgrade("More magic efficient", new ResourceCost(ResourceType.Magic, 200d),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.LiveCost, ResourceType.Magic, 0.95)),
				new Upgrade("More dark efficient", new ResourceCost(ResourceType.Magic, 200d),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.LiveCost, ResourceType.Dark, 0.95)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Magic, 150d),
					upgradeActions: new UpgradeActionGeneralMultiplier(ResourceNeedsType.CreateCost, 0.95)),
				new Upgrade("Add food generation", new ResourceCost(ResourceType.Magic, 500d), false,
					new UpgradeActionNewResource(ResourceNeedsType.GenerateAdded,
						new ResourceAdded(ResourceType.Food, 0.5, ResourceType.Wildlife)),
					new UpgradeActionNewResource(ResourceNeedsType.GenerateCostAdded, new Resource(ResourceType.Wildlife, 0.5))),
				new Upgrade("Add bones generation", new ResourceCost(ResourceType.Magic, 1000d),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Bones, 2)),
			});
			_upgrades.Add(FactionType.Necro, new List<Upgrade>
			{
				new Upgrade("Unlock necro faction",
					new[] { new ResourceCost(ResourceType.Dark, 10000), new ResourceCost(ResourceType.Magic, 1000) }),

				new Upgrade("More food", new ResourceCost(ResourceType.Magic, 1000d),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Food, 1.5)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Magic, 1000d),
					upgradeActions: new UpgradeActionMultiplier(ResourceNeedsType.CreateCost, ResourceType.Food, 0.9))
			});
			_upgrades.Add(FactionType.Dwarf, new List<Upgrade>
			{
				new Upgrade("Unlock dwarf faction todo",
					new[] { new ResourceCost(ResourceType.Dark, 10000), new ResourceCost(ResourceType.Magic, 1000) }),

				new Upgrade("More food", new ResourceCost(ResourceType.Magic, 1000d), false,
					new UpgradeActionNewResource(ResourceNeedsType.GenerateAdded,
						new ResourceAdded(ResourceType.Metal, 0.4d, ResourceType.Ore)),
					new UpgradeActionNewResource(ResourceNeedsType.GenerateCostAdded, new Resource(ResourceType.Ore, 0.2d))
				)
			});

			_upgrades.Add(FactionType.Human, new List<Upgrade>
			{
				new Upgrade("Learn to fish", new ResourceCost(ResourceType.Light, 100),
					upgradeActions: new UpgradeActionNewResource(ResourceNeedsType.GenerateAdded,
						new ResourceAdded(ResourceType.Food, 0.5d))),
				new Upgrade("Learn to chop wood", new ResourceCost(ResourceType.Light, 100),
					upgradeActions: new UpgradeActionNewResource(ResourceNeedsType.GenerateAdded,
						new ResourceAdded(ResourceType.Wood, 0.5d)))
			});
		}
	}
}