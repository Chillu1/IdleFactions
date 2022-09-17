using System.Diagnostics.CodeAnalysis;
using IdleFactions.Core;

namespace IdleFactions
{
	[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
	public class GameController
	{
		public bool IsPaused => _manualPause;

		private bool _manualPause;

		private readonly IRevertController _revertController;
		private readonly FactionController _factionController;
		public ResourceController ResourceController { get; }
		private readonly ProgressionController _progressionController;
		public StateController StateController { get; }

		public GameController(GameInitializer gameInitializer, DataController dataController, UIController uiController)
		{
			_revertController = new RevertController();
			ResourceController = new ResourceController();
			Upgrade.Setup(_revertController, ResourceController);
			Faction.Setup(_revertController, ResourceController);
			_factionController = new FactionController(dataController.FactionData);
			_progressionController = new ProgressionController(dataController.ProgressionData, _factionController, uiController);
			StateController = new StateController(ResourceController, _factionController, _progressionController);

			ResourceController.Added += _progressionController.OnAddResource;

			if (!dataController.LoadSavedGame)
				NewGame();
			else
				LoadGame(dataController.SaveName);

			uiController.Setup(this, ResourceController, _factionController, StateController);
			Faction.Discovered += uiController.UpdateTab;
			//Faction.Discovered += uiController.ShowNotification;
		}

		public void Update(float delta)
		{
			if (IsPaused)
				return;

			StateController.Update(delta);

			ResourceController.Update(delta);
			_revertController.Update(delta);
			_factionController.Update(delta);
			_progressionController.Update(delta);
		}

		public void Pause(bool state) => _manualPause = state;

		public static void CleanUp()
		{
			Faction.CleanUp();
		}

		public void NewGame()
		{
			StateController.SetNewGameSave();
			_factionController.Get(FactionType.Creation)!.Discover();
			_factionController.Get(FactionType.Creation)!.Unlock();

			_factionController.Get(FactionType.Creation)!.ChangePopulation(1);
		}

		private void LoadGame(string saveName)
		{
			StateController.Load(saveName);
		}
	}
}