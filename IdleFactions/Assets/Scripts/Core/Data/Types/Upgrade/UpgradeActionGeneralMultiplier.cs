namespace IdleFactions
{
	public readonly struct UpgradeActionGeneralMultiplier : IUpgradeAction
	{
		public FactionResourceType FactionResourceType { get; }
		public double Multiplier { get; }

		public UpgradeActionGeneralMultiplier(FactionResourceType factionResourceType, double multiplier)
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