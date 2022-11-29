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

		private UpgradeData UpgradeData { get; }
		public ProgressionData ProgressionData { get; }
		public PrestigeUpgradesData PrestigeUpgradesData { get; }
		public PrestigeProgressionData PrestigeProgressionData { get; }
		public PrestigeResourceData PrestigeResourceData { get; }
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

			UpgradeData = new UpgradeData();
			ProgressionData = new ProgressionData(UpgradeData);
			PrestigeUpgradesData = new PrestigeUpgradesData();
			PrestigeProgressionData = new PrestigeProgressionData();
			PrestigeResourceData = new PrestigeResourceData();
			FactionData = new FactionData(UpgradeData);
		}

		public void PrepareNewGame()
		{
			LoadSavedGame = false;
		}

		public void PrepareLoadGame(string saveName)
		{
			LoadSavedGame = true;
			SaveName = saveName;
		}
	}
}