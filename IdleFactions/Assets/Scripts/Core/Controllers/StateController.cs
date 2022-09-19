using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IdleFactions
{
	public class StateController //TODO Not part of gamecontroller?
	{
		public const int MaxSaves = 8; //TODO TEMP
		private static readonly Version saveVersion = new Version(0, 1, 0, 0);
		public static readonly string DefaultSavePath = Application.persistentDataPath + "/saves/";
		public const string DefaultSaveFileName = "save.json";
		private string _currentSaveName;
		private double _filePlayTime;
		private float _timer;
		private readonly double _saveInterval = 120;

		private readonly IResourceController _resourceController;
		private readonly IFactionController _factionController;
		private readonly ProgressionController _progressionController;

		public StateController(IResourceController resourceController, IFactionController factionController,
			ProgressionController progressionController)
		{
			_resourceController = resourceController;
			_factionController = factionController;
			_progressionController = progressionController;
		}

		public void Update(float deltaTime)
		{
			_timer += deltaTime;
			if (_timer >= _saveInterval)
			{
				_timer = 0;
				SaveCurrent();
			}
		}

		public void SetNewGameSave()
		{
			int i = 1;
			string saveName = DefaultSaveFileName;
			while (File.Exists(DefaultSavePath + saveName))
			{
				saveName = DefaultSaveFileName.Substring(0, DefaultSaveFileName.IndexOf(".json", StringComparison.Ordinal)) + i + ".json";
				i++;
			}

			_currentSaveName = saveName;
		}

		public bool SaveCurrent() => Save(_currentSaveName);

		public bool Save() => Save(DefaultSaveFileName);

		// TODO "" Ugh not ideal, App.DataPath can't be compile time constant, but we want to feed a directory as well
		public bool Save(string fileName, string directory = "")
		{
			if (string.IsNullOrWhiteSpace(directory))
				directory = DefaultSavePath;

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = directory + fileName;
			double playTime = Time.timeSinceLevelLoadAsDouble;

			if (File.Exists(fullSavePath))
			{
				using var streamReader = new StreamReader(fullSavePath);
				using var reader = new JsonTextReader(streamReader);

				var saveData = JObject.Load(reader);
				reader.Close();

				if (_filePlayTime == 0d)
					_filePlayTime = saveData.Value<double>("PlayTime");
			}

			using var streamWriter = new StreamWriter(fullSavePath, false);
			if (SaveInternal(streamWriter, fileName.Remove(fileName.IndexOf(".json", StringComparison.Ordinal)), _filePlayTime + playTime))
			{
				Log.Verbose("Save successful");
				return true;
			}

			return false;
		}

		/// <summary>
		///		Don't use directly, unless unit/bench test
		/// </summary>
		public bool SaveInternal(TextWriter textWriter, string saveName, double playTime)
		{
			try
			{
				using var writer = new JsonTextWriter(textWriter);
				writer.Formatting = Formatting.Indented;

				writer.WriteStartObject();
				writer.WritePropertyName("SaveName");
				writer.WriteValue(saveName);
				writer.WritePropertyName("GameVersion");
				writer.WriteValue(DataController.Version.ToString());
				writer.WritePropertyName("SaveVersion");
				writer.WriteValue(saveVersion.ToString());
				writer.WritePropertyName("PlayTime");
				writer.WriteValue(Math.Round(playTime, 1));
				writer.WritePropertyName("SaveDate");
				writer.WriteValue(DateTime.Now);

				_resourceController.Save(writer);
				_factionController.Save(writer);
				_progressionController.Save(writer);

				writer.WriteEndObject();

				writer.Close();
			}
			catch (Exception e)
			{
				Log.Error("Fatal. Invalid save data?");
				Log.Exception(e);
				return false;
			}

			return true;
		}

		public bool Load() => Load(DefaultSaveFileName);

		// TODO Same as Save()
		public bool Load(string fileName, string directory = "")
		{
			if (string.IsNullOrWhiteSpace(directory))
				directory = DefaultSavePath;

			if (!Directory.Exists(directory))
			{
				Log.Error($"Load directory {directory} does not exist");
				return false;
			}

			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = directory + fileName;

			if (!File.Exists(fullSavePath))
			{
				Log.Error($"File {fileName} does not exist");
				return false;
			}

			using var streamReader = new StreamReader(fullSavePath);
			if (LoadInternal(streamReader, fileName))
			{
				Log.Verbose("Load successful");
				return true;
			}

			return false;
		}

		/// <summary>
		///		Don't use directly, unless unit/bench test
		/// </summary>
		public bool LoadInternal(TextReader textReader, string fileName)
		{
			try
			{
				using var reader = new JsonTextReader(textReader);

				var saveData = JObject.Load(reader);
				reader.Close();
				string version = saveData.Value<string>("GameVersion");
				if (version != DataController.Version.ToString())
					Log.Warning("Saved game version is different from current game version.");
				string readSaveVersion = saveData.Value<string>("SaveVersion");
				if (readSaveVersion != saveVersion.ToString())
					Log.Warning("Saved save version is different from current save version. Might need to update?");

				_resourceController.Load(saveData);
				_factionController.Load(saveData);
				_progressionController.Load(saveData);

				_currentSaveName = saveData.Value<string>("SaveName");
				if (string.IsNullOrEmpty(_currentSaveName))
					_currentSaveName = fileName; //.Remove(fileName.IndexOf(".json", StringComparison.Ordinal));
			}
			catch (Exception e)
			{
				Log.Error("Invalid/corrupt save");
				Log.Exception(e);
				return false;
			}

			return true;
		}

		public struct SaveState
		{
			public string SaveName;
			public string GameVersion;
			public string SaveVersion;
			public double PlayTime;
			public DateTime SaveDate;
			public int FactionCount;
			public int AchievementsCount;
		}

		public static SaveState[] LoadDisplay(string directory = "")
		{
			if (directory == "")
				directory = DefaultSavePath;

			if (!Directory.Exists(directory))
			{
				Log.Error($"Load directory {directory} does not exist");
				return null;
			}

			string[] saveFiles = Directory.GetFiles(directory, "*.json");
			if (saveFiles.Length == 0)
			{
				Log.Error("No save files found");
				return null;
			}

			for (int i = 0; i < saveFiles.Length; i++)
				saveFiles[i] = Path.GetFileName(saveFiles[i]);

			SaveState[] saveData = new SaveState[saveFiles.Length];
			for (int i = 0; i < saveFiles.Length; i++)
				saveData[i] = LoadSingleDisplay(saveFiles[i], directory);

			return saveData;
		}

		private static SaveState LoadSingleDisplay(string fileName, string directory)
		{
			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = directory + fileName;

			try
			{
				using var streamReader = new StreamReader(fullSavePath);
				using var reader = new JsonTextReader(streamReader);

				var saveData = JObject.Load(reader);
				reader.Close();

				string saveName = saveData.Value<string>("SaveName");
				if (saveName == null)
					saveName = fileName.Remove(fileName.IndexOf(".json", StringComparison.Ordinal));
				string version = saveData.Value<string>("GameVersion");
				if (version != DataController.Version.ToString())
					Log.Warning("Saved game version is different from current game version.");
				string readSaveVersion = saveData.Value<string>("SaveVersion");
				if (readSaveVersion != saveVersion.ToString())
					Log.Warning("Saved save version is different from current save version. Might need to update?");

				var saveState = new SaveState();
				saveState.SaveName = saveName;
				saveState.GameVersion = version;
				saveState.SaveVersion = readSaveVersion;
				saveState.PlayTime = saveData.Value<double>("PlayTime");
				saveState.SaveDate = saveData.Value<DateTime>("SaveDate");

				var factions = saveData.Value<JArray>(FactionController.JSONKey);
				if (factions != null)
					saveState.FactionCount = factions.Count;

				Log.Verbose("Display load successful");
				return saveState;
			}
			catch (Exception e)
			{
				Log.Error("Invalid/corrupt save");
				Log.Exception(e);
			}

			return default;
		}
	}
}