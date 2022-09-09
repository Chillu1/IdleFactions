namespace IdleFactions
{
	public interface IFactionController : ISavable, ILoadable
	{
		Faction Get(FactionType type);
	}
}