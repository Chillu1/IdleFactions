using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public class FactionController : IFactionController
	{
		public const string JSONKey = "Factions";

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
		public Faction Get(FactionType type)
		{
			_factions.TryGetValue(type, out var faction);
			return faction;
		}

		public void Save(JsonTextWriter writer)
		{
			writer.WritePropertyName(JSONKey);
			writer.WriteStartArray();

			foreach (var faction in _factions.Values)
				faction.Save(writer);

			writer.WriteEndArray();
		}

		public void Load(JObject data)
		{
			var factions = data.Value<JArray>(JSONKey);
			foreach (var factionData in factions!)
			{
				_factions[(FactionType)factionData.Value<int>(nameof(Faction.Type))].Load((JObject)factionData);
			}
		}
	}
}