namespace IdleFactions
{
	public interface IUpgrade : IRevertible, ISavable, ILoadable, IShallowClone<Upgrade>, INotification
	{
		string Id { get; }
		bool IsNew { get; }
		bool IsUnlocked { get; }
		bool IsBought { get; }
		void SetupFactionType(FactionType factionType);
		void SetupFaction(Faction faction);
		void SetNotNew();
		void Unlock();
		bool TryBuy();
		string GetDataString();
		string GetCostsString();
	}
}