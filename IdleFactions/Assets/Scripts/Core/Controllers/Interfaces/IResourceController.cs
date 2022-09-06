using System.Collections.Generic;

namespace IdleFactions
{
	public interface IResourceController
	{
		void Add(ResourceType type, double value);
		void Add(IReadOnlyDictionary<ResourceType, IResource> resources, double usedGenMultiplier);

		void Add(IReadOnlyDictionary<ResourceType, IResourceAdded> resources, IReadOnlyDictionary<ResourceType, double> multipliers,
			double multiplier);

		void Add(ResourceCost[] resourceCosts);

		bool TryUseResource(ResourceCost[] resourceCosts);

		bool TryUseResource(IReadOnlyDictionary<ResourceType, IResource> resourceCosts, double multiplier);
		//bool TryUseResource((ResourceType type, double value)[] resources, double multiplier);
		//bool TryUseResource(ResourceType neededResourceType, double value);

		/// <summary>
		///		Special function for partial use of resources.
		/// </summary>
		/// <example> Living costs </example>
		/// <returns>If usedMultiplier was partial</returns>
		bool TryUsePartialLiveResource(IReadOnlyDictionary<ResourceType, IResource> resourceCosts, double multiplier,
			out double usedMultiplier);

		void TryUsePartialResource(IReadOnlyDictionary<ResourceType, IResource> resourceCosts, double multiplier,
			out double usedMultiplier);

		void TryUsePartialResourceAdded(IReadOnlyDictionary<ResourceType, IResourceAdded> resourceCosts, double multiplier,
			Dictionary<ResourceType, double> resourceMultipliers);

		StoredResource GetResource(int index);

		StoredResource GetResource(ResourceType type);
	}
}