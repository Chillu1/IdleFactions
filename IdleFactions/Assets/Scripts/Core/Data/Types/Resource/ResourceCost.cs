namespace IdleFactions
{
	public struct ResourceCost : IResource
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