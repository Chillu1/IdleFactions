using System.Diagnostics.CodeAnalysis;
using IdleFactions.Core;

namespace IdleFactions
{
	[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
	public class GameController
	{
		private readonly UpgradeData _upgradeData;
		private readonly FactionData _factionData;

		private readonly IRevertController _revertController;
		private readonly FactionController _factionController;
		private readonly IResourceController _resourceController;

		public GameController(GameInitializer gameInitializer, UIController uiController)
		{
			_upgradeData = new UpgradeData();
			_factionData = new FactionData(_upgradeData);
			_revertController = new RevertController();
			_resourceController = new ResourceController();
			Upgrade.Setup(_revertController, _resourceController);
			Faction.Setup(_revertController, _resourceController);
			_factionController = new FactionController(_factionData);

			uiController.Setup(_resourceController, _factionController);

			NewGame();
		}

		public void Update(float delta)
		{
			_revertController.Update(delta);
			_factionController.Update(delta);
		}

		private void NewGame()
		{
			_factionController.GetFaction(FactionType.Creation)!.Unlock();

			_factionController.GetFaction(FactionType.Creation)!.ChangePopulation(1);
		}
	}
}