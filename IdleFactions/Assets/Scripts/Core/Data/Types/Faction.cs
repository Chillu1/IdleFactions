using System;
using JetBrains.Annotations;

namespace IdleFactions
{
	public class Faction
	{
		public FactionType Type { get; }
		public double Population { get; private set; }
		public double PopulationDecay { get; private set; } = 1d;
		public ResourceType ResourceType { get; private set; }

		private readonly ResourceNeeds _resourceNeeds;

		private static ResourceController ResourceController { get; set; }

		public Faction(FactionType type, ResourceNeeds resourceNeeds)
		{
			Type = type;
			_resourceNeeds = resourceNeeds;
			ResourceType = ResourceType.None;
		}

		public static void Setup(ResourceController resourceController)
		{
			ResourceController = resourceController;
		}

		public void Update(float delta)
		{
			if (Population <= 0)
				return;

			if (_resourceNeeds.LiveCost != null &&
			    ResourceController.TryUseLiveResource(_resourceNeeds.LiveCost, Population * delta, out double usedLiveMultiplier))
			{
				//Partial use
				if (usedLiveMultiplier < 1d)
					Population -= PopulationDecay * (1d - usedLiveMultiplier) * delta;
			}

			double usedGenMultiplier = 1d;
			if (_resourceNeeds.GenerateCost == null ||
			    ResourceController.TryUseLiveResource(_resourceNeeds.GenerateCost, Population * delta, out usedGenMultiplier))
			{
				ResourceController.Add(_resourceNeeds.Generate, Population * usedGenMultiplier * delta);
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