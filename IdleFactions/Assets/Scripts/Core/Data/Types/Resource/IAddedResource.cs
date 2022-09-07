using System.Collections.Generic;

namespace IdleFactions
{
	public interface IAddedResource : IFactionResource
	{
		double GetMultiplier(IDictionary<ResourceType, double> resourceMultipliers);
	}
}