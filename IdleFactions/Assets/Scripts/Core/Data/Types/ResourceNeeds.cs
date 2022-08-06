using JetBrains.Annotations;

namespace IdleFactions
{
	public class ResourceNeeds
	{
		public ResourceCost[] Generate { get; }
		[CanBeNull] public ResourceCost[] CreateCost { get; }
		[CanBeNull] public ResourceCost[] GenerateCost { get; }
		[CanBeNull] public ResourceCost[] LiveCost { get; }

		public ResourceNeeds(ResourceCost generate, [CanBeNull] ResourceCost createCost,
			[CanBeNull] ResourceCost generateCost = null, [CanBeNull] ResourceCost liveCost = null)
		{
			Generate = new[] { generate };
			if (createCost != null) CreateCost = new[] { createCost };
			if (generateCost != null) GenerateCost = new[] { generateCost };
			if (liveCost != null) LiveCost = new[] { liveCost };
		}

		public ResourceNeeds(ResourceCost[] generate, ResourceCost[] createCost,
			ResourceCost[] generateCost = null, ResourceCost[] liveCost = null)
		{
			Generate = generate;
			CreateCost = createCost;
			GenerateCost = generateCost;
			LiveCost = liveCost;
		}
	}
}