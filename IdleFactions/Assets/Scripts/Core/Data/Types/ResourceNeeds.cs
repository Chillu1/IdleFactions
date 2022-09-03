using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class ResourceNeeds //TODO Rename
	{
		public Dictionary<ResourceType, Resource> Generate { get; }
		public Dictionary<ResourceType, Resource> CreateCost { get; }
		[CanBeNull] public Dictionary<ResourceType, Resource> GenerateCost { get; }
		[CanBeNull] public Dictionary<ResourceType, Resource> LiveCost { get; }

		public ResourceNeeds(ResourceNeedsProperties properties)
		{
			Generate = properties.Generate.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			CreateCost = properties.CreateCost.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			GenerateCost = properties.GenerateCost?.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			LiveCost = properties.LiveCost?.ToDictionary(cost => cost.Type, cost => new Resource(cost));
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
					ChangeMultiplier(Generate);
					break;
				case ResourceNeedsType.CreateCost:
					ChangeMultiplier(CreateCost);
					break;
				case ResourceNeedsType.GenerateCost:
					ChangeMultiplier(GenerateCost);
					break;
				case ResourceNeedsType.LiveCost:
					ChangeMultiplier(LiveCost);
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
					AddNewResource(Generate);
					break;
				case ResourceNeedsType.CreateCost:
					AddNewResource(CreateCost);
					break;
				case ResourceNeedsType.GenerateCost:
					AddNewResource(GenerateCost);
					break;
				case ResourceNeedsType.LiveCost:
					AddNewResource(LiveCost);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}

			void AddNewResource(Dictionary<ResourceType, Resource> source)
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
		}

		private void RemoveNewResource(ResourceNeedsType needsType, ResourceType resourceType, double value)
		{
			switch (needsType)
			{
				case ResourceNeedsType.Generate:
					RemoveNewResource(Generate);
					break;
				case ResourceNeedsType.CreateCost:
					RemoveNewResource(CreateCost);
					break;
				case ResourceNeedsType.GenerateCost:
					RemoveNewResource(GenerateCost);
					break;
				case ResourceNeedsType.LiveCost:
					RemoveNewResource(LiveCost);
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