using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class FactionResources : IDeepClone<FactionResources>
	{
		public IReadOnlyDictionary<ResourceType, IFactionResource> Generate => _generate;

		/// <summary>
		///		Contains all added resources by upgrades
		/// </summary>
		[CanBeNull]
		public IReadOnlyDictionary<ResourceType, IAddedResource> GenerateAdded => _generateAdded;

		public IReadOnlyDictionary<ResourceType, IFactionResource> CreateCost => _createCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IFactionResource> GenerateCost => _generateCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IAddedResource> GenerateCostAdded => _generateCostAdded;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IFactionResource> LiveCost => _liveCost;
		[CanBeNull] public IReadOnlyDictionary<ResourceType, IAddedResource> LiveCostAdded => _liveCostAdded;

		private readonly Dictionary<ResourceType, IFactionResource> _generate;
		private readonly Dictionary<ResourceType, IAddedResource> _generateAdded;
		private readonly Dictionary<ResourceType, IFactionResource> _createCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, IFactionResource> _generateCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, IAddedResource> _generateCostAdded; //TODO Lazy init

		[CanBeNull]
		private readonly Dictionary<ResourceType, IFactionResource> _liveCost;

		[CanBeNull]
		private readonly Dictionary<ResourceType, IAddedResource> _liveCostAdded;

		private readonly FactionResourceProperties _properties;

		public FactionResources(FactionResourceProperties properties)
		{
			_properties = properties;

			_generate = properties.Generate.ToDictionary(cost => cost.Type,
				cost => (IFactionResource)new FactionResource(cost.Type, cost.Value));
			_createCost = properties.CreateCost?.ToDictionary(cost => cost.Type,
				cost => (IFactionResource)new FactionResource(cost.Type, cost.Value));
			if (_createCost == null)
				_createCost = new Dictionary<ResourceType, IFactionResource>();
			if (_createCost.ContainsKey(ResourceType.Essence))
				Log.Error("ResourceNeeds: Essence is automatically applied to create cost(?). Do we want custom essence amounts?");
			AddNewResource(_createCost, new FactionResource(ResourceType.Essence, 1d));
			_generateCost =
				properties.GenerateCost?.ToDictionary(cost => cost.Type,
					cost => (IFactionResource)new FactionResource(cost.Type, cost.Value));
			_liveCost = properties.LiveCost?.ToDictionary(cost => cost.Type,
				cost => (IFactionResource)new FactionResource(cost.Type, cost.Value));

			_generateAdded = new Dictionary<ResourceType, IAddedResource>();
			_generateCostAdded = new Dictionary<ResourceType, IAddedResource>();
			_liveCostAdded = new Dictionary<ResourceType, IAddedResource>();
		}

		public void ActivateUpgradeAction(IUpgradeAction upgradeAction)
		{
			switch (upgradeAction)
			{
				case UpgradeAction.Multiplier action:
					ChangeMultiplier(action.FactionResourceType, action.ResourceType,
						action.ResourceMultiplier);
					break;
				case UpgradeAction.GeneralMultiplier action:
					ChangeGeneralMultiplier(action.FactionResourceType, action.Multiplier);
					break;
				case UpgradeAction.NewResource action:
					AddNewResource(action.FactionResourceType, action.Resource);
					break;
				default:
					Log.Error("Unknown action type: " + upgradeAction.GetType());
					break;
			}
		}

		public void RevertUpgradeAction(IUpgradeAction upgradeAction)
		{
			switch (upgradeAction)
			{
				case UpgradeAction.Multiplier action:
					ChangeMultiplier(action.FactionResourceType, action.ResourceType,
						1d / action.ResourceMultiplier);
					break;
				case UpgradeAction.GeneralMultiplier action:
					ChangeGeneralMultiplier(action.FactionResourceType, 1d / action.Multiplier);
					break;
				case UpgradeAction.NewResource action:
					RemoveNewResource(action.FactionResourceType, action.Resource);
					break;
				default:
					Log.Error("Unknown action type: " + upgradeAction.GetType());
					break;
			}
		}

		private void ChangeMultiplier(FactionResourceType type, ResourceType resourceType, double multiplier)
		{
			switch (type)
			{
				case FactionResourceType.Generate:
					ChangeMultiplier(_generate);
					ChangeMultiplierAdded(_generateAdded);
					break;
				case FactionResourceType.CreateCost:
					ChangeMultiplier(_createCost);
					break;
				case FactionResourceType.GenerateCost:
					ChangeMultiplier(_generateCost);
					ChangeMultiplierAdded(_generateCostAdded);
					break;
				case FactionResourceType.LiveCost:
					ChangeMultiplier(_liveCost);
					ChangeMultiplierAdded(_liveCostAdded);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + type);
					break;
			}

			void ChangeMultiplier<TResource>(Dictionary<ResourceType, TResource> source) where TResource : IFactionResource
			{
				if (!source.ContainsKey(resourceType))
				{
					Log.Warning($"Creating resource type {resourceType} before it exists. Need to check if works");
					source.Add(resourceType, (TResource)(IFactionResource)new FactionResource(resourceType));
				}

				source[resourceType].TimesMultiplier(multiplier);
			}

			void ChangeMultiplierAdded<TResource>(Dictionary<ResourceType, TResource> source) where TResource : IFactionResource
			{
				if (!source.ContainsKey(resourceType))
					return;

				source[resourceType].TimesMultiplier(multiplier);
			}
		}

		private void ChangeGeneralMultiplier(FactionResourceType type, double multiplier)
		{
			switch (type)
			{
				case FactionResourceType.Generate:
					ChangeGeneralMultiplier(_generate);
					ChangeGeneralMultiplier(_generateAdded);
					break;
				case FactionResourceType.CreateCost:
					ChangeGeneralMultiplier(_createCost);
					break;
				case FactionResourceType.GenerateCost:
					ChangeGeneralMultiplier(_generateCost);
					ChangeGeneralMultiplier(_generateCostAdded);
					break;
				case FactionResourceType.LiveCost:
					ChangeGeneralMultiplier(_liveCost);
					ChangeGeneralMultiplier(_liveCostAdded);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + type);
					break;
			}

			void ChangeGeneralMultiplier<TResource>(IReadOnlyDictionary<ResourceType, TResource> source) where TResource : IFactionResource
			{
				foreach (var resource in source.Values)
					resource.TimesMultiplier(multiplier);
			}
		}

		private void AddNewResource(FactionResourceType type, IFactionResource resource)
		{
			switch (type)
			{
				case FactionResourceType.CreateCost:
					Log.Warning("Do we ever want to add new resources to create cost?");
					AddNewResource(_createCost, resource);
					break;
				case FactionResourceType.GenerateAdded:
					AddNewResource(_generateAdded, (IAddedResource)resource);
					break;
				case FactionResourceType.LiveCostAdded:
					AddNewResource(_liveCostAdded, (IAddedResource)resource);
					break;
				case FactionResourceType.GenerateCostAdded:
					AddNewResource(_generateCostAdded, (IAddedResource)resource);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + type);
					break;
			}
		}

		private static void AddNewResource<TResource>(Dictionary<ResourceType, TResource> source, TResource resource)
			where TResource : IFactionResource
		{
			if (source.TryGetValue(resource.Type, out var localResource))
			{
				//Add to base
				localResource.Add(resource.Value);
				return;
			}

			source.Add(resource.Type, resource);
		}

		private void RemoveNewResource(FactionResourceType type, IResource resource)
		{
			switch (type)
			{
				case FactionResourceType.CreateCost:
					RemoveNewResource(_createCost);
					break;
				case FactionResourceType.GenerateAdded:
					RemoveNewResource(_generateAdded);
					break;
				case FactionResourceType.GenerateCostAdded:
					RemoveNewResource(_generateCostAdded);
					break;
				case FactionResourceType.LiveCostAdded:
					RemoveNewResource(_liveCostAdded);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + type);
					break;
			}

			void RemoveNewResource<TResource>(Dictionary<ResourceType, TResource> source) where TResource : IFactionResource
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

		public FactionResources DeepClone()
		{
			return new FactionResources(_properties);
		}
	}
}