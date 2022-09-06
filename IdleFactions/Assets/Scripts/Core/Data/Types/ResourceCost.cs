namespace IdleFactions
{
	public struct ResourceCost : IBaseResource
	{
		public ResourceType Type { get; }
		public double Value { get; }

		public ResourceCost(ResourceType type, double value = 1d)
		{
			Type = type;
			Value = value;
		}
	}
}