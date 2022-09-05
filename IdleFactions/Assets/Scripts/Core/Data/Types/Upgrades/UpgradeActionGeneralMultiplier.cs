namespace IdleFactions
{
	public readonly struct UpgradeActionGeneralMultiplier : IUpgradeAction
	{
		public ResourceNeedsType ResourceNeedsType { get; }
		public double Multiplier { get; }

		public UpgradeActionGeneralMultiplier(ResourceNeedsType resourceNeedsType, double multiplier)
		{
			ResourceNeedsType = resourceNeedsType;
			Multiplier = multiplier;
		}
	}
}