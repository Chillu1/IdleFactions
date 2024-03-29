using System.Collections.Generic;
using System.Linq;

namespace IdleFactions
{
	/// <summary>
	///		Special resource for added resources that need to hold info of the added resources, to get the right multiplier
	/// </summary>
	public class AddedResource : FactionResource, IAddedResource
	{
		private readonly ResourceType[] _addedResources;
		private readonly List<double> _multipliers;

		/// <summary>
		///		Added resource modifier can be 0, since we don't want to generate any new resources with 0 genCost
		/// </summary>
		private const double MinMultiplier = 0.0d;

		public AddedResource(ResourceType type, double value, params ResourceType[] addedResources) : base(type, value)
		{
			_addedResources = addedResources;
			_multipliers = new List<double>(_addedResources.Length);
		}

		public double GetMultiplier(IDictionary<ResourceType, double> resourceMultipliers)
		{
			if (_addedResources.Length == 0)
				return 1d;

			foreach (var resource in _addedResources)
			{
				if (!resourceMultipliers.ContainsKey(resource))
				{
					Log.Warning("We didn't calculate the multiplier for " + resource);
					continue;
				}

				_multipliers.Add(resourceMultipliers[resource]);
			}

			double multiplier = _multipliers.Min();
			_multipliers.Clear();

			if (multiplier <= MinMultiplier)
				multiplier = MinMultiplier;

			return multiplier;
		}
	}
}