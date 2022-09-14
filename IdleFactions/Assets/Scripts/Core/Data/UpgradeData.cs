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
			_upgrades.Add(FactionType.Creation, new[]
			{
				new Upgrade("More essence",
					new ResourceCost(ResourceType.Infinity, 5), false,
					new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Essence, 2)),
			});
			_upgrades.Add(FactionType.Divinity, new[]
			{
				new Upgrade("Unlock the light", new ResourceCost(ResourceType.Essence), true,
					upgradeActions: new UpgradeActionUnlock()),

				new Upgrade("More light",
					new ResourceCost(ResourceType.Light, 30), false,
					new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Light, 2)),
				new Upgrade("More dark consumption, more light",
					new ResourceCost(ResourceType.Light, 100), false,
					new UpgradeActionMultiplier(FactionResourceType.LiveCost, ResourceType.Dark, 4),
					new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Light, 2))
			});

			_upgrades.Add(FactionType.Void, new[]
			{
				new Upgrade("Unlock the void", new ResourceCost(ResourceType.Essence), true, new UpgradeActionUnlock()),

				new Upgrade("More dark",
					new ResourceCost(ResourceType.Light, 30), false,
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Dark, 2)),
				new Upgrade("More light consumption, more dark",
					new ResourceCost(ResourceType.Dark, 100), false,
					new UpgradeActionMultiplier(FactionResourceType.LiveCost, ResourceType.Light, 4),
					new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Dark, 2)
				)
			});

			_upgrades.Add(FactionType.Heat, new[]
			{
				new Upgrade("Unlock lava",
					new[] { new ResourceCost(ResourceType.Light, 100), new ResourceCost(ResourceType.Dark, 100) }, true,
					new UpgradeActionUnlock()
				),
			});

			_upgrades.Add(FactionType.Nature, new[]
			{
				new Upgrade("More food", new ResourceCost(ResourceType.Light, 100d),
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Food, 1.5)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Light, 100d),
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.CreateCost, ResourceType.Food, 0.9))
			});

			_upgrades.Add(FactionType.Skeleton, new[]
			{
				new Upgrade("Unlock skeleton faction",
					new[]
					{
						new ResourceCost(ResourceType.Dark, 1000), new ResourceCost(ResourceType.Magic, 100),
						new ResourceCost(ResourceType.Bones, 100)
					},
					upgradeActions: new UpgradeActionUnlock()),

				new Upgrade("More magic efficient", new ResourceCost(ResourceType.Magic, 200d),
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.LiveCost, ResourceType.Magic, 0.95)),
				new Upgrade("More dark efficient", new ResourceCost(ResourceType.Magic, 200d),
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.LiveCost, ResourceType.Dark, 0.95)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Magic, 150d),
					upgradeActions: new UpgradeActionGeneralMultiplier(FactionResourceType.CreateCost, 0.95)),
				new Upgrade("Add food generation", new ResourceCost(ResourceType.Magic, 500d), false,
					new UpgradeActionNewResource(FactionResourceType.GenerateAdded,
						new AddedResource(ResourceType.Food, 0.5, ResourceType.Wildlife)),
					new UpgradeActionNewResource(FactionResourceType.GenerateCostAdded, new FactionResource(ResourceType.Wildlife, 0.5))),
				new Upgrade("Add bones generation", new ResourceCost(ResourceType.Magic, 1000d),
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Bones, 2)),
			});
			_upgrades.Add(FactionType.Necro, new[]
			{
				new Upgrade("Unlock necro faction",
					new[] { new ResourceCost(ResourceType.Dark, 10000), new ResourceCost(ResourceType.Magic, 1000) }),

				new Upgrade("More food", new ResourceCost(ResourceType.Magic, 1000d),
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.Generate, ResourceType.Food, 1.5)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Magic, 1000d),
					upgradeActions: new UpgradeActionMultiplier(FactionResourceType.CreateCost, ResourceType.Food, 0.9))
			});
			_upgrades.Add(FactionType.Dwarf, new[]
			{
				new Upgrade("Unlock dwarf faction todo",
					new[] { new ResourceCost(ResourceType.Dark, 10000), new ResourceCost(ResourceType.Magic, 1000) }),

				new Upgrade("More food", new ResourceCost(ResourceType.Magic, 1000d), false,
					new UpgradeActionNewResource(FactionResourceType.GenerateAdded,
						new AddedResource(ResourceType.Metal, 0.4d, ResourceType.Ore)),
					new UpgradeActionNewResource(FactionResourceType.GenerateCostAdded, new FactionResource(ResourceType.Ore, 0.2d))
				)
			});

			_upgrades.Add(FactionType.Human, new[]
			{
				new Upgrade("Learn to fish", new ResourceCost(ResourceType.Light, 100),
					upgradeActions: new UpgradeActionNewResource(FactionResourceType.GenerateAdded,
						new AddedResource(ResourceType.Food, 0.5d))),
				new Upgrade("Learn to chop wood", new ResourceCost(ResourceType.Light, 100),
					upgradeActions: new UpgradeActionNewResource(FactionResourceType.GenerateAdded,
						new AddedResource(ResourceType.Wood, 0.5d)))
			});
		}
	}
}