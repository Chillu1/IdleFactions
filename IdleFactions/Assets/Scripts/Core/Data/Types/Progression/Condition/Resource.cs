namespace IdleFactions
{
	public static partial class ProgressionCondition
	{
		public readonly struct Resource : IProgressionCondition
		{
			public double OrderValue => Value;
			public double Value { get; }

			public Resource(double value) => Value = value;
		}
	}
}