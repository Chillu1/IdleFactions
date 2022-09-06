namespace IdleFactions
{
	public interface IStoredResource : IBaseResource // TODO Rename
	{
		void Add(double value);
		void Remove(double value);
	}
}