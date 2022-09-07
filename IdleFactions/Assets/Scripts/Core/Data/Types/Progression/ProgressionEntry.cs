namespace IdleFactions
{
	public class ProgressionEntry : IProgressionEntry
	{
		public IProgressionCondition Condition { get; }
		public IProgressionAction Action { get; }

		public ProgressionEntry(IProgressionCondition condition, IProgressionAction action)
		{
			Condition = condition;
			Action = action;
		}
	}
}