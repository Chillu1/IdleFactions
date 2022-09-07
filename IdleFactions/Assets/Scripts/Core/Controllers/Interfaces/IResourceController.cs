using System.Collections.Generic;

namespace IdleFactions
{
	public interface IResourceController
	{
		event ResourceAddedHandler Added;

		void Add(ResourceType type, double value);
		void Add(IReadOnlyDictionary<ResourceType, IFactionResource> resources, double usedGenMultiplier);

		void Add(IReadOnlyDictionary<ResourceType, IAddedResource> resources, IDictionary<ResourceType, double> multipliers,
			double multiplier);

		void Add(ResourceCost[] resourceCosts);

		bool TryUseResource(ResourceCost[] resourceCosts);

		bool TryUseResource(IReadOnlyDictionary<ResourceType, IFactionResource> resourceCosts, double multiplier);
		//bool TryUseResource((ResourceType type, double value)[] resources, double multiplier);
		//bool TryUseResource(ResourceType neededResourceType, double value);

		/// <summary>
		///		Special function for partial use of resources.
		/// </summary>
		/// <example> Living costs </example>
		/// <returns>If usedMultiplier was partial</returns>
		bool TryUsePartialLiveResource(IReadOnlyDictionary<ResourceType, IFactionResource> resourceCosts, double multiplier,
			out double usedMultiplier);

		void TryUsePartialResource(IReadOnlyDictionary<ResourceType, IFactionResource> resourceCosts, double multiplier,
			out double usedMultiplier);

		void TryUsePartialResourceAdded(IReadOnlyDictionary<ResourceType, IAddedResource> resourceCosts, double multiplier,
			IDictionary<ResourceType, double> resourceMultipliers);

		IChangeableResource GetResource(int index);

		IChangeableResource GetResource(ResourceType type);
	}
}