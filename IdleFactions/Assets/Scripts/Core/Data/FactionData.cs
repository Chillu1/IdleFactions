using System.Collections.Generic;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class FactionData
	{
		private readonly Dictionary<FactionType, Faction> _factions;

		public FactionData()
		{
			_factions = new Dictionary<FactionType, Faction>();
			SetupFactions();
		}

		[CanBeNull]
		public Faction Get(FactionType type)
		{
			_factions.TryGetValue(type, out var faction);
			return faction;
		}

		private void SetupFactions()
		{
			{
				var properties = new ResourceNeedsProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Light));
				properties.AddGenerateCost(new ResourceCost(ResourceType.Dark, 0.25d)); //TEMP
				//properties.AddLiveCost(new ResourceCost(ResourceType.Dark, 0.25d));

				var faction = AddFaction(new Faction(FactionType.Divinity, new ResourceNeeds(properties)));

				var upgrades = new List<Upgrade>();
				upgrades.Add(new Upgrade("More light", new ResourceCost(ResourceType.Light, 5),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)));
				upgrades.Add(new Upgrade("More light 2", new ResourceCost(ResourceType.Light, 10),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)));
				upgrades.Add(new Upgrade("More light 3", new ResourceCost(ResourceType.Light, 15),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2)));

				upgrades.Add(new Upgrade("More dark consumption, more light",
					new[] { new ResourceCost(ResourceType.Light, 100), new ResourceCost(ResourceType.Dark, 100) },
					() =>
					{
						faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.GenerateCost, ResourceType.Dark, 2);
						faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Light, 2);
					}));
				faction.SetupUpgrades(upgrades);
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Dark));
				properties.AddGenerateCost(new ResourceCost(ResourceType.Light, 0.25d)); //TEMP
				//properties.AddLiveCost(new ResourceCost(ResourceType.Light, 0.25d));

				var faction = AddFaction(new Faction(FactionType.Void, new ResourceNeeds(properties)));

				var upgrades = new List<Upgrade>();
				upgrades.Add(new Upgrade("More dark", new ResourceCost(ResourceType.Light, 5),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)));
				upgrades.Add(new Upgrade("More dark 2", new ResourceCost(ResourceType.Light, 10),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)));
				upgrades.Add(new Upgrade("More dark 3", new ResourceCost(ResourceType.Light, 15),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2)));

				upgrades.Add(new Upgrade("More light consumption, more dark",
					new[] { new ResourceCost(ResourceType.Dark, 100), new ResourceCost(ResourceType.Light, 100) },
					() =>
					{
						faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.GenerateCost, ResourceType.Light, 2);
						faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Dark, 2);
					}));
				faction.SetupUpgrades(upgrades);
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Water));
				properties.AddCreateCost(new ResourceCost(ResourceType.Light));

				AddFaction(new Faction(FactionType.Ocean, new ResourceNeeds(properties)));
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.AddGenerate(new[]
				{
					new ResourceCost(ResourceType.Plants, 2d),
					new ResourceCost(ResourceType.Food, 0.5d)
				});
				properties.AddCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Water, 2d)
				});
				properties.AddLiveCost(new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Water, 2d)
				});

				var faction = AddFaction(new Faction(FactionType.Nature, new ResourceNeeds(properties)));

				//Upgrade, TODO MOVE
				var upgrades = new List<Upgrade>();
				//ResourceCost, UpgradeEffectType, ref to faction?
				upgrades.Add(new Upgrade("More food", new ResourceCost(ResourceType.Light, 100d),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.Generate, ResourceType.Food, 1.5)));
				upgrades.Add(new Upgrade("Lower living cost", new ResourceCost(ResourceType.Light, 100d),
					() => faction.ResourceNeeds.ChangeMultiplier(ResourceNeedsType.CreateCost, ResourceType.Food, 0.9)));
				faction.SetupUpgrades(upgrades);
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Wildlife));
				properties.AddCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Water, 2d)
				});

				AddFaction(new Faction(FactionType.Nature2, new ResourceNeeds(properties)));
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.AddGenerate(new[]
				{
					new ResourceCost(ResourceType.Food, 2d),
					new ResourceCost(ResourceType.Wood)
				});
				properties.AddCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Nature, 10d),
					new ResourceCost(ResourceType.Water, 5d)
				});
				properties.AddLiveCost(new[]
				{
					new ResourceCost(ResourceType.Food),
					new ResourceCost(ResourceType.Water)
				});
				properties.AddGenerateCost(new[]
				{
					new ResourceCost(ResourceType.Nature),
					new ResourceCost(ResourceType.Wildlife)
				});

				AddFaction(new Faction(FactionType.Human, new ResourceNeeds(properties)));
			}
		}

		private Faction AddFaction(Faction faction)
		{
			_factions.Add(faction.Type, faction);
			return faction;
		}
	}
}