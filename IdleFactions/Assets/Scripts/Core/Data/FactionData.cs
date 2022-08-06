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
			if (!_factions.ContainsKey(type))
				return null;

			return _factions[type];
		}

		private void SetupFactions()
		{
			{
				var properties = new ResourceNeedProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Light));

				AddFaction(new Faction(FactionType.Divinity, new ResourceNeeds(properties)));
			}
			{
				var properties = new ResourceNeedProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Dark));

				AddFaction(new Faction(FactionType.Void, new ResourceNeeds(properties)));
			}
			{
				var properties = new ResourceNeedProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Water));
				properties.AddCreateCost(new ResourceCost(ResourceType.Light));

				AddFaction(new Faction(FactionType.Ocean, new ResourceNeeds(properties)));
			}
			{
				var properties = new ResourceNeedProperties();
				properties.AddGenerate(new []
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

				AddFaction(new Faction(FactionType.Nature, new ResourceNeeds(properties)));
			}
			{
				var properties = new ResourceNeedProperties();
				properties.AddGenerate(new ResourceCost(ResourceType.Wildlife));
				properties.AddCreateCost(new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Water, 2d)
				});

				AddFaction(new Faction(FactionType.Nature2, new ResourceNeeds(properties)));
			}
			{
				var properties = new ResourceNeedProperties();
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

		private void AddFaction(Faction faction)
		{
			_factions.Add(faction.Type, faction);
		}
	}
}