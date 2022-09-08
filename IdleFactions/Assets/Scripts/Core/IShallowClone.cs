namespace IdleFactions
{
	/// <summary>
	///		Used when a class doesn't have reference type members that need to be cloned
	/// </summary>
	public interface IShallowClone<out T>
	{
		T ShallowClone();
	}
}