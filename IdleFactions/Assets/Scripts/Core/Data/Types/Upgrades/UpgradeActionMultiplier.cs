namespace IdleFactions
{
	public readonly struct UpgradeActionMultiplier : IUpgradeAction
	{
		public ResourceNeedsType ResourceNeedsType { get; }
		public ResourceType ResourceType { get; }
		public double Multiplier { get; }

		public UpgradeActionMultiplier(ResourceNeedsType resourceNeedsType, ResourceType resourceType, double multiplier)
		{
			ResourceNeedsType = resourceNeedsType;
			ResourceType = resourceType;
			Multiplier = multiplier;
		}
	}
}