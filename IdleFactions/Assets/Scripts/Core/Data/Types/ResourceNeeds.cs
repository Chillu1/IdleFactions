using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class ResourceNeeds //TODO Rename
	{
		public IReadOnlyDictionary<ResourceType, IResource> Generate => _generate;

		/// <summary>
		///		Contains all added resources by upgrades
		/// </summary>
		[CanBeNull]
		public IReadOnlyDictionary<ResourceType, IResourceAdded> GenerateAdded => _generateAdded;

		public IReadOnlyDictionary<ResourceType, IResource> CreateCost => _createCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IResource> GenerateCost => _generateCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IResourceAdded> GenerateCostAdded => _generateCostAdded;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IResource> LiveCost => _liveCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IResourceAdded> LiveCostAdded => _liveCostAdded;

		private readonly Dictionary<ResourceType, IResource> _generate;
		private readonly Dictionary<ResourceType, IResourceAdded> _generateAdded;
		private readonly Dictionary<ResourceType, IResource> _createCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, IResource> _generateCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, IResourceAdded> _generateCostAdded;

		[CanBeNull]
		private readonly Dictionary<ResourceType, IResource> _liveCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, IResourceAdded> _liveCostAdded;

		public ResourceNeeds(ResourceNeedsProperties properties)
		{
			_generate = properties.Generate.ToDictionary(cost => cost.Type, cost => (IResource)new Resource(cost.Type, cost.Value));
			_createCost = properties.CreateCost?.ToDictionary(cost => cost.Type, cost => (IResource)new Resource(cost.Type, cost.Value));
			if (_createCost == null)
				_createCost = new Dictionary<ResourceType, IResource>();
			if (_createCost.ContainsKey(ResourceType.Essence))
				Log.Error("ResourceNeeds: Essence is automatically applied to create cost(?). Do we want custom essence amounts?");
			AddNewResource(_createCost, new Resource(ResourceType.Essence, 1d));
			_generateCost = properties.GenerateCost?.ToDictionary(cost => cost.Type, cost => (IResource)new Resource(cost.Type, cost.Value));
			_liveCost = properties.LiveCost?.ToDictionary(cost => cost.Type, cost => (IResource)new Resource(cost.Type, cost.Value));

			_generateAdded = new Dictionary<ResourceType, IResourceAdded>();
			_generateCostAdded = new Dictionary<ResourceType, IResourceAdded>();
			_liveCostAdded = new Dictionary<ResourceType, IResourceAdded>();
		}

		public void ActivateUpgradeAction(IUpgradeAction action)
		{
			switch (action)
			{
				case UpgradeActionMultiplier actionMultiplier:
					ChangeMultiplier(actionMultiplier.ResourceNeedsType, actionMultiplier.ResourceType, actionMultiplier.Multiplier);
					break;
				case UpgradeActionGeneralMultiplier actionGeneralMultiplier:
					ChangeGeneralMultiplier(actionGeneralMultiplier.ResourceNeedsType, actionGeneralMultiplier.Multiplier);
					break;
				case UpgradeActionNewResource actionNewResource:
					AddNewResource(actionNewResource.ResourceNeedsType, actionNewResource.Resource);
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
				case UpgradeActionGeneralMultiplier actionGeneralMultiplier:
					ChangeGeneralMultiplier(actionGeneralMultiplier.ResourceNeedsType, 1d / actionGeneralMultiplier.Multiplier);
					break;
				case UpgradeActionNewResource actionNewResource:
					RemoveNewResource(actionNewResource.ResourceNeedsType, actionNewResource.Resource);
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
					ChangeMultiplierAdded(_generateAdded);
					break;
				case ResourceNeedsType.CreateCost:
					ChangeMultiplier(_createCost);
					break;
				case ResourceNeedsType.GenerateCost:
					ChangeMultiplier(_generateCost);
					ChangeMultiplierAdded(_generateCostAdded);
					break;
				case ResourceNeedsType.LiveCost:
					ChangeMultiplier(_liveCost);
					ChangeMultiplierAdded(_liveCostAdded);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}

			void ChangeMultiplier<TResource>(Dictionary<ResourceType, TResource> source) where TResource : IResource
			{
				if (!source.ContainsKey(resourceType))
				{
					Log.Warning($"Creating resource type {resourceType} before it exists. Need to check if works");
					source.Add(resourceType, (TResource)(IResource)new Resource(resourceType));
				}

				source[resourceType].TimesMultiplier(multiplier);
			}

			void ChangeMultiplierAdded<TResource>(Dictionary<ResourceType, TResource> source) where TResource : IResource
			{
				if (!source.ContainsKey(resourceType))
					return;

				source[resourceType].TimesMultiplier(multiplier);
			}
		}

		private void ChangeGeneralMultiplier(ResourceNeedsType needsType, double multiplier)
		{
			switch (needsType)
			{
				case ResourceNeedsType.Generate:
					ChangeGeneralMultiplier(_generate);
					ChangeGeneralMultiplier(_generateAdded);
					break;
				case ResourceNeedsType.CreateCost:
					ChangeGeneralMultiplier(_createCost);
					break;
				case ResourceNeedsType.GenerateCost:
					ChangeGeneralMultiplier(_generateCost);
					ChangeGeneralMultiplier(_generateCostAdded);
					break;
				case ResourceNeedsType.LiveCost:
					ChangeGeneralMultiplier(_liveCost);
					ChangeGeneralMultiplier(_liveCostAdded);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}

			void ChangeGeneralMultiplier<TResource>(IReadOnlyDictionary<ResourceType, TResource> source) where TResource : IResource
			{
				foreach (var resource in source.Values)
					resource.TimesMultiplier(multiplier);
			}
		}

		private void AddNewResource(ResourceNeedsType needsType, IResource resource)
		{
			switch (needsType)
			{
				case ResourceNeedsType.CreateCost:
					Log.Warning("Do we ever want to add new resources to create cost?");
					AddNewResource(_createCost, resource);
					break;
				case ResourceNeedsType.GenerateAdded:
					AddNewResource(_generateAdded, (IResourceAdded)resource);
					break;
				case ResourceNeedsType.LiveCostAdded:
					AddNewResource(_liveCostAdded, (IResourceAdded)resource);
					break;
				case ResourceNeedsType.GenerateCostAdded:
					AddNewResource(_generateCostAdded, (IResourceAdded)resource);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}
		}

		private static void AddNewResource<TResource>(Dictionary<ResourceType, TResource> source, TResource resource)
			where TResource : IResource
		{
			if (source.TryGetValue(resource.Type, out var localResource))
			{
				//Add to base
				localResource.Add(resource.Value);
				return;
			}

			source.Add(resource.Type, resource);
		}

		private void RemoveNewResource(ResourceNeedsType needsType, IBaseResource resource)
		{
			switch (needsType)
			{
				case ResourceNeedsType.CreateCost:
					RemoveNewResource(_createCost);
					break;
				case ResourceNeedsType.GenerateAdded:
					RemoveNewResource(_generateAdded);
					break;
				case ResourceNeedsType.GenerateCostAdded:
					RemoveNewResource(_generateCostAdded);
					break;
				case ResourceNeedsType.LiveCostAdded:
					RemoveNewResource(_liveCostAdded);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}

			void RemoveNewResource<TResource>(Dictionary<ResourceType, TResource> source) where TResource : IResource
			{
				if (!source.TryGetValue(resource.Type, out var localResource))
				{
					Log.Error("Trying to remove resource that doesn't exist");
					return;
				}

				//Remove from base
				localResource.Remove(resource.Value);
			}
		}
	}
}