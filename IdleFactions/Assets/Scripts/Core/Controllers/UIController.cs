using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFactions
{
	public class UIController : MonoBehaviour
	{
		private const int MaxButtonUpgrades = 6; //TEMP

		private IResourceController _resourceController;
		private FactionController _factionController;

		private Button[] _factionTabButtons;

		private TMP_Text[] _resourceTexts;
		private Button[][] _factionUpgradeButtons;

		//Faction Tab
		private TMP_Text _factionType;
		private TMP_Text _factionBuyPopulationText;
		private Button[] _upgradeButtons;
		private TMP_Text[] _upgradeButtonTexts;
		private TMP_Text _population;
		private TMP_Text[] _needs;
		private TMP_Text[] _rates;

		private FactionType _currentFactionType;

		public void Setup(IResourceController resourceController, FactionController factionController)
		{
			_resourceController = resourceController;
			_factionController = factionController;
		}

		private void Start()
		{
			var canvas = GameObject.Find("Canvas").transform;

			var factions = canvas.Find("Factions");
			_factionTabButtons = factions.GetComponentsInChildren<Button>();
			for (int i = 0; i < _factionTabButtons.Length; i++)
			{
				var button = _factionTabButtons[i];
				var factionType = (FactionType)i + 1;
				button.GetComponentInChildren<TMP_Text>().text = factionType.ToString();
				button.onClick.AddListener(() => SwitchFactionTab(factionType));
				button.gameObject.SetActive(_factionController.GetFaction(factionType)?.IsDiscovered == true);
			}

			var factionTab = canvas.Find("FactionTab");
			_factionType = factionTab.Find("FactionType").GetComponent<TMP_Text>();

			_factionBuyPopulationText = factionTab.Find("BuyPopulation").GetComponentInChildren<TMP_Text>();
			factionTab.Find("BuyPopulation").GetComponent<Button>().onClick.AddListener(() =>
			{
				_factionController.GetFaction(_currentFactionType)?.TryBuyPopulation(1);
				UpdateFactionTabPopulationInfo();
			});
			factionTab.Find("ToggleGeneration").GetComponent<Button>().onClick.AddListener(() => _factionController
				.GetFaction(_currentFactionType)?.ToggleGeneration());

			var upgrades = factionTab.Find("Upgrades");
			int upgradesChildCount = upgrades.childCount;
			_upgradeButtons = new Button[upgradesChildCount];
			_upgradeButtonTexts = new TMP_Text[upgradesChildCount];
			for (int i = 0; i < upgradesChildCount; i++)
			{
				var upgradeButton = upgrades.GetChild(i).GetComponent<Button>();
				_upgradeButtons[i] = upgradeButton;
				_upgradeButtonTexts[i] = upgradeButton.GetComponentInChildren<TMP_Text>();
				int upgradeIndex = i;
				upgradeButton.onClick.AddListener(() =>
				{
					//TODO upgradeIndex will be wrong/not dynamic, unless we do some special logic in faction
					if (_factionController.GetFaction(_currentFactionType)?.TryBuyUpgrade(upgradeIndex) == true)
					{
						upgradeButton.interactable = false;
					}
				});
			}

			_population = factionTab.Find("Population").GetComponent<TMP_Text>();
			_needs = factionTab.Find("Needs").GetComponentsInChildren<TMP_Text>();
			_rates = factionTab.Find("Rates").GetComponentsInChildren<TMP_Text>();

			var factionResources = canvas.Find("FactionResources");

			int childCount = factionResources.childCount;
			_resourceTexts = new TMP_Text[childCount];
			_factionUpgradeButtons = new Button[childCount][];
			for (int i = 0; i < childCount; i++)
			{
				var factionResource = factionResources.GetChild(i);
				_resourceTexts[i] = factionResource.GetComponent<TMP_Text>();

				/*int j = i;
				for (int k = 0; k < MaxButtonUpgrades; k++)
				{
					//Log.Info(factionResource.GetChild(0).name + "_" + factionResource.GetChild(0).GetChild(0).name);
					_factionUpgradeButtons[i] = new Button[MaxButtonUpgrades];

					_factionUpgradeButtons[i][k] =
						factionResource.Find("Upgrades").Find("UpgradeButton (" + k + ")").GetComponent<Button>();

					var button = _factionUpgradeButtons[i][k];
					button.GetComponentInChildren<TMP_Text>().text = _factionController.GetFaction((FactionType)j + 1)?.GetUpgradeId(k);
					int k1 = k;
					button.onClick
						.AddListener(() =>
						{
							_factionController.GetFaction((FactionType)j + 1)?.TryBuyUpgrade(k1);
							button.interactable = false;
						});
				}*/
			}

			SwitchFactionTab(FactionType.Creation);
		}

		private void Update()
		{
			for (int i = 0; i < _resourceTexts.Length; i++)
			{
				var resourceText = _resourceTexts[i];
				resourceText.text = _resourceController.GetResource(i)?.ToString();
			}
		}

		public void UpdateTab(Faction faction)
		{
			_factionTabButtons[(int)faction.Type - 1].gameObject.SetActive(true);
		}

		private void SwitchFactionTab(FactionType type)
		{
			var faction = _factionController.GetFaction(type);
			if (faction == null)
				return;

			_currentFactionType = type;

			_factionType.text = type.ToString();

			for (int i = 0; i < _upgradeButtons.Length; i++)
			{
				_upgradeButtons[i].interactable = faction.GetUpgrade(i) is { Unlocked: true, Bought: false };
				_upgradeButtonTexts[i].text = faction.GetUpgradeId(i);
			}

			UpdateFactionTabInfo(faction);
		}

		private void UpdateFactionTabInfo(Faction faction)
		{
			_needs[0].text = "Generation: " + string.Join(", ", faction.ResourceNeeds.Generate.Select(r => r.Value.ToString()));
			_needs[1].text = "CreateCost: " + string.Join(", ", faction.ResourceNeeds.CreateCost.Select(r => r.Value.ToString()));
			_needs[2].text = faction.ResourceNeeds.GenerateCost != null
				? "GenerateCost: " + string.Join(", ", faction.ResourceNeeds.GenerateCost.Select(r => r.Value.ToString()))
				: "GenerateCost: None";
			_needs[3].text = faction.ResourceNeeds.LiveCost != null
				? "LiveCost: " + string.Join(", ", faction.ResourceNeeds.LiveCost.Select(r => r.Value.ToString()))
				: "LiveCost: None";

			UpdateFactionTabPopulationInfo();
			//TODO _rates
		}

		private void UpdateFactionTabPopulationInfo()
		{
			var faction = _factionController.GetFaction(_currentFactionType);
			if (faction == null)
				return;

			double multiplier = faction.GetPopulationCostMultiplier(1);
			string costs = string.Join(", ", faction.ResourceNeeds.CreateCost.Select(r =>
				(r.Value.Value * multiplier).ToString("F1") + " " + r.Key));
			_factionBuyPopulationText.text = "Buy 1 population: " + costs;
		}
	}
}