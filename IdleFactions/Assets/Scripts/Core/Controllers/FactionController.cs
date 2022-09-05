using System.Collections.Generic;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class FactionController
	{
		private readonly Dictionary<FactionType, Faction> _factions;

		public FactionController(FactionData factionData)
		{
			_factions = new Dictionary<FactionType, Faction>();

			foreach (var type in FactionTypeHelper.AllFactionTypes)
			{
				var faction = factionData.Get(type);
				if (faction == null)
					continue;

				_factions.Add(type, faction);
			}
		}

		public void Update(float delta)
		{
			foreach (var faction in _factions.Values)
				faction.Update(delta);
		}

		[CanBeNull]
		public Faction GetFaction(FactionType type)
		{
			_factions.TryGetValue(type, out var faction);
			return faction;
		}
	}
}