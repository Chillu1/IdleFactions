using IdleFactions.Core;
using UnityEditor;
using UnityEngine;

namespace IdleFactions.Editor
{
	[CustomEditor(typeof(GameInitializer))]
	public class DevPanel : UnityEditor.Editor
	{
		private GameController _gameController;

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

			serializedObject.ApplyModifiedProperties();
		}
	}
}