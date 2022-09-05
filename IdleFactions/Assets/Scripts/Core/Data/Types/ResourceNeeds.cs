using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class ResourceNeeds //TODO Rename
	{
		public IReadOnlyDictionary<ResourceType, Resource> Generate => _generate;
		public IReadOnlyDictionary<ResourceType, Resource> CreateCostRead => _createCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, Resource> GenerateCostRead => _generateCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, Resource> LiveCostRead => _liveCost;

		private readonly Dictionary<ResourceType, Resource> _generate;
		private readonly Dictionary<ResourceType, Resource> _createCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, Resource> _generateCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, Resource> _liveCost;

		public ResourceNeeds(ResourceNeedsProperties properties)
		{
			_generate = properties.Generate.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			_createCost = properties.CreateCost.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			//AddNewResource(CreateCost, ResourceType.Essence, 1d);//TODOPRIO
			_generateCost = properties.GenerateCost?.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			_liveCost = properties.LiveCost?.ToDictionary(cost => cost.Type, cost => new Resource(cost));
		}

		public void ActivateUpgradeAction(IUpgradeAction action)
		{
			switch (action)
			{
				case UpgradeActionMultiplier actionMultiplier:
					ChangeMultiplier(actionMultiplier.ResourceNeedsType, actionMultiplier.ResourceType, actionMultiplier.Multiplier);
					break;
				case UpgradeActionNewResource actionNewResource:
					AddNewResource(actionNewResource.ResourceNeedsType, actionNewResource.ResourceType, actionNewResource.Value);
					break;
				default:
					Log.Error("Unknown action type: " + action.GetType());
					break;
			}
		}

		public void RevertUpgradeAction(IUpgradeAction action)
		{
			switch (action)
			{
				case UpgradeActionMultiplier actionMultiplier:
					ChangeMultiplier(actionMultiplier.ResourceNeedsType, actionMultiplier.ResourceType, 1d / actionMultiplier.Multiplier);
					break;
				case UpgradeActionNewResource actionNewResource:
					RemoveNewResource(actionNewResource.ResourceNeedsType, actionNewResource.ResourceType, actionNewResource.Value);
					break;
				default:
					Log.Error("Unknown action type: " + action.GetType());
					break;
			}
		}

		private void ChangeMultiplier(ResourceNeedsType needsType, ResourceType resourceType, double multiplier)
		{
			switch (needsType)
			{
				case ResourceNeedsType.Generate:
					ChangeMultiplier(_generate);
					break;
				case ResourceNeedsType.CreateCost:
					ChangeMultiplier(_createCost);
					break;
				case ResourceNeedsType.GenerateCost:
					ChangeMultiplier(_generateCost);
					break;
				case ResourceNeedsType.LiveCost:
					ChangeMultiplier(_liveCost);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}

			void ChangeMultiplier(Dictionary<ResourceType, Resource> source)
			{
				if (!source.ContainsKey(resourceType))
				{
					Log.Warning($"Creating resource type {resourceType} before it exists. Need to check if works");
					source.Add(resourceType, new Resource(resourceType));
				}

				source[resourceType].TimesMultiplier(multiplier);
			}
		}

		private void AddNewResource(ResourceNeedsType needsType, ResourceType resourceType, double value)
		{
			switch (needsType)
			{
				case ResourceNeedsType.Generate:
					AddNewResource(_generate, resourceType, value);
					break;
				case ResourceNeedsType.CreateCost:
					AddNewResource(_createCost, resourceType, value);
					break;
				case ResourceNeedsType.GenerateCost:
					AddNewResource(_generateCost, resourceType, value);
					break;
				case ResourceNeedsType.LiveCost:
					AddNewResource(_liveCost, resourceType, value);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}
		}

		private static void AddNewResource(Dictionary<ResourceType, Resource> source, ResourceType resourceType, double value)
		{
			if (source.TryGetValue(resourceType, out var resource))
			{
				//Add to base
				resource.Add(value);
				return;
			}

			var newResource = new Resource(resourceType);
			newResource.Add(value);

			source.Add(resourceType, newResource);
		}

		private void RemoveNewResource(ResourceNeedsType needsType, ResourceType resourceType, double value)
		{
			switch (needsType)
			{
				case ResourceNeedsType.Generate:
					RemoveNewResource(_generate);
					break;
				case ResourceNeedsType.CreateCost:
					RemoveNewResource(_createCost);
					break;
				case ResourceNeedsType.GenerateCost:
					RemoveNewResource(_generateCost);
					break;
				case ResourceNeedsType.LiveCost:
					RemoveNewResource(_liveCost);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}

			void RemoveNewResource(Dictionary<ResourceType, Resource> source)
			{
				if (!source.TryGetValue(resourceType, out var resource))
				{
					Log.Error("Trying to remove resource that doesn't exist");
					return;
				}

				//Remove from base
				resource.Remove(value);
			}
		}
	}
}