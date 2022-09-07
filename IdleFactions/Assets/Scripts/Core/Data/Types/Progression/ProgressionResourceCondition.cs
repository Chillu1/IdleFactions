namespace IdleFactions
{
	public struct ProgressionResourceCondition : IProgressionCondition
	{
		public double Value { get; }

		public ProgressionResourceCondition(double value)
		{
			Value = value;
		}
	}
}