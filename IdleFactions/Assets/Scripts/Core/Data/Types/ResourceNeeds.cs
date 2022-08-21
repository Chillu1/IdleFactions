using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class ResourceNeeds //TODO Rename
	{
		public Dictionary<ResourceType, Resource> Generate { get; }
		[CanBeNull] public Dictionary<ResourceType, Resource> CreateCost { get; }
		[CanBeNull] public Dictionary<ResourceType, Resource> GenerateCost { get; }
		[CanBeNull] public Dictionary<ResourceType, Resource> LiveCost { get; }

		public ResourceNeeds(ResourceNeedsProperties properties)
		{
			Generate = properties.Generate.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			CreateCost = properties.CreateCost?.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			GenerateCost = properties.GenerateCost?.ToDictionary(cost => cost.Type, cost => new Resource(cost));
			LiveCost = properties.LiveCost?.ToDictionary(cost => cost.Type, cost => new Resource(cost));
		}

		public void ChangeMultiplier(UpgradeAction upgradeAction)
		{
			ChangeMultiplier(upgradeAction.ResourceNeedsType, upgradeAction.ResourceType, upgradeAction.Multiplier);
		}

		public void ChangeMultiplier(ResourceNeedsType needsType, ResourceType resourceType, double multiplier)
		{
			switch (needsType)
			{
				case ResourceNeedsType.Generate:
					ChangeMultiplier(Generate, resourceType, multiplier);
					break;
				case ResourceNeedsType.CreateCost:
					ChangeMultiplier(CreateCost, resourceType, multiplier);
					break;
				case ResourceNeedsType.GenerateCost:
					ChangeMultiplier(GenerateCost, resourceType, multiplier);
					break;
				case ResourceNeedsType.LiveCost:
					ChangeMultiplier(LiveCost, resourceType, multiplier);
					break;
				default:
					Log.Error("Invalid ResourceNeedsType: " + needsType);
					break;
			}
		}

		private static void ChangeMultiplier(Dictionary<ResourceType, Resource> source, ResourceType type, double multiplier)
		{
			if (!source.ContainsKey(type))
			{
				Log.Error("Invalid ResourceType: " + type);
				return;
			}

			source[type].TimesMultiplier(multiplier);
		}
	}
}