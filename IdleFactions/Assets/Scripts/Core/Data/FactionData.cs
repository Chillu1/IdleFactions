using System.Collections.Generic;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class FactionData : IDataStore<FactionType, Faction>
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
			return faction?.DeepClone();
		}

		private void SetupFactions()
		{
			//Early game
			{
				var factionType = FactionType.Creation;
				string description = "Foundation of the universe";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Essence));
				properties.SetCreateCost(new ResourceCost(ResourceType.Infinity));

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Divinity;
				string description = "Empty";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Light));
				properties.SetLiveCost(new ResourceCost(ResourceType.Dark, 0.01d)); //Buff this?

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Void;
				string description = "Empty";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Dark));
				properties.SetLiveCost(new ResourceCost(ResourceType.Light, 0.01d)); //Buff this?

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Heat;
				string description = "Empty";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Lava));
				properties.SetCreateCost(new ResourceCost(ResourceType.Light, 10e3));
				properties.SetLiveCost(new ResourceCost(ResourceType.Lava, 0.5d));

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Ocean;
				string description = "Empty";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Water));
				properties.SetCreateCost(new ResourceCost(ResourceType.Light, 100e3), new ResourceCost(ResourceType.Dark, 100e3));
				properties.SetGenerateCost(new ResourceCost(ResourceType.Light, 10e3), new ResourceCost(ResourceType.Dark, 10e3));

				AddFaction(factionType, description, properties);
			}

			//Mid game
			{
				var factionType = FactionType.Golem;
				string description = "Stone creatures";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Stone));
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Dark, 1e6),
					new ResourceCost(ResourceType.Heat, 1e3),
					new ResourceCost(ResourceType.Water, 1e3)
				);
				properties.SetLiveCost(new ResourceCost(ResourceType.Dark, 10e3));
				properties.SetGenerateCost(new ResourceCost(ResourceType.Heat, 10), new ResourceCost(ResourceType.Water, 10));

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Nature;
				string description = "Massive ecosystems, diverse pool of carbon based resources";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Plant),
					new ResourceCost(ResourceType.Wildlife, 0.5d),
					new ResourceCost(ResourceType.Food, 0.1d));
				properties.SetCreateCost(new ResourceCost(ResourceType.Light, 1e6), new ResourceCost(ResourceType.Water, 1e3));
				properties.SetLiveCost(new ResourceCost(ResourceType.Light, 100e3), new ResourceCost(ResourceType.Water, 100),
					new ResourceCost(ResourceType.Plant, 1.5d), new ResourceCost(ResourceType.Wildlife, 0.8d));

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Treant;
				string description = "Masters of wood, and some plants";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Wood));
				properties.SetCreateCost(new ResourceCost(ResourceType.Light, 10e6), new ResourceCost(ResourceType.Water, 10e3));
				properties.SetLiveCost(new ResourceCost(ResourceType.Light, 1e6), new ResourceCost(ResourceType.Water, 1e3));
				properties.SetGenerateCost(new ResourceCost(ResourceType.Light, 1e6), new ResourceCost(ResourceType.Water, 1e3));
				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Skeleton;
				string description = "Mindless workers, bone experts, need magic to survive";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Bones, 1d));
				properties.SetCreateCost(new ResourceCost(ResourceType.Dark, 10d), new ResourceCost(ResourceType.Magic, 20d));
				properties.SetLiveCost(new ResourceCost(ResourceType.Dark, 0.1d), new ResourceCost(ResourceType.Magic, 0.2d));

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Necro;
				string description = "Skeleton masters";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Skeleton, 0.1d));
				properties.SetCreateCost(new ResourceCost(ResourceType.Dark, 100d), new ResourceCost(ResourceType.Magic, 200d));
				properties.SetLiveCost(new ResourceCost(ResourceType.Dark, 0.2d), new ResourceCost(ResourceType.Magic, 0.4d));
				properties.SetGenerateCost(new ResourceCost(ResourceType.Essence, 0.1d), new ResourceCost(ResourceType.Energy, 0.5d),
					new ResourceCost(ResourceType.Bones, 5d));

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Demon;
				string description = "Live in fire, darkness and magic";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Heat, 0.2d), new ResourceCost(ResourceType.Fire, 1d));
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Dark, 1000d),
					new ResourceCost(ResourceType.Magic, 2000d),
					new ResourceCost(ResourceType.Fire, 1000d));
				properties.SetLiveCost(
					new ResourceCost(ResourceType.Dark, 0.2d),
					new ResourceCost(ResourceType.Magic, 0.4d),
					new ResourceCost(ResourceType.Fire, 0.5d));
				properties.SetGenerateCost(new ResourceCost(ResourceType.Heat, 0.1d));

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Dwarf;
				string description = "Masters of mountains, keep treasures, and are blacksmiths";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Stone, 0.1d),
					new ResourceCost(ResourceType.Ore, 0.02d),
					new ResourceCost(ResourceType.Gold, 0.002d),
					new ResourceCost(ResourceType.Bones, 0.01d)
				);
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Light, 1e12),
					new ResourceCost(ResourceType.Dark, 1e12),
					new ResourceCost(ResourceType.Stone, 5e6),
					new ResourceCost(ResourceType.Wood, 5e3)
				);
				properties.SetLiveCost(
					new ResourceCost(ResourceType.Light, 100e9),
					new ResourceCost(ResourceType.Dark, 100e9),
					new ResourceCost(ResourceType.Food, 2e3)
				);
				properties.SetGenerateCost(
					new ResourceCost(ResourceType.Stone, 100e3),
					new ResourceCost(ResourceType.Wood, 1e3),
					new ResourceCost(ResourceType.Metal, 1e3) //?
				);
				//Upg: Smelting, lava, ore to gold/metal.

				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Goblin;
				string description = "Greedy mountain race, different ethics from dwarfs though. But both love treasures";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Stone, 0.05d),
					new ResourceCost(ResourceType.Ore, 0.01d),
					new ResourceCost(ResourceType.Gold, 0.0005d),
					new ResourceCost(ResourceType.Bones, 0.01d)
				);
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Dark, 1e12),
					new ResourceCost(ResourceType.Stone, 1e6),
					new ResourceCost(ResourceType.Wood, 1e3)
				);
				properties.SetLiveCost(
					new ResourceCost(ResourceType.Dark, 100e9),
					new ResourceCost(ResourceType.Food, 1e3)
				);
				properties.SetGenerateCost(
					new ResourceCost(ResourceType.Stone, 50e3),
					new ResourceCost(ResourceType.Wood, 2e3),
					new ResourceCost(ResourceType.Metal, 1e3) //?
				);
				AddFaction(factionType, description, properties);
			}

			//Late game
			/*{
				var factionType = FactionType.Nature2;
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Wildlife),
					new ResourceCost(ResourceType.Food, 0.5d)
				);
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Light, 1000),
					new ResourceCost(ResourceType.Water, 200)
				);
				properties.SetLiveCost(
					new ResourceCost(ResourceType.Light, 0.5d),
					new ResourceCost(ResourceType.Water, 0.2d)
				);

				AddFaction(factionType, properties);
			}*/
			{
				var factionType = FactionType.Human;
				string description = "Can do anything... Human. Massive worldy needs";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Food, 0.5d)
				);
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Light, 100),
					new ResourceCost(ResourceType.Plant, 10),
					new ResourceCost(ResourceType.Water, 5)
				);
				properties.SetLiveCost(
					new ResourceCost(ResourceType.Food, 1d),
					new ResourceCost(ResourceType.Water, 1d),
					new ResourceCost(ResourceType.Plant, 0.1d)
				);
				properties.SetGenerateCost(
					new ResourceCost(ResourceType.Food, 1d),
					new ResourceCost(ResourceType.Water, 1d),
					new ResourceCost(ResourceType.Plant, 0.1d),
					new ResourceCost(ResourceType.Wildlife, 1d)
				);

				AddFaction(factionType, description, properties);
			}

			//TEMP Resources
			{
				var factionType = FactionType.Ogre;
				string description = "Dumb swamp race, great for X though";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Infinity)
				);
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Infinity)
				);
				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Mage;
				string description = "Best magic creators";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Infinity)
				);
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Infinity),
				});
				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Warlock;
				string description = "Magic creators, not as good as mages, and use darkness. But excel at other things";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Infinity)
				);
				properties.SetCreateCost(new[]
				{
					new ResourceCost(ResourceType.Infinity),
				});
				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Drowner;
				string description = "Ocean creatures, use creatures to do their bidding";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Infinity)
				);
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Infinity)
				);
				AddFaction(factionType, description, properties);
			}
			{
				var factionType = FactionType.Elf;
				string description = "Forest creatures";
				var properties = new FactionResourceProperties();
				properties.SetGenerate(
					new ResourceCost(ResourceType.Infinity)
				);
				properties.SetCreateCost(
					new ResourceCost(ResourceType.Infinity)
				);
				AddFaction(factionType, description, properties);
			}
		}

		private void AddFaction(FactionType type, string description, FactionResourceProperties properties)
		{
			bool isSoulBased = FactionTypeHelper.SoulFactionTypes.Contains(type);
			_factions.Add(type, new Faction(type, description, new FactionResources(properties, isSoulBased)));
		}
	}
}