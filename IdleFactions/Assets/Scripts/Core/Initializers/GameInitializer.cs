using UnityEngine;

namespace IdleFactions.Core
{
	public class GameInitializer : MonoBehaviour
	{
		public GameController GameController { get; private set; }

		private void Start()
		{
			var dataController = FindObjectOfType<DataInitializer>().DataController;
			GameController = new GameController(this, dataController, GetComponent<UIController>());
		}

		private void Update()
		{
			GameController.Update(Time.deltaTime);
		}

		private void OnApplicationQuit()
		{
			//TODO Also on moving to main menu
			GameController.CleanUp();
		}
	}
}