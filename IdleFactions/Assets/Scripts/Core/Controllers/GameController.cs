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
		private readonly ProgressionData _progressionData;

		public GameController(GameInitializer gameInitializer, UIController uiController)
		{
			_upgradeData = new UpgradeData();
			_factionData = new FactionData(_upgradeData);
			_revertController = new RevertController();
			_resourceController = new ResourceController();
			Upgrade.Setup(_revertController, _resourceController);
			Faction.Setup(_revertController, _resourceController);
			_factionController = new FactionController(_factionData);
			_progressionData = new ProgressionData(_factionController);

			_resourceController.Added += _progressionData.OnAddResource;

			NewGame();

			uiController.Setup(_resourceController, _factionController);
			Faction.Discovered += uiController.UpdateTab;
			//Faction.Discovered += uiController.ShowNotification;
		}

		public void Update(float delta)
		{
			_revertController.Update(delta);
			_factionController.Update(delta);
			_progressionData.Update(delta);
		}

		public void CleanUp()
		{
			Faction.CleanUp();
		}

		private void NewGame()
		{
			_factionController.GetFaction(FactionType.Creation)!.Discover();
			_factionController.GetFaction(FactionType.Creation)!.Unlock();

			_factionController.GetFaction(FactionType.Creation)!.ChangePopulation(1);
		}
	}
}