using IdleFactions.Core;
using UnityEditor;
using UnityEngine;

namespace IdleFactions.Editor
{
	[CustomEditor(typeof(GameInitializer))]
	public class DevPanel : UnityEditor.Editor
	{
		private GameController _gameController;
		private ResourceType _selectedResourceType = ResourceType.Light;
		private double _resourceAmount = 1000;
		private FactionType _selectedFactionType = FactionType.Divinity;
		private double _populationAmount = 100;
		private string _fileName = StateController.DefaultSaveFileName;

		private void OnEnable()
		{
			_gameController = (target as GameInitializer)?.GameController;
		}

		public override void OnInspectorGUI()
		{
			_gameController ??= (target as GameInitializer)?.GameController; //Fixes the second click bug

			serializedObject.Update();

			if (GUILayout.Button("Test"))
			{
			}

			//Selectable GUILayout enum ResourceType
			_selectedResourceType = (ResourceType)EditorGUILayout.EnumPopup("Resource Type", _selectedResourceType);
			_resourceAmount = EditorGUILayout.DoubleField("Amount", _resourceAmount);
			if (GUILayout.Button("Give Resource"))
			{
				_gameController.ResourceController.Add(_selectedResourceType, _resourceAmount);
			}

			if (GUILayout.Button("HeadStart"))
			{
				_gameController.ResourceController.Add(ResourceType.Essence, 10000);
				_gameController.ResourceController.Add(ResourceType.Light, 10000);
				_gameController.ResourceController.Add(ResourceType.Dark, 10000);
			}

			_selectedFactionType = (FactionType)EditorGUILayout.EnumPopup("Faction Type", _selectedFactionType);
			_populationAmount = EditorGUILayout.DoubleField("Amount", _populationAmount);
			if (GUILayout.Button("Buy Population"))
			{
				_gameController.FactionController.Get(_selectedFactionType).ChangePopulation(_populationAmount);
			}

			if (GUILayout.Button("New Game"))
				_gameController.NewGame();

			GUILayout.Label("Save/Load file name");
			GUILayout.BeginHorizontal();
			_fileName = GUILayout.TextArea(_fileName, GUILayout.MaxWidth(200));
			if (GUILayout.Button("Save", GUILayout.MaxWidth(100)))
			{
				if (_fileName != "")
					_gameController.StateController.Save(_fileName);
				else
					_gameController.StateController.Save();
			}

			if (GUILayout.Button("Load", GUILayout.MaxWidth(100)))
			{
				if (_fileName != "")
					_gameController.StateController.Load(_fileName);
				else
					_gameController.StateController.Load();
			}

			GUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}
	}
}