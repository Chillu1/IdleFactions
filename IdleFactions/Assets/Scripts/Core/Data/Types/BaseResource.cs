using BreakInfinity;

namespace IdleFactions
{
	public class BaseResource : IBaseResource
	{
		public ResourceType Type { get; }
		public BigDouble Value { get; }

		public BaseResource(ResourceType type)
		{
			Type = type;
			Value = 1d;
		}

		public BaseResource(ResourceType type, BigDouble value)
		{
			Type = type;
			Value = value;
		}
	}
}