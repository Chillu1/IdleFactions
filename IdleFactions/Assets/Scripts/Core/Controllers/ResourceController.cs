using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleFactions
{
	public class ResourceController
	{
		private Dictionary<ResourceType, Resource> _resources;

		public ResourceController()
		{
			_resources = new Dictionary<ResourceType, Resource>();
		}

		public void Add(Resource resource)
		{
			if (!_resources.ContainsKey(resource.Type))
			{
				_resources.Add(resource.Type, resource);
			}
			else
			{
				_resources[resource.Type].Add(resource.Value);
			}
		}

		public void Add(ResourceType type, double amount)
		{
			if (!_resources.ContainsKey(type))
			{
				var resource = new Resource(type);
				resource.Add(amount);
				_resources.Add(type, resource);
			}
			else
			{
				_resources[type].Add(amount);
			}
		}

		public void Add(ResourceCost[] resources, double usedGenMultiplier)
		{
			foreach (var cost in resources)
				Add(cost.Type, cost.Value * usedGenMultiplier);
		}

		public bool TryUseResource((ResourceType type, double amount)[] resources, double multiplier)
		{
			foreach (var resource in resources)
			{
				if (!_resources.ContainsKey(resource.type))
					return false;
				else if (_resources[resource.type].Value < resource.amount * multiplier)
					return false;
			}

			foreach (var resource in resources)
				_resources[resource.type].Remove(resource.amount * multiplier);

			return true;
		}

		public bool TryUseResource(ResourceType neededResourceType, double multiplier)
		{
			if (!_resources.ContainsKey(neededResourceType))
				return false;

			if (_resources[neededResourceType].Value < multiplier)
				return false;

			_resources[neededResourceType].Remove(multiplier);
			return true;
		}

		public bool TryUseResource(ResourceCost[] resourceCosts, double multiplier)
		{
			foreach (var cost in resourceCosts)
			{
				if (!_resources.ContainsKey(cost.Type))
					return false;
				else if (_resources[cost.Type].Value < cost.Value * multiplier)
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
		public bool TryUseLiveResource(ResourceCost[] resourceCosts, double multiplier, out double usedMultiplier)
		{
			double[] usedMultipliers = new double[resourceCosts.Length];
			for (int i = 0; i < resourceCosts.Length; i++)
			{
				var cost = resourceCosts[i];
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
				_resources[cost.Type].Remove(cost.Value * usedMultiplier);

			return usedMultiplier > 0;
		}

		public Resource GetResource(int index)
		{
			if (index < 0 || index >= _resources.Count)
				return null;

			return _resources.ElementAt(index).Value;
		}
	}
}