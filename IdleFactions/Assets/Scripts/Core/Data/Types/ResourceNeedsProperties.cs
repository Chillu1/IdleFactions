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

		public void SetGenerate(ResourceCost cost) => Generate = new[] { cost };
		public void SetGenerate(ResourceCost[] costs) => Generate = costs;

		public void SetCreateCost(ResourceCost cost) => CreateCost = new[] { cost };
		public void SetCreateCost(ResourceCost[] costs) => CreateCost = costs;

		public void SetLiveCost(ResourceCost cost) => LiveCost = new[] { cost };
		public void SetLiveCost(ResourceCost[] costs) => LiveCost = costs;

		public void SetGenerateCost(ResourceCost cost) => GenerateCost = new[] { cost };
		public void SetGenerateCost(ResourceCost[] costs) => GenerateCost = costs;
	}
}