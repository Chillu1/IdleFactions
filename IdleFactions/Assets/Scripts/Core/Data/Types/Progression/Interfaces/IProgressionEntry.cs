namespace IdleFactions
{
	public interface IProgressionEntry
	{
		IProgressionCondition Condition { get; }
		IProgressionAction Action { get; }
	}
}