namespace IdleFactions
{
	public struct UpgradeAction
	{
		public ResourceNeedsType ResourceNeedsType { get; }
		public ResourceType ResourceType { get; }
		public double Multiplier { get; }

		public UpgradeAction(ResourceNeedsType resourceNeedsType, ResourceType resourceType, double multiplier)
		{
			ResourceNeedsType = resourceNeedsType;
			ResourceType = resourceType;
			Multiplier = multiplier;
		}
	}
}