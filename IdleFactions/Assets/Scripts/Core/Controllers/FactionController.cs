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

			//Starting factions
			AddFaction(factionData.Get(FactionType.Divinity));
			AddFaction(factionData.Get(FactionType.Ocean));
			//AddFaction(factionData.Get(FactionType.Nature));
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