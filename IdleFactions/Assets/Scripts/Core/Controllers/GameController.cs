using UnityEngine;

namespace IdleFactions
{
	public class GameController : MonoBehaviour
	{
		private FactionData _factionData;

		private FactionController _factionController;
		private ResourceController _resourceController;

		private void Start()
		{
			_factionData = new FactionData();
			_resourceController = new ResourceController();
			Upgrade.Setup(_resourceController);
			Faction.Setup(_resourceController);
			_factionController = new FactionController(_factionData);

			_resourceController.Add(ResourceType.Light, 1000);
			_resourceController.Add(ResourceType.Dark, 1000);

			//TEMP
			_factionController.GetFaction(FactionType.Divinity)?.ChangePopulation(10);
			_factionController.GetFaction(FactionType.Void)?.ChangePopulation(10);
			//_factionController.GetFaction(FactionType.Nature)?.ChangePopulation(2);

			FindObjectOfType<UIController>().Setup(_resourceController, _factionController);
		}

		private void Update()
		{
			float delta = Time.deltaTime;

			_factionController.Update(delta);
		}
	}
}