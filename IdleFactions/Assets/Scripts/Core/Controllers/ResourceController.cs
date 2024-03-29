using System;
using System.Collections.Generic;
using System.Linq;
using IdleFactions.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public delegate void ResourceAddedHandler(IChangeableResource resource);

	public class ResourceController : ISavable, ILoadable
	{
		public const string JSONKey = "Resources";

		public ResourceRates Rates { get; }

		public event ResourceAddedHandler Added;

		private readonly Dictionary<ResourceType, IChangeableResource> _resources;

		private readonly IReadOnlyDictionary<FactionType, ISet<ResourceType>> _primaryResources;

		private readonly PrestigeResourceController _prestigeResourceController;

		public ResourceController(PrestigeResourceData prestigeResourceData, PrestigeResourceController prestigeResourceController)
		{
			_prestigeResourceController = prestigeResourceController;

			_primaryResources = prestigeResourceData.GetPrimaryResources();

			_resources = new Dictionary<ResourceType, IChangeableResource>();
			_resources.Add(ResourceType.Essence, new ChangeableResource(ResourceType.Essence));
			_resources.Add(ResourceType.Light, new ChangeableResource(ResourceType.Light));
			_resources.Add(ResourceType.Dark, new ChangeableResource(ResourceType.Dark));

			Rates = new ResourceRates();
		}

		public void Update(float dt)
		{
			Rates.Update(dt);
		}

		public void Add(ResourceType type, double value)
		{
			if (!_resources.ContainsKey(type))
				_resources.Add(type, new ChangeableResource(type));

			_resources[type].Add(value);

			//Added?.Invoke(this, new ResourceEventArgs(type, value));
			Rates.ChangeResource(type, value);
			Added?.Invoke(_resources[type]);
		}

		public void AddByFaction(FactionType factionType, ResourceType type, double value)
		{
			//If resource is part of faction's primary resource, add to prestige resources
			if (_primaryResources.TryGetValue(factionType, out var primaryResources) && primaryResources.Contains(type))
				_prestigeResourceController.Add(factionType, type, value);

			Add(type, value);
		}

		public void AddByFaction(FactionType factionType, IReadOnlyDictionary<ResourceType, IFactionResource> resources,
			double usedGenMultiplier)
		{
			foreach (var cost in resources)
				AddByFaction(factionType, cost.Key, cost.Value.Value * usedGenMultiplier);
		}

		public void AddByFaction(FactionType factionType, IReadOnlyDictionary<ResourceType, IAddedResource> resources,
			IDictionary<ResourceType, double> multipliers, double multiplier)
		{
			foreach (var cost in resources)
			{
				double extraMultiplier = cost.Value.GetMultiplier(multipliers);
				AddByFaction(factionType, cost.Key, cost.Value.Value * multiplier * extraMultiplier);
			}
		}

		public void Add(IReadOnlyDictionary<ResourceType, IFactionResource> resources, double usedGenMultiplier)
		{
			foreach (var cost in resources)
				Add(cost.Key, cost.Value.Value * usedGenMultiplier);
		}

		public void Add(ResourceCost[] resourceCosts)
		{
			foreach (var cost in resourceCosts)
				Add(cost.Type, cost.Value);
		}

		private void Remove(ResourceType type, double value)
		{
			if (!_resources.ContainsKey(type))
				return;

			_resources[type].Remove(value);
			Rates.ChangeResource(type, -value);
		}

		public bool ResourceEquals(IResource resource)
		{
			return ResourceEquals(resource.Type, resource.Value);
		}

		public bool ResourceEquals(ResourceType type, double value, double tolerance = 0.001d)
		{
			if (!_resources.ContainsKey(type))
				return value == 0;

			return Math.Abs(_resources[type].Value - value) < tolerance;
		}

		public bool ContainsResources(IReadOnlyDictionary<ResourceType, IFactionResource> factionResourcesCreateCost)
		{
			foreach (var cost in factionResourcesCreateCost)
			{
				if (!_resources.ContainsKey(cost.Key))
					return false;

				if (_resources[cost.Key].Value < cost.Value.Value)
					return false;
			}

			return true;
		}

		public bool TryUseResource((ResourceType type, double value)[] resources, double multiplier)
		{
			foreach (var resource in resources)
			{
				if (!_resources.ContainsKey(resource.type))
					return false;
				else if (_resources[resource.type].Value < resource.value * multiplier)
					return false;
			}

			foreach (var resource in resources)
				Remove(resource.type, resource.value * multiplier);

			return true;
		}

		public bool TryUseResource(ResourceType neededResourceType, double value)
		{
			if (!_resources.ContainsKey(neededResourceType))
				return false;

			if (_resources[neededResourceType].Value < value)
				return false;

			Remove(neededResourceType, value);
			return true;
		}

		/// <summary>
		///		Doesn't affect rates, for upgrades
		/// </summary>
		public bool TryUseResource(ResourceCost[] resourceCosts)
		{
			foreach (var cost in resourceCosts)
			{
				if (!_resources.ContainsKey(cost.Type))
					return false;
				if (_resources[cost.Type].Value < cost.Value)
					return false;
			}

			foreach (var cost in resourceCosts)
				_resources[cost.Type].Remove(cost.Value);

			return true;
		}

		/// <summary>
		///		Special resource use, that doesn't affect rates
		/// </summary>
		public bool TryUseManualResource(IReadOnlyDictionary<ResourceType, IFactionResource> resourceCosts, double multiplier)
		{
			foreach (var cost in resourceCosts)
			{
				if (!_resources.ContainsKey(cost.Key))
					return false;
				if (_resources[cost.Key].Value < cost.Value.Value * multiplier)
					return false;
			}

			foreach (var cost in resourceCosts)
				_resources[cost.Key].Remove(cost.Value.Value * multiplier);

			return true;
		}

		/// <summary>
		///		Special function for partial use of resources.
		/// </summary>
		/// <example> Living costs </example>
		/// <returns>If usedMultiplier was partial</returns>
		public bool TryUsePartialLiveResource(IReadOnlyDictionary<ResourceType, IFactionResource> resourceCosts, double multiplier,
			out double usedMultiplier)
		{
			TryUsePartialResource(resourceCosts, multiplier, out usedMultiplier);
			//TryUsePartialResource(resourceCosts.LiveCostAdded, multiplier, out usedMultipliersAdded);

			if (usedMultiplier == 1d)
				usedMultiplier = Faction.MaxLiveBonusMultiplier;

			return usedMultiplier < 1d;
		}

		public void TryUsePartialResource(IReadOnlyDictionary<ResourceType, IFactionResource> resourceCosts, double multiplier,
			out double usedMultiplier)
		{
			double[] usedMultipliers = new double[resourceCosts.Count];
			for (int i = 0; i < resourceCosts.Count; i++)
			{
				var cost = resourceCosts.ElementAt(i);
				if (!_resources.ContainsKey(cost.Key))
				{
					usedMultipliers[i] = 0;
					continue;
				}

				if (_resources[cost.Key].Value == 0)
				{
					usedMultipliers[i] = 0;
					continue;
				}

				//Not enough resources
				if (_resources[cost.Key].Value < cost.Value.Value * multiplier)
				{
					usedMultipliers[i] = _resources[cost.Key].Value / (cost.Value.Value * multiplier);
					continue;
				}

				usedMultipliers[i] = 1d;
			}

			usedMultiplier = usedMultipliers.Min();
			foreach (var cost in resourceCosts)
				Remove(cost.Key, cost.Value.Value * multiplier * usedMultiplier);
		}

		public void TryUsePartialResourceAdded(IReadOnlyDictionary<ResourceType, IAddedResource> resourceCosts, double multiplier,
			IDictionary<ResourceType, double> resourceMultipliers)
		{
			foreach (var cost in resourceCosts)
			{
				if (!_resources.ContainsKey(cost.Key))
				{
					resourceMultipliers.Add(cost.Key, 0);
					continue;
				}

				if (_resources[cost.Key].Value == 0)
				{
					resourceMultipliers.Add(cost.Key, 0);
					continue;
				}

				//Not enough resources
				if (_resources[cost.Key].Value < cost.Value.Value * multiplier)
				{
					resourceMultipliers.Add(cost.Key, _resources[cost.Key].Value / (cost.Value.Value * multiplier));
					continue;
				}

				resourceMultipliers.Add(cost.Key, 1d);
			}

			foreach (var cost in resourceCosts)
			{
				double specificResourceMultiplier = cost.Value.GetMultiplier(resourceMultipliers);
				Remove(cost.Key, cost.Value.Value * multiplier * specificResourceMultiplier);
			}
		}

		public IEnumerable<string> GetResourceStrings()
		{
			foreach (var resource in _resources)
				yield return resource.Value.ToString();
		}

		private void Set(ResourceType type, double value)
		{
			_resources[type] = new ChangeableResource(type);
			_resources[type].Add(value);
		}

		public void Save(JsonTextWriter writer)
		{
			writer.WritePropertyName(JSONKey);
			writer.WriteStartArray();
			foreach (var resource in _resources.Values)
				resource.Save(writer);
			writer.WriteEndArray();
		}

		public void Load(JObject data)
		{
			var resources = data.Value<JArray>(JSONKey);

			foreach (var resource in resources.EmptyIfNull())
				Set((ResourceType)resource.Value<int>(nameof(ChangeableResource.Type)),
					resource.Value<double>(nameof(ChangeableResource.Value)));
		}
	}
}