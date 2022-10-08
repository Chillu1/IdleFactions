namespace IdleFactions
{
	public static partial class ProgressionCondition
	{
		public readonly struct Faction : IProgressionCondition
		{
			public double OrderValue => Value;
			public double Value { get; }

			public Faction(double value) => Value = value;
		}
	}
}