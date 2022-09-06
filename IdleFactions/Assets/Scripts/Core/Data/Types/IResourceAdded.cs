using System.Collections.Generic;

namespace IdleFactions
{
	public interface IResourceAdded : IResource
	{
		double GetMultiplier(IDictionary<ResourceType, double> resourceMultipliers);
	}
}