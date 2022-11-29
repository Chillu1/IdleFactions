using System.Collections;
using System.Collections.Generic;
using IdleFactions.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public sealed class PrestigeResourceController : ISavable, ILoadable
	{
		public const string JSONKey = "PrestigeResources";

		private readonly IReadOnlyDictionary<FactionType, IReadOnlyDictionary<ResourceType, float>> _resourceMultipliers;

		private readonly IDictionary<FactionType, PrestigeResource> _currentRunResources;
		private readonly IDictionary<FactionType, PrestigeResource> _resources;


		public PrestigeResourceController(PrestigeResourceData prestigeResourceData)
		{
			_resourceMultipliers = prestigeResourceData.GetResourceMultipliers();

			_currentRunResources = new Dictionary<FactionType, PrestigeResource>();
			_resources = new Dictionary<FactionType, PrestigeResource>();
		}

		public void Prestige()
		{
			foreach (var resource in _currentRunResources.Values)
				_resources[resource.Type].Add(resource);

			_currentRunResources.Clear();
			//TODO Restart population, upgrades, etc.
		}

		//TODO different resource types having different weights, ex dwarf stone = 1, dwarf gold = 100
		public void Add(FactionType factionType, ResourceType resourceType, double amount)
		{
			if (!_currentRunResources.ContainsKey(factionType))
				_currentRunResources.Add(factionType, new PrestigeResource(factionType));

			float resourceMultiplier = GetResourceMultiplier(factionType, resourceType);
			_currentRunResources[factionType].Add(amount * resourceMultiplier);
		}

		public bool TrySpend(PrestigeResourceCost[] costs)
		{
			foreach (var cost in costs)
			{
				if (!_resources.ContainsKey(cost.Type))
					return false;

				if (!_resources[cost.Type].CanAfford(cost.Value))
					return false;
			}

			foreach (var cost in costs)
				_resources[cost.Type].Spend(cost.Value);

			return true;
		}

		public bool TrySpend(FactionType factionType, double amount)
		{
			return _resources.ContainsKey(factionType) && _resources[factionType].TrySpend(amount);
		}

		private float GetResourceMultiplier(FactionType factionType, ResourceType resourceType)
		{
			if (!_resourceMultipliers.ContainsKey(factionType))
			{
				Log.Warning("FactionType not found in resource multipliers: " + factionType);
				return 1f;
			}

			if (!_resourceMultipliers[factionType].ContainsKey(resourceType))
			{
				Log.Warning("No resource multiplier: " + resourceType +
				            "We either didn't setup a multiplier for a main resource type, or we're adding a non-prestige resource as a prestige resource");
				return 1f;
			}

			return _resourceMultipliers[factionType][resourceType];
		}

		public void Save(JsonTextWriter writer)
		{
			writer.WritePropertyName(JSONKey);
			writer.WriteStartObject();
			writer.WritePropertyName("CurrentRunResources");
			writer.WriteStartArray();
			foreach (var resource in _currentRunResources.Values)
				resource.Save(writer);
			writer.WriteEndArray();

			writer.WritePropertyName("Resources");
			writer.WriteStartArray();
			foreach (var resource in _resources.Values)
				resource.Save(writer);
			writer.WriteEndArray();
			writer.WriteEndObject();
		}

		public void Load(JObject data)
		{
			var mainKey = data.Value<JObject>(JSONKey);
			var currentRunResources = mainKey.Value<JArray>("CurrentRunResources");

			foreach (var resource in currentRunResources.EmptyIfNull())
			{
				var factionType = (FactionType)resource.Value<int>(nameof(PrestigeResource.Type));
				_currentRunResources[factionType] = new PrestigeResource(factionType, resource.Value<double>("Amount"));
			}

			var resources = mainKey.Value<JArray>("Resources");

			foreach (var resource in resources.EmptyIfNull())
			{
				var factionType = (FactionType)resource.Value<int>(nameof(PrestigeResource.Type));
				_resources[factionType] = new PrestigeResource(factionType, resource.Value<double>("Amount"));
			}
		}
	}
}