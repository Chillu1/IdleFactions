using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class UpgradeData : IDataStore<FactionType, IReadOnlyList<IUpgrade>>
	{
		private readonly Dictionary<FactionType, IReadOnlyList<IUpgrade>> _upgrades;

		public UpgradeData()
		{
			_upgrades = new Dictionary<FactionType, IReadOnlyList<IUpgrade>>();

			SetupUpgrades();
		}

		[CanBeNull]
		public IReadOnlyList<IUpgrade> Get(FactionType factionType)
		{
			_upgrades.TryGetValue(factionType, out var upgrades);
			return upgrades;
		}

		public IReadOnlyList<IProgressionUpgrade> GetAllProgressionUpgrades()
		{
			return _upgrades.Values.SelectMany(readOnlyList => (IReadOnlyList<IProgressionUpgrade>)readOnlyList).ToList();
		}

		private void SetupUpgrades()
		{
			AddUpgrades(FactionType.Creation, new[]
			{
				new Upgrade("More essence",
					new ResourceCost(ResourceType.Infinity, 5),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Essence, 2)),
			});
			AddUpgrades(FactionType.Divinity, new[]
			{
				new UnlockUpgrade("Unlock the light", new ResourceCost(ResourceType.Essence)),

				new Upgrade("More light",
					new ResourceCost(ResourceType.Light, 30),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Light, 2)),
				new Upgrade("More dark consumption, more light",
					new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.Multiplier(FactionResourceType.LiveCost, ResourceType.Dark, 4),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Light, 2)),
				new Upgrade("More light 2",
					new ResourceCost(ResourceType.Light, 2000),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Light, 4)),
				new Upgrade("Heat up",
					new ResourceCost(ResourceType.Lava, 100),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Light, 2)),
			});

			AddUpgrades(FactionType.Void, new[]
			{
				new UnlockUpgrade("Unlock the void", new ResourceCost(ResourceType.Essence)),

				new Upgrade("More dark",
					new ResourceCost(ResourceType.Dark, 30),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Dark, 2)),
				new Upgrade("More light consumption, more dark",
					new ResourceCost(ResourceType.Dark, 100),
					new UpgradeAction.Multiplier(FactionResourceType.LiveCost, ResourceType.Light, 4),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Dark, 2)),
				new Upgrade("More dark 2",
					new ResourceCost(ResourceType.Dark, 2000),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Dark, 4))
			});

			AddUpgrades(FactionType.Heat, new[]
			{
				new UnlockUpgrade("Unlock lava", new ResourceCost(ResourceType.Light, 10e3)),

				new Upgrade("Lava burst",
					new[] { new ResourceCost(ResourceType.Lava, 50) },
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Lava, 2)),
				new Upgrade("Lava light",
					new[] { new ResourceCost(ResourceType.Light, 50e3) },
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Lava, 2)),
				new Upgrade("Lava light enchantment",
					new[] { new ResourceCost(ResourceType.Light, 500e3) },
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Lava, 2),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateCostAdded, new AddedResource(ResourceType.Light, 0.5d))),

				new Upgrade("Light generation",
					new[] { new ResourceCost(ResourceType.Lava, 1e3), new ResourceCost(ResourceType.Light, 1e6) },
					new UpgradeAction.Multiplier(FactionResourceType.GenerateCost, ResourceType.Lava, 2),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded, new AddedResource(ResourceType.Light, 1d))),
			});

			AddUpgrades(FactionType.Ocean, new[]
			{
				new UnlockUpgrade("Unlock ocean", new ResourceCost(ResourceType.Dark, 100e3)),

				new Upgrade("Darker sea",
					new[] { new ResourceCost(ResourceType.Dark, 500e3) },
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Water, 2),
					new UpgradeAction.Multiplier(FactionResourceType.GenerateCost, ResourceType.Dark, 4)),

				new Upgrade("Deep sea",
					new[] { new ResourceCost(ResourceType.Water, 1e3), new ResourceCost(ResourceType.Dark, 1e6) },
					new UpgradeAction.Multiplier(FactionResourceType.GenerateCost, ResourceType.Water, 2),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded, new AddedResource(ResourceType.Dark, 1d))),
			});

			AddUpgrades(FactionType.Nature, new[]
			{
				new UnlockUpgrade("Unlock nature", new ResourceCost(ResourceType.Light), new ResourceCost(ResourceType.Water)),

				new Upgrade("More food", new ResourceCost(ResourceType.Light, 100d),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Food, 1.5)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Light, 100d),
					new UpgradeAction.Multiplier(FactionResourceType.CreateCost, ResourceType.Food, 0.9)),

				//Healthier ecosystems
			});
			AddUpgrades(FactionType.Treant, new[]
			{
				new UnlockUpgrade("Unlock treants", new ResourceCost(ResourceType.Light), new ResourceCost(ResourceType.Water)),

				new Upgrade("More water absorption", new ResourceCost(ResourceType.Water, 1000d),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Wood, 2),
					new UpgradeAction.Multiplier(FactionResourceType.GenerateCost, ResourceType.Water, 4)),
				//Nature(plant)-treant synergy
			});

			AddUpgrades(FactionType.Skeleton, new[]
			{
				new UnlockUpgrade("Unlock skeleton faction",
					new ResourceCost(ResourceType.Dark, 1000), new ResourceCost(ResourceType.Magic, 100),
					new ResourceCost(ResourceType.Bones, 100)
				),

				new Upgrade("More magic efficient", new ResourceCost(ResourceType.Magic, 200d),
					new UpgradeAction.Multiplier(FactionResourceType.LiveCost, ResourceType.Magic, 0.95)),
				new Upgrade("More dark efficient", new ResourceCost(ResourceType.Magic, 200d),
					new UpgradeAction.Multiplier(FactionResourceType.LiveCost, ResourceType.Dark, 0.95)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Magic, 150d),
					new UpgradeAction.GeneralMultiplier(FactionResourceType.CreateCost, 0.95)),
				new Upgrade("Add food generation", new ResourceCost(ResourceType.Magic, 500d),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded,
						new AddedResource(ResourceType.Food, 0.5, ResourceType.Wildlife)),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateCostAdded,
						new FactionResource(ResourceType.Wildlife, 0.5))),
				new Upgrade("Add bones generation", new ResourceCost(ResourceType.Magic, 1000d),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Bones, 2)),
			});
			AddUpgrades(FactionType.Necro, new[]
			{
				new UnlockUpgrade("Unlock necro faction",
					new[] { new ResourceCost(ResourceType.Dark, 10000), new ResourceCost(ResourceType.Magic, 1000) }),

				new Upgrade("More food", new ResourceCost(ResourceType.Magic, 1000d),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Food, 1.5)),
				new Upgrade("Lower living cost", new ResourceCost(ResourceType.Magic, 1000d),
					new UpgradeAction.Multiplier(FactionResourceType.CreateCost, ResourceType.Food, 0.9))
			});
			AddUpgrades(FactionType.Dwarf, new[]
			{
				new UnlockUpgrade("Unlock dwarf faction todo",
					new[] { new ResourceCost(ResourceType.Dark, 10000), new ResourceCost(ResourceType.Magic, 1000) }),

				new Upgrade("More food", new ResourceCost(ResourceType.Magic, 1000d),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded,
						new AddedResource(ResourceType.Metal, 0.4d, ResourceType.Ore)),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateCostAdded, new FactionResource(ResourceType.Ore, 0.2d))
				),
				new Upgrade("Deeper mines",
					new[]
					{
						new ResourceCost(ResourceType.Light, 1000), new ResourceCost(ResourceType.Dark, 1000),
						new ResourceCost(ResourceType.Fire, 1000), new ResourceCost(ResourceType.Stone, 1000),
						new ResourceCost(ResourceType.Wood, 1000)
					},
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Stone, 2),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Ore, 2),
					new UpgradeAction.Multiplier(FactionResourceType.LiveCost, ResourceType.Light, 2),
					new UpgradeAction.Multiplier(FactionResourceType.LiveCost, ResourceType.Dark, 2),
					new UpgradeAction.Multiplier(FactionResourceType.LiveCost, ResourceType.Food, 2)
				),
				//Smelt ore to metal
				//Dwarfs keep gold, and work faster in general
				//Dwarfs keep more gold, ...
				//Dwarfs hunting, needs more wood & metal, worse than goblin hunting
				//Gold infused pickaxes, more stone, ore, gold, etc. Costs more gold per gen
				//Dwarf-Treant alliance. Dwarfs can plant trees in mountains, (unlocks upgrade for treant-less runs)
				//Dwarf-Golem alliance. Working together in mountain, much more stone. (unlocks stone upgrade for golem-less runs)
				//Dwarf-Elf alliance? 2X all generation, special upgrades unlocked?
				//Dwarf-Mage alliance? Magic infused pickaxes?
				//Dwarf-Human alliance. Dwarfs can live outside of mountains.
			});
			AddUpgrades(FactionType.Goblin, new[]
			{
				new UnlockUpgrade("Unlock goblin faction todo",
					new[] { new ResourceCost(ResourceType.Dark, 10000), new ResourceCost(ResourceType.Magic, 1000) }),

				new Upgrade("More food", new ResourceCost(ResourceType.Magic, 1000d),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded,
						new AddedResource(ResourceType.Metal, 0.4d, ResourceType.Ore)),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateCostAdded, new FactionResource(ResourceType.Ore, 0.2d))
				)
				//Deeper mountains, more stone, ore
				//Smelt ore to metal (less efficient than dwarfs)
				//Goblins keep gold, and work faster in general (more efficient than dwarfs)
				//Goblin hunting, needs more wood & metal, better than dwarf hunting
				//Starve goblins, less food needed, lower production
				//Sacrifice goblins (some other faction upgrade), gives food, dark, magic?, 
			});

			AddUpgrades(FactionType.Warlock, new[]
			{
				new UnlockUpgrade("Unlock warlock faction",
					new[] { new ResourceCost(ResourceType.Dark, 1e6), new ResourceCost(ResourceType.Magic, 1e3) }),
			});

			AddUpgrades(FactionType.Human, new[]
			{
				new Upgrade("Learn to fish", new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded,
						new AddedResource(ResourceType.Food, 0.5d, ResourceType.Wildlife)),
					new UpgradeAction.Multiplier(FactionResourceType.GenerateCost, ResourceType.Wildlife, 2d)
				),
				new Upgrade("Learn to chop wood", new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded, new AddedResource(ResourceType.Wood, 0.5d))),
				new Upgrade("Stone cutting", new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded, new AddedResource(ResourceType.Stone, 0.5d))
					//Wood & Metal?
				),
				new Upgrade("Agriculture", new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.Multiplier(FactionResourceType.Generate, ResourceType.Food, 4),
					new UpgradeAction.Multiplier(FactionResourceType.GenerateCostAdded, ResourceType.Plant, 4),
					new UpgradeAction.Multiplier(FactionResourceType.GenerateCostAdded, ResourceType.Water, 8)
				),
				new Upgrade("Woodcutting", new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded, new AddedResource(ResourceType.Wood, 0.5d))
					//Wood & Metal?
				),
				new Upgrade("Mining", new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded, new AddedResource(ResourceType.Ore, 0.5d)),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded, new AddedResource(ResourceType.Gold, 0.01d))
					//Wood & Metal?
				),
				//Industrial revolution and it's consequences
				new Upgrade("Electricity", new ResourceCost(ResourceType.Light, 100),
					new UpgradeAction.NewResource(FactionResourceAddedType.GenerateAdded,
						new AddedResource(ResourceType.Electricity, 0.5d))),
				//Dangerous upgrade: Human harvesting (souls, food, water, costs humans, obvs)
				//new Upgrade("Human harvesting", new ResourceCost(ResourceType.Light, 100),
				//	new UpgradeAction.NewResource(FactionResourceType.GenerateAdded, new AddedResource(ResourceType.Soul, 1d)),
				//	new UpgradeAction.NewResource(FactionResourceType.GenerateAdded, new AddedResource(ResourceType.Food, 0.5d)),
				//	new UpgradeAction.NewResource(FactionResourceType.GenerateAdded, new AddedResource(ResourceType.Water, 0.5d)),
				//	new UpgradeAction.NewResource(FactionResourceType.GenerateCostAdded, new AddedResource(FactionType.Human, 1d))),
			});
		}

		private void AddUpgrades(FactionType factionType, IReadOnlyList<IUpgrade> upgrades)
		{
			foreach (var upgrade in upgrades)
				upgrade.SetupFactionType(factionType);
			_upgrades.Add(factionType, upgrades);
		}
	}
}