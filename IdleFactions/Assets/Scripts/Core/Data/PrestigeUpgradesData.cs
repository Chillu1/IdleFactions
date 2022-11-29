using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleFactions
{
	/// <summary>
	///		Upgrades bought by prestige resources
	/// </summary>
	public sealed class PrestigeUpgradesData
	{
		//TODO
		//Unlocks alliance
		//Unlocks side/optional faction

		private readonly IDictionary<FactionType, IPrestigeUpgrade[]> _prestigeUpgrades;

		public PrestigeUpgradesData()
		{
			_prestigeUpgrades = SetupPrestigeUpgrades();
		}

		public IReadOnlyList<IPrestigeUpgrade> GetPrestigeUpgrades(FactionType factionType)
		{
			return _prestigeUpgrades.ContainsKey(factionType) ? _prestigeUpgrades[factionType] : Array.Empty<IPrestigeUpgrade>();
		}

		private static IDictionary<FactionType, IPrestigeUpgrade[]> SetupPrestigeUpgrades()
		{
			var prestigeUpgrades = new Dictionary<FactionType, IPrestigeUpgrade[]>();

			//Single resource cost setup rn
			void SetupUpgrades(FactionType factionType, params (string Id, double Amount, IPrestigeUpgradeAction Action)[] upgrades)
			{
				prestigeUpgrades.Add(factionType,
					upgrades.Select(x => (IPrestigeUpgrade)new PrestigeUpgrade(x.Id, factionType,
						new PrestigeResourceCost(factionType, x.Amount), x.Action)).ToArray());
			}

			SetupUpgrades(FactionType.Divinity,
				("X", 1000, new PrestigeUpgradeAction.UnlockUpgrade("X")),
				("Y", 1000, new PrestigeUpgradeAction.UnlockUpgrade("Y")));

			return prestigeUpgrades;
		}
	}
}