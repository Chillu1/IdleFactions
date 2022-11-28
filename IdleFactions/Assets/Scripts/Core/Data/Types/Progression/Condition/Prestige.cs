namespace IdleFactions
{
	public static partial class ProgressionCondition
	{
		public readonly struct Prestige : IProgressionCondition
		{
			public double OrderValue => Value;
			public int Value { get; }

			public Prestige(int value) => Value = value;
		}
	}
}