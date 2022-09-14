namespace IdleFactions
{
	public readonly struct UpgradeActionMultiplier : IUpgradeAction
	{
		public FactionResourceType FactionResourceType { get; }
		public ResourceType ResourceType { get; }
		public double Multiplier { get; }

		public UpgradeActionMultiplier(FactionResourceType factionResourceType, ResourceType resourceType, double multiplier)
		{
			FactionResourceType = factionResourceType;
			ResourceType = resourceType;
			Multiplier = multiplier;
		}

		public override string ToString()
		{
			return $"Multiplier: {Multiplier} {FactionResourceType.ToString()} {ResourceType.ToString()}";
		}
	}
}