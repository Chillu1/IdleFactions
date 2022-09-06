namespace IdleFactions
{
	public interface IResource : IBaseResource
	{
		void Add(double value);
		void Remove(double value);
		void TimesMultiplier(double multiplier);
	}
}