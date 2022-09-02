using System.Diagnostics.CodeAnalysis;
using IdleFactions.Core;

namespace IdleFactions
{
	[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
	public class GameController
	{
		private readonly UpgradeData _upgradeData;
		private readonly FactionData _factionData;

		private readonly FactionController _factionController;
		private readonly IResourceController _resourceController;

		public GameController(GameInitializer gameInitializer, UIController uiController)
		{
			_upgradeData = new UpgradeData();
			_factionData = new FactionData(_upgradeData);
			_resourceController = new ResourceController();
			Upgrade.Setup(_resourceController);
			Faction.Setup(_resourceController);
			_factionController = new FactionController(_factionData);

			_resourceController.Add(ResourceType.Light, 1);
			_resourceController.Add(ResourceType.Dark, 1);

			_factionController.GetFaction(FactionType.Divinity)?.Unlock();
			_factionController.GetFaction(FactionType.Void)?.Unlock();
			_factionController.GetFaction(FactionType.Ocean)?.Unlock();
			_factionController.GetFaction(FactionType.Nature)?.Unlock();
			_factionController.GetFaction(FactionType.Human)?.Unlock();

			_factionController.GetFaction(FactionType.Divinity)?.ChangePopulation(1);
			_factionController.GetFaction(FactionType.Void)?.ChangePopulation(1);

			uiController.Setup(_resourceController, _factionController);
		}

		public void Update(float delta)
		{
			_factionController.Update(delta);
		}
	}
}