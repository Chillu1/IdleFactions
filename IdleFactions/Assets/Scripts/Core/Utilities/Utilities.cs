namespace IdleFactions.Utils
{
	public static class Utilities
	{
		/// <summary>
		///     Is power of two, and isn't zero
		/// </summary>
		/// <param name="x"></param>
		public static bool IsPowerOfTwo(int x)
		{
			return x != 0 && (x & (x - 1)) == 0;
		}
	}
}