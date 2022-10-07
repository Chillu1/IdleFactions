namespace IdleFactions
{
	public static partial class UpgradeAction
	{
		public readonly struct Multiplier : IUpgradeAction
		{
			public FactionResourceType FactionResourceType { get; }
			public ResourceType ResourceType { get; }
			public double ResourceMultiplier { get; }

			public Multiplier(FactionResourceType factionResourceType, ResourceType resourceType, double multiplier)
			{
				FactionResourceType = factionResourceType;
				ResourceType = resourceType;
				ResourceMultiplier = multiplier;
			}

			public override string ToString()
			{
				return $"Multiplier: {ResourceMultiplier} {FactionResourceType.ToString()} {ResourceType.ToString()}";
			}
		}
	}
}