namespace IdleFactions
{
	public interface IDataStore<TKey, TValue>
	{
		TValue Get(TKey key);
	}
}