namespace IdleFactions
{
	public readonly struct PrestigeResourceCost
	{
		public FactionType Type { get; }
		public double Value { get; }

		public PrestigeResourceCost(FactionType type, double value = 1d)
		{
			Type = type;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Type}: {Value}";
		}
	}
}