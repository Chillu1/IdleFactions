namespace IdleFactions
{
	public class Progression
	{
		public IProgressionCondition Condition { get; }
		public IProgressionAction Action { get; }

		public Progression(IProgressionCondition condition, IProgressionAction action)
		{
			Condition = condition;
			Action = action;
		}

		public void Increment()
		{
			//TODOPRIO
		}
	}
}