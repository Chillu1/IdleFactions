namespace IdleFactions
{
	public interface IProgressionUpgrade : IUpgrade
	{
		ResourceCost[] Costs { get; }
		FactionType FactionType { get; }
	}
}