namespace IdleFactions
{
	public interface IChangeableResource : IResource
	{
		void Add(double value);
		void Remove(double value);
	}
}