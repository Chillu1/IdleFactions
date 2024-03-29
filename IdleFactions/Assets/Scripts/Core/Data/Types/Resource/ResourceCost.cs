namespace IdleFactions
{
	public readonly struct ResourceCost : IResource
	{
		public ResourceType Type { get; }
		public double Value { get; }

		public ResourceCost(ResourceType type, double value = 1d)
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