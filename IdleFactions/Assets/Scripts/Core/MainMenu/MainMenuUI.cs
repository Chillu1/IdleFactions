using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IdleFactions
{
	public class MainMenuUI : MonoBehaviour
	{
		private Button _newGameButton;
		private Button _loadGameButton;
		private Button _quitButton;

		private Transform[] _saves;

		private void Start()
		{
			var canvas = FindObjectOfType<Canvas>().transform;
			_newGameButton = canvas.Find("NewGame").GetComponent<Button>();
			_loadGameButton = canvas.Find("LoadGame").GetComponent<Button>();
			_quitButton = canvas.Find("Quit").GetComponent<Button>();

			_newGameButton.onClick.AddListener(() =>
			{
				FindObjectOfType<DataInitializer>().DataController.PrepareNewGame();
				SceneManager.LoadScene("GameplayScene");
			});
			_loadGameButton.onClick.AddListener(() => OpenLoadMenu());

			_quitButton.onClick.AddListener(() =>
			{
#if UNITY_EDITOR
				EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			});

			var savesTransform = canvas.Find("Saves");
			_saves = new Transform[StateController.MaxSaves];
			for (int i = 0; i < StateController.MaxSaves; i++)
			{
				_saves[i] = savesTransform.Find($"Save ({i})");
				_saves[i].gameObject.SetActive(false);
			}
		}

		public void OpenLoadMenu()
		{
			var dataInitializer = FindObjectOfType<DataInitializer>();

			StateController.SaveState[] saveStates = StateController.LoadDisplay();
			for (int i = 0; i < saveStates.Length; i++)
			{
				var saveState = saveStates[i];
				var saveUI = _saves[i];
				if (saveState.Equals(default(StateController.SaveState)))
				{
					saveUI.gameObject.SetActive(false);
					continue;
				}

				saveUI.gameObject.SetActive(true);
				string displayText = $"Name: {saveState.SaveName}\n";
				if (saveState.PlayTime > 3600)
					displayText += $"PlayTime: {saveState.PlayTime / 3600:F0}h {saveState.PlayTime % 3600 / 60:F1}m\n";
				else
					displayText += $"PlayTime: {saveState.PlayTime / 60:F0}m {saveState.PlayTime % 60:F0}s\n";

				if (saveState.SaveDate != default)
					displayText += $"Date: {saveState.SaveDate.ToString("HH:mm:ss dd/MM/yy")}\n";
				displayText += $"Version: {saveState.GameVersion}\n";
				displayText += $"SaveVersion: {saveState.SaveVersion}\n";
				displayText += $"Factions: {saveState.FactionCount}\n";
				displayText += $"Achievements: {saveState.AchievementsCount}\n";

				saveUI.GetComponentInChildren<TMP_Text>().text = displayText;

				var button = saveUI.GetComponent<Button>();
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() =>
				{
					dataInitializer.DataController.PrepareLoadGame(saveState.SaveName);
					SceneManager.LoadScene("GameplayScene");
				});
			}
		}
	}
}