using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class ResourceController
	{
		private Dictionary<ResourceType, StoredResource> _resources;

		public ResourceController()
		{
			_resources = new Dictionary<ResourceType, StoredResource>();
			_resources.Add(ResourceType.Light, new StoredResource(ResourceType.Light));
			_resources.Add(ResourceType.Dark, new StoredResource(ResourceType.Dark));
		}

		public void Add(ResourceType type, double value)
		{
			if (!_resources.ContainsKey(type))
			{
				var resource = new StoredResource(type);
				resource.Add(value);
				_resources.Add(type, resource);
			}
			else
			{
				_resources[type].Add(value);
			}
		}

		public void Add(IReadOnlyCollection<Resource> resources, double usedGenMultiplier)
		{
			foreach (var cost in resources)
				Add(cost.Type, cost.Value * usedGenMultiplier);
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

		public bool TryUseResource(Dictionary<ResourceType, Resource>.ValueCollection resourceCosts, double multiplier)
		{
			foreach (var cost in resourceCosts)
			{
				if (!_resources.ContainsKey(cost.Type))
					return false;
				if (_resources[cost.Type].Value < cost.Value * multiplier)
					return false;
			}

			foreach (var cost in resourceCosts)
				_resources[cost.Type].Remove(cost.Value * multiplier);

			return true;
		}

		/// <summary>
		///		Special function for partial use of resources.
		/// </summary>
		/// <example> Living costs </example>
		public bool TryUseLiveResource(Dictionary<ResourceType, Resource>.ValueCollection resourceCosts, double multiplier,
			out double usedMultiplier)
		{
			double[] usedMultipliers = new double[resourceCosts.Count];
			for (int i = 0; i < resourceCosts.Count; i++)
			{
				var cost = resourceCosts.ElementAt(i);
				if (!_resources.ContainsKey(cost.Type))
				{
					usedMultiplier = 0;
					return false;
				}

				if (_resources[cost.Type].Value == 0)
				{
					usedMultiplier = 0;
					return false;
				}

				//Not enough resources
				if (_resources[cost.Type].Value < cost.Value * multiplier)
				{
					usedMultipliers[i] = _resources[cost.Type].Value / cost.Value * multiplier;
					continue;
				}

				usedMultipliers[i] = 1d;
			}

			usedMultiplier = usedMultipliers.Min();
			foreach (var cost in resourceCosts)
			{
				_resources[cost.Type].Remove(cost.Value * multiplier * usedMultiplier);
			}

			return usedMultiplier > 0;
		}

		public StoredResource GetResource(int index)
		{
			if (index < 0 || index >= _resources.Count)
				return null;

			return _resources.ElementAt(index).Value;
		}

		[CanBeNull]
		public StoredResource GetResource(ResourceType type)
		{
			_resources.TryGetValue(type, out var resource);
			return resource;
		}
	}
}