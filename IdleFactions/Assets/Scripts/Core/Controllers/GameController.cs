using System.Diagnostics.CodeAnalysis;
using IdleFactions.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IdleFactions
{
	[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
	public class GameController
	{
		public bool IsPaused => _manualPause;

		private bool _manualPause;

		private readonly IRevertController _revertController;
		private readonly FactionController _factionController;
		private readonly ResourceController _resourceController;
		private readonly ProgressionController _progressionController;
		public StateController StateController { get; }

		public GameController(GameInitializer gameInitializer, DataController dataController, UIController uiController)
		{
			_revertController = new RevertController();
			_resourceController = new ResourceController();
			Upgrade.Setup(_revertController, _resourceController);
			Faction.Setup(_revertController, _resourceController);
			_factionController = new FactionController(dataController.FactionData);
			_progressionController = new ProgressionController(dataController.ProgressionData, _factionController, uiController);
			StateController = new StateController(_resourceController, _factionController, _progressionController);

			_resourceController.Added += _progressionController.OnAddResource;

			if (!dataController.LoadSavedGame)
				NewGame();
			else
				LoadGame(dataController.SaveName);

			uiController.Setup(_resourceController, _factionController);
			Faction.Discovered += uiController.UpdateTab;
			//Faction.Discovered += uiController.ShowNotification;
		}

		public void Update(float delta)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				StateController.SaveCurrent();
				CleanUp();
				SceneManager.LoadScene("MainMenu");
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				_manualPause = !_manualPause;
				Log.Verbose("Manual pause: " + _manualPause);
			}

			if (IsPaused)
				return;

			_resourceController.Update(delta);
			_revertController.Update(delta);
			_factionController.Update(delta);
			_progressionController.Update(delta);
		}

		public void CleanUp()
		{
			Faction.CleanUp();
		}

		public void NewGame()
		{
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