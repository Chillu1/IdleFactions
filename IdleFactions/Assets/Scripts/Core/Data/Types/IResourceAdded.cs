using System.Collections.Generic;

namespace IdleFactions
{
	public interface IResourceAdded : IResource
	{
		double GetMultiplier(IReadOnlyDictionary<ResourceType, double> resourceMultipliers);
	}
}