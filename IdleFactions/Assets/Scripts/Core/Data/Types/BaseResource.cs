namespace IdleFactions
{
	public class BaseResource : IBaseResource
	{
		public ResourceType Type { get; }
		public double Value { get; }

		public BaseResource(ResourceType type, double value = 1d)
		{
			Type = type;
			Value = value;
		}
	}
}