using BreakInfinity;

namespace IdleFactions
{
	public struct ResourceCost : IBaseResource
	{
		public ResourceType Type { get; }
		public BigDouble Value { get; }

		public ResourceCost(ResourceType type) : this(type, 1d)
		{
		}

		public ResourceCost(ResourceType type, BigDouble value)
		{
			Type = type;
			Value = value;
		}
	}
}