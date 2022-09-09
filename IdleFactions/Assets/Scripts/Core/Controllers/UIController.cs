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
		private IFactionController _factionController;

		private Image _background;

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

		private int _currentPopulationAmount = 1;
		private PurchaseType _currentPurchaseType;
		private Faction _currentFaction;
		private FactionType _currentFactionType;

		public void Setup(IResourceController resourceController, IFactionController factionController)
		{
			_resourceController = resourceController;
			_factionController = factionController;
		}

		private void Start()
		{
			var canvas = GameObject.Find("Canvas").transform;

			_background = canvas.Find("Background").GetComponent<Image>();

			var factions = canvas.Find("Factions");
			_factionTabButtons = factions.GetComponentsInChildren<Button>();
			for (int i = 0; i < _factionTabButtons.Length; i++)
			{
				var button = _factionTabButtons[i];
				var factionType = (FactionType)i + 1;
				button.GetComponentInChildren<TMP_Text>().text = factionType.ToString();
				button.onClick.AddListener(() => SwitchFactionTab(factionType));
				button.gameObject.SetActive(_factionController.Get(factionType)?.IsDiscovered == true);
			}

			var factionTab = canvas.Find("FactionTab");
			_factionType = factionTab.Find("FactionType").GetComponent<TMP_Text>();

			_factionBuyPopulationText = factionTab.Find("BuyPopulation").GetComponentInChildren<TMP_Text>();
			var populationAmount = factionTab.Find("PopulationAmount");
			var populationAmountText = populationAmount.GetComponentInChildren<TMP_Text>();
			populationAmount.GetComponent<Button>().onClick.AddListener(() =>
			{
				//TODO 10%, 50%, 100%
				_currentPopulationAmount *= 10;
				if (_currentPopulationAmount > 100)
					_currentPopulationAmount = 1;
				populationAmountText.text = _currentPopulationAmount.ToString();
			});
			factionTab.Find("BuyPopulation").GetComponent<Button>().onClick.AddListener(() =>
			{
				_currentFaction?.TryBuyPopulation(_currentPopulationAmount);
				//_currentFaction?.TryBuyPopulation(_currentPurchaseType);
				UpdateFactionTabPopulationInfo();
			});
			factionTab.Find("ToggleGeneration").GetComponent<Button>().onClick.AddListener(() => _currentFaction?.ToggleGeneration());

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
					if (_currentFaction?.TryBuyUpgrade(upgradeIndex) == true)
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
			UpdateFactionTabPopulation();
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
			_currentFaction = _factionController.Get(type);
			if (_currentFaction == null)
				return;

			_background.sprite = GetFactionBackground(type);

			_factionType.text = type.ToString();

			UpdateFactionTabInfo();
		}

		private void UpdateFactionTabInfo() //TODO On unlock upgrade, update buttons
		{
			for (int i = 0; i < _upgradeButtons.Length; i++)
			{
				_upgradeButtons[i].interactable = _currentFaction.GetUpgrade(i) is { Unlocked: true, Bought: false };
				_upgradeButtonTexts[i].text = _currentFaction.GetUpgradeId(i);
			}

			_needs[0].text = "Generation: " +
			                 string.Join(", ", _currentFaction.FactionResources.Generate.Select(r => r.Value.ToString()));
			if (_currentFaction.FactionResources.GenerateAdded != null && _currentFaction.FactionResources.GenerateAdded.Count > 0)
				_needs[0].text += ". Added: " +
				                  string.Join(", ", _currentFaction.FactionResources.GenerateAdded.Select(r => r.Value.ToString()));

			_needs[1].text = "CreateCost: " +
			                 string.Join(", ", _currentFaction.FactionResources.CreateCost.Select(r => r.Value.ToString()));

			_needs[2].text = _currentFaction.FactionResources.GenerateCost != null
				? "GenerateCost: " + string.Join(", ", _currentFaction.FactionResources.GenerateCost.Select(r => r.Value.ToString()))
				: "GenerateCost: None";
			if (_currentFaction.FactionResources.GenerateCostAdded != null && _currentFaction.FactionResources.GenerateCostAdded.Count > 0)
				_needs[2].text += ". Added: " +
				                  string.Join(", ", _currentFaction.FactionResources.GenerateCostAdded.Select(r => r.Value.ToString()));

			_needs[3].text = _currentFaction.FactionResources.LiveCost != null
				? "LiveCost: " + string.Join(", ", _currentFaction.FactionResources.LiveCost.Select(r => r.Value.ToString()))
				: "LiveCost: None";
			if (_currentFaction.FactionResources.LiveCostAdded != null && _currentFaction.FactionResources.LiveCostAdded.Count > 0)
				_needs[3].text += ". Added: " +
				                  string.Join(", ", _currentFaction.FactionResources.LiveCostAdded.Select(r => r.Value.ToString()));

			UpdateFactionTabPopulationInfo();
			//TODO _rates
		}

		private void UpdateFactionTabPopulationInfo()
		{
			if (_currentFaction == null)
				return;

			UpdateFactionTabPopulation();

			double multiplier = _currentFaction.GetPopulationCostMultiplier(_currentPopulationAmount);
			string costs = string.Join(", ", _currentFaction.FactionResources.CreateCost.Select(r =>
				(r.Value.Value * multiplier).ToString("F1") + " " + r.Key));
			_factionBuyPopulationText.text = $"Buy {_currentPopulationAmount} population: {costs}";
		}

		private void UpdateFactionTabPopulation()
		{
			_population.color = Colors.GetColor(_currentFaction.PopulationValueRate);
			_population.text = "Population: " + _currentFaction.Population.ToString("F1");
		}

		private static Sprite GetFactionBackground(FactionType type)
		{
			return Resources.Load<Sprite>("Textures/Faction/Backgrounds/" + type);
		}

		private static Sprite GetFactionIcon(FactionType type)
		{
			return Resources.Load<Sprite>("Textures/Faction/Icons/" + type);
		}
	}
}