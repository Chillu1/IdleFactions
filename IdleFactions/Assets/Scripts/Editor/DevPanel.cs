using IdleFactions.Core;
using UnityEditor;
using UnityEngine;

namespace IdleFactions.Editor
{
	[CustomEditor(typeof(GameInitializer))]
	public class DevPanel : UnityEditor.Editor
	{
		private GameController _gameController;
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

			if (GUILayout.Button("HeadStart"))
			{
				_gameController.ResourceController.Add(ResourceType.Essence, 10000);
				_gameController.ResourceController.Add(ResourceType.Light, 10000);
				_gameController.ResourceController.Add(ResourceType.Dark, 10000);
			}

			if (GUILayout.Button("NewGame"))
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