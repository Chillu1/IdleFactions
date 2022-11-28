using System.Collections.Generic;
using System.Linq;

namespace IdleFactions
{
	public sealed class PrestigeResourceData
	{
		private readonly IDictionary<FactionType, IDictionary<ResourceType, float>> _resourceMultipliers;

		public PrestigeResourceData()
		{
			_resourceMultipliers = new Dictionary<FactionType, IDictionary<ResourceType, float>>();

			SetupResourceMultipliers();
		}

		public IReadOnlyDictionary<FactionType, ISet<ResourceType>> GetPrimaryResources()
		{
			return _resourceMultipliers.ToDictionary(
				kvp => kvp.Key,
				kvp => (ISet<ResourceType>)kvp.Value.Keys.ToHashSet());
		}

		public IReadOnlyDictionary<FactionType, IReadOnlyDictionary<ResourceType, float>> GetResourceMultipliers()
		{
			return _resourceMultipliers.ToDictionary(
				kvp => kvp.Key,
				kvp => (IReadOnlyDictionary<ResourceType, float>)kvp.Value);
		}

		private void SetupResourceMultipliers()
		{
			void SetupResourceMultiplier(FactionType factionType, params (ResourceType Type, float Multiplier)[] resources)
			{
				_resourceMultipliers.Add(factionType, resources.ToDictionary(x => x.Type, x => x.Multiplier));
			}

			SetupResourceMultiplier(FactionType.Creation, (ResourceType.Essence, 1f));
			SetupResourceMultiplier(FactionType.Divinity, (ResourceType.Light, 1f));
			SetupResourceMultiplier(FactionType.Void, (ResourceType.Dark, 1f));
			SetupResourceMultiplier(FactionType.Heat, (ResourceType.Lava, 1f));
			SetupResourceMultiplier(FactionType.Ocean, (ResourceType.Water, 1f));
			SetupResourceMultiplier(FactionType.Golem, (ResourceType.Stone, 1f));

			SetupResourceMultiplier(FactionType.Dwarf, (ResourceType.Stone, 1f), (ResourceType.Gold, 100f));
		}
	}
}