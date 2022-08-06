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
			
			AddFaction(new Faction(FactionType.Light, ResourceType.Light));
			AddFaction(new Faction(FactionType.Water, ResourceType.Water));
			AddFaction(new Faction(FactionType.Nature, ResourceType.Nature, new []
			{
				(ResourceType.Light, 3d), (ResourceType.Water, 2d)
			}));
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