using System.Collections.Generic;

namespace IdleFactions
{
	//Ugh, testing
	public interface IResourceController : IResourceAddable, IResourceRemovable, IResourceGettable
	{
	}

	public interface IResourceAddable
	{
		void Add(ResourceType type, double value);
		void Add(IReadOnlyCollection<Resource> resources, double usedGenMultiplier);
	}

	public interface IResourceRemovable
	{
		//bool TryUseResource((ResourceType type, double value)[] resources, double multiplier);
		//bool TryUseResource(ResourceType neededResourceType, double value);
		bool TryUseResource(ResourceCost[] resourceCosts);
		bool TryUseResource(ICollection<Resource> resourceCosts, double multiplier);

		/// <summary>
		///		Special function for partial use of resources.
		/// </summary>
		/// <example> Living costs </example>
		/// <returns>If usedMultiplier was partial</returns>
		bool TryUsePartialLiveResource(ICollection<Resource> resourceCosts, double multiplier, out double usedMultiplier);

		void TryUsePartialResource(ICollection<Resource> resourceCosts, double multiplier, out double usedMultiplier);
	}

	public interface IResourceGettable
	{
		StoredResource GetResource(int index);
		StoredResource GetResource(ResourceType type);
	}
}