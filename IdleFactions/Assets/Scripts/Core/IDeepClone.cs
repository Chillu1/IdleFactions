namespace IdleFactions
{
	/// <summary>
	///		Used when a class has reference type members that need to be cloned
	/// </summary>
	public interface IDeepClone<out T>
	{
		T DeepClone();
	}
}