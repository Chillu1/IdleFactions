namespace IdleFactions
{
	public interface IUpgrade : IRevertible, ISavable, ILoadable, IShallowClone<Upgrade>
	{
		string Id { get; }
		bool Unlocked { get; }
		bool Bought { get; }
		void SetupFactionType(FactionType factionType);
		void SetupFaction(Faction faction);
		void Unlock();
		bool TryBuy();
		string GetDataString();
		string GetCostsString();
	}
}