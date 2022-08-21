using UnityEngine;

namespace IdleFactions.Core
{
	public class GameInitializer : MonoBehaviour
	{
		public GameController GameController { get; private set; }

		private void Start()
		{
			GameController = new GameController(this, FindObjectOfType<UIController>());
		}

		private void Update()
		{
			GameController.Update(Time.deltaTime);
		}
	}
}