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
				_resources[resource.Type] += resource.Value;
			}
		}
		
		public void Add(ResourceType type, double amount)
		{
			if (!_resources.ContainsKey(type))
			{
				var resource = new Resource(type);
				resource += amount;
				_resources.Add(type, resource);
			}
			else
			{
				_resources[type] += amount;
			}
		}

		public bool TryUseResource((ResourceType type, double amount)[] resources, double multiplier)
		{
			foreach (var resource in resources)
			{
				if (!_resources.ContainsKey(resource.type))
					return false;
				else if (_resources[resource.type] < resource.amount * multiplier)
					return false;
			}
			
			foreach (var resource in resources)
				_resources[resource.type] -= resource.amount * multiplier;

			return true;
		}

		public bool TryUseResource(ResourceType neededResourceType, double neededResourceAmount)
		{
			if (!_resources.ContainsKey(neededResourceType))
				return false;
			
			if (_resources[neededResourceType] < neededResourceAmount)
				return false;
			
			_resources[neededResourceType] -= neededResourceAmount;
			return true;
		}

		public Resource GetResource(int index)
		{
			if(index < 0 || index >= _resources.Count)
				return null;
			
			return _resources.ElementAt(index).Value;
		}
	}
}