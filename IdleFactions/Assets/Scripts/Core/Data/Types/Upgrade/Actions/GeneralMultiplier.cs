namespace IdleFactions
{
	public static partial class UpgradeAction
	{
		public readonly struct GeneralMultiplier : IUpgradeAction
		{
			public FactionResourceType FactionResourceType { get; }
			public double Multiplier { get; }

			public GeneralMultiplier(FactionResourceType factionResourceType, double multiplier)
			{
				FactionResourceType = factionResourceType;
				Multiplier = multiplier;
			}

			public override string ToString()
			{
				return $"Multiplier: {Multiplier} {FactionResourceType.ToString()}";
			}
		}
	}
}