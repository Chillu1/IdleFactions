using JetBrains.Annotations;

namespace IdleFactions
{
	public class ResourceNeedsProperties
	{
		public ResourceCost[] Generate;

		[CanBeNull]
		public ResourceCost[] CreateCost;

		[CanBeNull]
		public ResourceCost[] LiveCost;

		[CanBeNull]
		public ResourceCost[] GenerateCost;

		public void AddGenerate(ResourceCost cost) => Generate = new[] { cost };
		public void AddGenerate(ResourceCost[] costs) => Generate = costs;

		public void AddCreateCost(ResourceCost cost) => CreateCost = new[] { cost };
		public void AddCreateCost(ResourceCost[] costs) => CreateCost = costs;

		public void AddLiveCost(ResourceCost cost) => LiveCost = new[] { cost };
		public void AddLiveCost(ResourceCost[] costs) => LiveCost = costs;

		public void AddGenerateCost(ResourceCost cost) => GenerateCost = new[] { cost };
		public void AddGenerateCost(ResourceCost[] costs) => GenerateCost = costs;
	}
}