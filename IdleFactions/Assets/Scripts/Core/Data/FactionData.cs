using System.Collections.Generic;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class FactionData : IDataStore<FactionType, Faction>
	{
		private readonly Dictionary<FactionType, Faction> _factions;

		private readonly UpgradeData _upgradeData;

		public FactionData(UpgradeData upgradeData)
		{
			_upgradeData = upgradeData;

			_factions = new Dictionary<FactionType, Faction>();
			SetupFactions();
		}

		[CanBeNull]
		public Faction Get(FactionType type)
		{
			_factions.TryGetValue(type, out var faction);
			return faction?.DeepClone();
		}

		private void SetupFactions()
		{
			//Early game
			{
				var factionType = FactionType.Creation;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Essence));
				properties.SetCreateCost(new ResourceCost(ResourceType.Infinity));

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Divinity;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Light));
				properties.SetLiveCost(new ResourceCost(ResourceType.Dark, 0.01d));

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Void;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Dark));
				properties.SetLiveCost(new ResourceCost(ResourceType.Light, 0.01d));

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Heat;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Lava));
				properties.SetCreateCost(new[] { new ResourceCost(ResourceType.Light, 100d), new ResourceCost(ResourceType.Dark, 100d) });
				properties.SetGenerateCost(new ResourceCost(ResourceType.Light, 0.5d));

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Ocean;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Water));
				properties.SetCreateCost(new[] { new ResourceCost(ResourceType.Light, 1000d), new ResourceCost(ResourceType.Dark, 1000d) });
				properties.SetGenerateCost(new[] { new ResourceCost(ResourceType.Light, 0.2d), new ResourceCost(ResourceType.Dark, 0.2d) });

				AddFaction(factionType, properties);
			}

			//Mid game
			{
				var factionType = FactionType.Nature;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new[]
				{
					new ResourceCost(ResourceType.Plant, 2d),
					new ResourceCost(ResourceType.Food, 0.5d)
				});
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Water, 2d)
				});
				properties.SetLiveCost(new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Water, 2d)
				});

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Skeleton;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new[]
				{
					new ResourceCost(ResourceType.Bones, 1d)
				});
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Dark, 10d),
					new ResourceCost(ResourceType.Magic, 20d)
				});
				properties.SetLiveCost(new[]
				{
					new ResourceCost(ResourceType.Dark, 0.1d),
					new ResourceCost(ResourceType.Magic, 0.2d)
				});

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Necro;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new[]
				{
					new ResourceCost(ResourceType.Skeleton, 0.1d)
				});
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Dark, 100d),
					new ResourceCost(ResourceType.Magic, 200d)
				});
				properties.SetLiveCost(new[]
				{
					new ResourceCost(ResourceType.Dark, 0.2d),
					new ResourceCost(ResourceType.Magic, 0.4d)
				});
				properties.SetGenerateCost(new[]
				{
					new ResourceCost(ResourceType.Essence, 0.1d),
					new ResourceCost(ResourceType.Energy, 0.5d),
					new ResourceCost(ResourceType.Bones, 5d)
				});

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Demon;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new[]
				{
					new ResourceCost(ResourceType.Heat, 0.2d),
					new ResourceCost(ResourceType.Fire, 1d)
				});
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Dark, 1000d),
					new ResourceCost(ResourceType.Magic, 2000d),
					new ResourceCost(ResourceType.Fire, 1000d)
				});
				properties.SetLiveCost(new[]
				{
					new ResourceCost(ResourceType.Dark, 0.2d),
					new ResourceCost(ResourceType.Magic, 0.4d),
					new ResourceCost(ResourceType.Fire, 0.5d)
				});
				properties.SetGenerateCost(new[]
				{
					new ResourceCost(ResourceType.Heat, 0.1d)
				});

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Dwarf;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new[]
				{
					new ResourceCost(ResourceType.Stone, 0.6d),
					new ResourceCost(ResourceType.Ore, 0.2d),
					new ResourceCost(ResourceType.Gold, 0.1d),
					new ResourceCost(ResourceType.Bones, 0.01d),
				});
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light, 1000d),
					new ResourceCost(ResourceType.Dark, 1000d),
					new ResourceCost(ResourceType.Magic, 1000d),
					new ResourceCost(ResourceType.Stone, 1000d),
					new ResourceCost(ResourceType.Soul),
				});
				properties.SetLiveCost(new[]
				{
					new ResourceCost(ResourceType.Food, 0.8d),
					new ResourceCost(ResourceType.Light, 0.2d),
					new ResourceCost(ResourceType.Dark, 0.2d),
					new ResourceCost(ResourceType.Magic, 0.2d)
				});
				properties.SetGenerateCost(new[]
				{
					new ResourceCost(ResourceType.Stone, 0.2d),
					new ResourceCost(ResourceType.Wood, 0.1d),
					new ResourceCost(ResourceType.Metal, 0.1d),
				});
				//Upg: Smelting, lava, ore to gold/metal.

				AddFaction(factionType, properties);
			}

			//Late game
			{
				var factionType = FactionType.Nature2;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new[]
				{
					new ResourceCost(ResourceType.Wildlife),
					new ResourceCost(ResourceType.Food, 0.5d)
				});
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light, 1000),
					new ResourceCost(ResourceType.Water, 200)
				});
				properties.SetLiveCost(new[]
				{
					new ResourceCost(ResourceType.Light, 0.5d),
					new ResourceCost(ResourceType.Water, 0.2d)
				});

				AddFaction(factionType, properties);
			}
			{
				var factionType = FactionType.Human;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new[]
				{
					new ResourceCost(ResourceType.Stone, 0.1d)
				});
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light, 100),
					new ResourceCost(ResourceType.Nature, 10),
					new ResourceCost(ResourceType.Water, 5),
					new ResourceCost(ResourceType.Soul, 1)
				});
				properties.SetLiveCost(new[]
				{
					new ResourceCost(ResourceType.Food, 1d),
					new ResourceCost(ResourceType.Water, 1d),
					new ResourceCost(ResourceType.Nature, 0.1d)
				});
				properties.SetGenerateCost(new[]
				{
					new ResourceCost(ResourceType.Food, 1d),
					new ResourceCost(ResourceType.Water, 1d),
					new ResourceCost(ResourceType.Nature, 0.1d),
					new ResourceCost(ResourceType.Wildlife, 1d)
				});

				AddFaction(factionType, properties);
			}
		}

		private void AddFaction(FactionType type, FactionResourceProperties properties)
		{
			_factions.Add(type, new Faction(type, new FactionResources(properties), _upgradeData.Get(type)));
		}
	}
}