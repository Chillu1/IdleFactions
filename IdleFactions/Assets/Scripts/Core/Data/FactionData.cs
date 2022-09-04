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
			return faction;
		}

		private void SetupFactions()
		{
			//Early game
			{
				var properties = new ResourceNeedsProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Essence));
				properties.SetCreateCost(new ResourceCost(ResourceType.Infinity));

				AddFaction(FactionType.Creation, properties);
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Light));
				properties.SetCreateCost(new ResourceCost(ResourceType.Essence));
				properties.SetLiveCost(new ResourceCost(ResourceType.Dark, 0.01d));

				AddFaction(FactionType.Divinity, properties);
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Dark));
				properties.SetCreateCost(new ResourceCost(ResourceType.Essence));
				properties.SetLiveCost(new ResourceCost(ResourceType.Dark, 0.01d));

				AddFaction(FactionType.Void, properties);
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Lava));
				properties.SetCreateCost(new[] { new ResourceCost(ResourceType.Light, 100d), new ResourceCost(ResourceType.Dark, 100d) });
				properties.SetGenerateCost(new ResourceCost(ResourceType.Light, 0.5d));

				AddFaction(FactionType.Heat, properties);
			}
			{
				var properties = new ResourceNeedsProperties();
				properties.SetGenerate(new ResourceCost(ResourceType.Water));
				properties.SetCreateCost(new[] { new ResourceCost(ResourceType.Light, 1000d), new ResourceCost(ResourceType.Dark, 1000d) });
				properties.SetGenerateCost(new[] { new ResourceCost(ResourceType.Light, 0.2d), new ResourceCost(ResourceType.Dark, 0.2d) });

				AddFaction(FactionType.Ocean, properties);
			}

			//Mid game
			{
				var properties = new ResourceNeedsProperties();
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

				AddFaction(FactionType.Nature, properties);
			}

			//Late game
			{
				var properties = new ResourceNeedsProperties();
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

				AddFaction(FactionType.Nature2, properties);
			}
			{
				var properties = new ResourceNeedsProperties();
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

				AddFaction(FactionType.Human, properties);
			}

			void AddFaction(FactionType type, ResourceNeedsProperties properties)
			{
				_factions.Add(type, new Faction(type, new ResourceNeeds(properties), _upgradeData.Get(type)));
			}
		}
	}
}