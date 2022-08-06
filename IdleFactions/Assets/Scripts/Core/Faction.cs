using System;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class Faction
	{
		public FactionType Type { get; }
		public int Population { get; private set; }
		public ResourceType ResourceType { get; private set; }

		[CanBeNull]
		private (ResourceType type, double amount)[] _resourceNeeds;

		private static ResourceController ResourceController { get; set; }

		public Faction(FactionType type, ResourceType resourceType, (ResourceType type, double amount)[] neededResourceType = null)
		{
			Type = type;
			ResourceType = resourceType;
			_resourceNeeds = neededResourceType;
		}

		public static void Setup(ResourceController resourceController)
		{
			ResourceController = resourceController;
		}

		public void Update(float delta)
		{
			if (_resourceNeeds == null
			    || _resourceNeeds.Length == 0
			    || ResourceController.TryUseResource(_resourceNeeds, Population * delta))
			{
				ResourceController.Add(ResourceType, Population * delta);
			}
		}

		public void ChangePopulation(int amount)
		{
			Population += amount;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}