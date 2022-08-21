using IdleFactions.Core;

namespace IdleFactions
{
	public class GameController
	{
		private readonly FactionData _factionData;

		private readonly FactionController _factionController;
		private readonly ResourceController _resourceController;

		public GameController(GameInitializer gameInitializer, UIController uiController)
		{
			_factionData = new FactionData();
			_resourceController = new ResourceController();
			Upgrade.Setup(_resourceController);
			Faction.Setup(_resourceController);
			_factionController = new FactionController(_factionData);

			_resourceController.Add(ResourceType.Light, 1000);
			_resourceController.Add(ResourceType.Dark, 1000);

			_factionController.GetFaction(FactionType.Divinity)?.Unlock();
			_factionController.GetFaction(FactionType.Void)?.Unlock();

			//TEMP
			_factionController.GetFaction(FactionType.Divinity)?.ChangePopulation(10);
			_factionController.GetFaction(FactionType.Void)?.ChangePopulation(10);
			//_factionController.GetFaction(FactionType.Nature)?.ChangePopulation(2);

			uiController.Setup(_resourceController, _factionController);
		}

		public void Update(float delta)
		{
			_factionController.Update(delta);
		}
	}
}