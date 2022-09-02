namespace IdleFactions
{
	public readonly struct UpgradeActionNewResource : IUpgradeAction
	{
		public ResourceNeedsType ResourceNeedsType { get; }
		public ResourceType ResourceType { get; }
		public double Value { get; }

		public UpgradeActionNewResource(ResourceNeedsType resourceNeedsType, ResourceType resourceType, double amount)
		{
			ResourceNeedsType = resourceNeedsType;
			ResourceType = resourceType;
			Value = amount;
		}
	}
}