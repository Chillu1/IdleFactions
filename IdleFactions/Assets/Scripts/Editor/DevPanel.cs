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
				//Pop0, 0+5+5, 10
				// 5, 10-5,
				//Pop0, 0+10, 10
				Log.Info(Faction.CalculateFormula(5), Faction.CalculateFormula(10) - Faction.CalculateFormula(5));
				Log.Info(Faction.CalculateFormula(0), Faction.CalculateFormula(10));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}