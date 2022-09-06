namespace IdleFactions
{
	public struct ProgressionResourceCondition : IProgressionCondition
	{
		public ResourceCost ResourceCost { get; }

		public ProgressionResourceCondition(ResourceCost resourceCost)
		{
			ResourceCost = resourceCost;
		}
	}
}