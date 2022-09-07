using BreakInfinity;

namespace IdleFactions
{
	public interface IBaseResource
	{
		ResourceType Type { get; }
		BigDouble Value { get; }
	}
}