namespace IdleFactions
{
	public struct ProgressionResourceCondition : IProgressionCondition
	{
		public double OrderValue => Value;
		public double Value { get; }

		public ProgressionResourceCondition(double value)
		{
			Value = value;
		}
	}
}