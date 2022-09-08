using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IdleFactions
{
	public class DataController
	{
		public static Version Version { get; }

		/// <summary>
		///		For development, in case we load through gameplayscene
		/// </summary>
		public bool MainMenuLoad { get; private set; }

		public bool LoadSavedGame { get; private set; }

		//In case of load game
		public string SaveName { get; private set; }

		public ProgressionData ProgressionData { get; }
		private UpgradeData UpgradeData { get; }
		public FactionData FactionData { get; }

		static DataController()
		{
			Version = new Version(Application.version);
			//Log.AddCategory("idlearpg", LogLevel.Verbose);
		}

		public DataController()
		{
			if (SceneManager.GetActiveScene().name == "MainMenu")
				MainMenuLoad = true;

			ProgressionData = new ProgressionData();
			UpgradeData = new UpgradeData();
			FactionData = new FactionData(UpgradeData);
		}

		public void NewGame()
		{
			LoadSavedGame = false;
		}

		public void LoadGame(string saveName)
		{
			LoadSavedGame = true;
			SaveName = saveName;
		}
	}
}