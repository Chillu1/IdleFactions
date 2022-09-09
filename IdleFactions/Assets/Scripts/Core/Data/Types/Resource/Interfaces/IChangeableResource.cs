namespace IdleFactions
{
	public interface IChangeableResource : IResource, ISavable
	{
		void Add(double value);
		void Remove(double value);
	}
}