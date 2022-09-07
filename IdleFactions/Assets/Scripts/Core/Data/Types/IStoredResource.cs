using BreakInfinity;

namespace IdleFactions
{
	public interface IStoredResource : IBaseResource // TODO Rename
	{
		void Add(BigDouble value);
		void Remove(BigDouble value);
	}
}