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
			{
				var upgradeList = new List<Upgrade>
				{
					new Upgrade("More light",
						new ResourceCost(ResourceType.Light, 5),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)),
					new Upgrade("More light 2",
						new ResourceCost(ResourceType.Light, 10),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)),
					new Upgrade("More light 3",
						new ResourceCost(ResourceType.Light, 15),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)),
					new Upgrade("More dark consumption, more light",
						new ResourceCost(ResourceType.Light, 15),
						new UpgradeActionMultiplier(ResourceNeedsType.LiveCost, ResourceType.Dark, 2),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2))
				};

				_upgrades.Add(FactionType.Divinity, upgradeList);
			}

			{
				var upgradeList = new List<Upgrade>
				{
					new Upgrade("More dark",
						new ResourceCost(ResourceType.Light, 5),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)),
					new Upgrade("More dark 2",
						new ResourceCost(ResourceType.Light, 10),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)),
					new Upgrade("More dark 3",
						new ResourceCost(ResourceType.Light, 15),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)),
					new Upgrade("More light consumption, more dark",
						new ResourceCost(ResourceType.Dark, 100),
						new UpgradeActionMultiplier(ResourceNeedsType.LiveCost, ResourceType.Light, 2),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)
					)
				};

				_upgrades.Add(FactionType.Void, upgradeList);
			}

			{
				var upgradeList = new List<Upgrade>
				{
					new Upgrade("More food", new ResourceCost(ResourceType.Light, 100d),
						new UpgradeActionMultiplier(ResourceNeedsType.Generate, ResourceType.Food, 1.5)),
					new Upgrade("Lower living cost", new ResourceCost(ResourceType.Light, 100d),
						new UpgradeActionMultiplier(ResourceNeedsType.CreateCost, ResourceType.Food, 0.9))
				};

				_upgrades.Add(FactionType.Nature, upgradeList);
			}

			{
				var upgradeList = new List<Upgrade>
				{
					new Upgrade("Human fishers", new ResourceCost(ResourceType.Light), new UpgradeActionNewResource
						(ResourceNeedsType.Generate, ResourceType.Food, 0.5d))
				};

				_upgrades.Add(FactionType.Human, upgradeList);
			}
		}
	}
}