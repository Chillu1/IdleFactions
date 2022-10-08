using System.Diagnostics.CodeAnalysis;
using IdleFactions.Core;

namespace IdleFactions
{
	[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
	public class GameController
	{
		public bool IsPaused => _manualPause || _internalPause;

		private bool _manualPause;
		private bool _internalPause;

		private readonly IRevertController _revertController;
		public FactionController FactionController { get; }
		public ResourceController ResourceController { get; }
		private readonly ProgressionController _progressionController;
		public StateController StateController { get; }

		public GameController(GameInitializer gameInitializer, DataController dataController, UIController uiController)
		{
			_revertController = new RevertController();
			ResourceController = new ResourceController();
			Upgrade.Setup(_revertController, ResourceController);
			Faction.Setup(_revertController, ResourceController);
			FactionController = new FactionController(dataController.FactionData);
			_progressionController = new ProgressionController(dataController.ProgressionData, FactionController, uiController);
			StateController = new StateController(ResourceController, FactionController, _progressionController);

			ResourceController.Added += _progressionController.OnAddResource;
			Faction.PopulationAdded += _progressionController.OnAddFactionPopulation;

			if (!dataController.LoadSavedGame)
				NewGame();
			else
				LoadGame(dataController.SaveName);

			uiController.Setup(this, ResourceController, FactionController, StateController);
			Faction.Discovered += uiController.UpdateTab;
			Faction.Discovered += uiController.DisplayNotification;
			Upgrade.Unlocked += uiController.DisplayNotification;
			Upgrade.Bought += _ => uiController.UpdateFactionTabInfo();
		}

		public void Update(float delta)
		{
			if (IsPaused)
				return;

			StateController.Update(delta);

			ResourceController.Update(delta);
			_revertController.Update(delta);
			FactionController.Update(delta);
			_progressionController.Update(delta);
		}

		public void PlayerPause(bool state) => _manualPause = state;
		public void InternalPause(bool state) => _internalPause = state;

		public static void CleanUp()
		{
			Faction.CleanUp();
			Upgrade.CleanUp();
		}

		public void NewGame()
		{
			StateController.SetNewGameSave();
			FactionController.Get(FactionType.Creation)!.Discover();
			FactionController.Get(FactionType.Creation)!.Unlock();

			FactionController.Get(FactionType.Creation)!.ChangePopulation(1);
		}

		private void LoadGame(string saveName)
		{
			StateController.Load(saveName);
		}
	}
}