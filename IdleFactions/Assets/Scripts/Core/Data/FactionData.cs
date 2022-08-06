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

			AddFaction(new Faction(FactionType.Light, new ResourceNeeds(
				generate: new ResourceCost(ResourceType.Light),
				createCost: null)));

			AddFaction(new Faction(FactionType.Water,
				new ResourceNeeds(
					generate: new ResourceCost(ResourceType.Water),
					createCost: new ResourceCost(ResourceType.Light))));

			AddFaction(new Faction(FactionType.Nature, new ResourceNeeds(
				generate: new[]
				{
					new ResourceCost(ResourceType.Nature)
				}, createCost: new[]
				{
					new ResourceCost(ResourceType.Light),
					new ResourceCost(ResourceType.Water, 2d)
				})));
		}

		[CanBeNull]
		public Faction Get(FactionType type)
		{
			if (!_factions.ContainsKey(type))
				return null;

			return _factions[type];
		}

		private void AddFaction(Faction faction)
		{
			_factions.Add(faction.Type, faction);
		}
	}
}