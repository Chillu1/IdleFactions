using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public delegate void ResourceAddedHandler(IChangeableResource resource);

	public class ResourceController : IResourceController, ISavable
	{
		public event ResourceAddedHandler Added;

		private readonly Dictionary<ResourceType, IChangeableResource> _resources;

		public ResourceController()
		{
			_resources = new Dictionary<ResourceType, IChangeableResource>();
			_resources.Add(ResourceType.Light, new ChangeableResource(ResourceType.Light));
			_resources.Add(ResourceType.Dark, new ChangeableResource(ResourceType.Dark));
		}

		public void Add(ResourceType type, double value)
		{
			if (!_resources.ContainsKey(type))
			{
				var resource = new ChangeableResource(type, value);
				_resources.Add(type, resource);
			}
			else
			{
				_resources[type].Add(value);
			}

			//Added?.Invoke(this, new ResourceEventArgs(type, value));
			Added?.Invoke(_resources[type]);
		}

		public void Add(IReadOnlyDictionary<ResourceType, IFactionResource> resources, double usedGenMultiplier)
		{
			foreach (var cost in resources)
				Add(cost.Key, cost.Value.Value * usedGenMultiplier);
		}

		public void Add(IReadOnlyDictionary<ResourceType, IAddedResource> resources, IDictionary<ResourceType, double> multipliers,
			double multiplier)
		{
			foreach (var cost in resources)
			{
				double extraMultiplier = cost.Value.GetMultiplier(multipliers);
				Add(cost.Key, cost.Value.Value * multiplier * extraMultiplier);
			}
		}

		public void Add(ResourceCost[] resourceCosts)
		{
			foreach (var cost in resourceCosts)
				Add(cost.Type, cost.Value);
		}

		public bool ResourceEquals(IResource resource)
		{
			return ResourceEquals(resource.Type, resource.Value);
		}

		public bool ResourceEquals(ResourceType type, double value)
		{
			if (!_resources.ContainsKey(type))
				return false;

			return _resources[type].Value == value;
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
				_resources[resource.type].Remove(resource.value * multiplier);

			return true;
		}

		public bool TryUseResource(ResourceType neededResourceType, double value)
		{
			if (!_resources.ContainsKey(neededResourceType))
				return false;

			if (_resources[neededResourceType].Value < value)
				return false;

			_resources[neededResourceType].Remove(value);
			return true;
		}

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

		public bool TryUseResource(IReadOnlyDictionary<ResourceType, IFactionResource> resourceCosts, double multiplier)
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
				_resources[cost.Key].Remove(cost.Value.Value * multiplier * usedMultiplier);
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
				_resources[cost.Key].Remove(cost.Value.Value * multiplier * specificResourceMultiplier);
			}
		}

		public IChangeableResource GetResource(int index)
		{
			if (index < 0 || index >= _resources.Count)
				return null;

			return _resources.ElementAt(index).Value;
		}

		[CanBeNull]
		public IChangeableResource GetResource(ResourceType type)
		{
			_resources.TryGetValue(type, out var resource);
			return resource;
		}

		public void Save(JsonTextWriter writer)
		{
			//TODO
		}

		public void Load(JObject data)
		{
			//TODO
		}
	}
}