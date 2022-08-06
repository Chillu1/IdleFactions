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
            Faction.Setup(_resourceController);
            _factionController = new FactionController(_factionData);
            
            //TEMP
            _factionController.GetFaction(FactionType.Divinity)?.ChangePopulation(13);
            _factionController.GetFaction(FactionType.Ocean)?.ChangePopulation(7);
            _factionController.GetFaction(FactionType.Nature)?.ChangePopulation(2);
            
            FindObjectOfType<UIController>().Setup(_resourceController);
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            
            _factionController.Update(delta);
        }
    }
}