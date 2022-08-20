using System.Collections.Generic;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class FactionController
	{
		private readonly Dictionary<FactionType, Faction> _factions;

		private const float ResourceCooldown = 1.0f;
		private float _resourceTimer;

		public FactionController(FactionData factionData)
		{
			_factions = new Dictionary<FactionType, Faction>();

			foreach (var type in FactionTypeHelper.AllFactionTypes)
				AddFaction(factionData.Get(type));
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

		private Faction AddFaction(Faction faction)
		{
			_factions.Add(faction.Type, faction);
			return faction;
		}
	}
}