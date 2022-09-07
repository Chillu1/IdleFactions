using BreakInfinity;

namespace IdleFactions
{
	public readonly struct PopulationPurchase : IRevertible
	{
		private readonly Faction _faction;
		private readonly BigDouble _amount;
		private readonly double _multiplier;

		public PopulationPurchase(Faction faction, BigDouble amount, double multiplier)
		{
			_faction = faction;
			_amount = amount;
			_multiplier = multiplier;
		}

		public void Revert()
		{
			_faction.RevertPopulation(_amount, _multiplier);
		}
	}
}