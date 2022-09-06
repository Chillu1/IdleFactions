namespace IdleFactions
{
	public interface IBaseResource
	{
		ResourceType Type { get; }
		double Value { get; }
	}
}