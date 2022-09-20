using System.Linq;
using IdleFactions.Behaviours;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IdleFactions
{
	public class UIController : MonoBehaviour
	{
		private const int MaxButtonUpgrades = 6; //TEMP

		private GameController _gameController;
		private IResourceController _resourceController;
		private IFactionController _factionController;
		private StateController _stateController;

		private Image _background;

		private Button[] _factionTabButtons;
		private Image[] _factionTabImages;

		private TMP_Text[] _resourceTexts;
		private Button[][] _factionUpgradeButtons;

		//Faction Tab
		private TMP_Text _factionType;
		private TMP_Text _factionBuyPopulationText;
		private TMP_Text _factionPopulationCostText;
		private Button[] _upgradeButtons;
		private Image[] _upgradeImages;
		private TMP_Text[] _upgradeButtonTexts;
		private TMP_Text _population;
		private TMP_Text[] _needs;
		private TMP_Text[] _rates;

		//Notification
		private GameObject _notificationGo;
		private TMP_Text _notificationText;

		//Hover
		private GameObject _hoverPanelGo;
		private TMP_Text _hoverPanelNameText;
		private TMP_Text _hoverPanelEffectsText;
		private TMP_Text _hoverPanelCostsText;

		//EscPanel
		private GameObject _escPanelGo;

		private GameObject _testVersionPanel;

		private int _currentPopulationAmount = 1;
		private PurchaseType _currentPurchaseType;
		private Faction _currentFaction;
		private FactionType _currentFactionType;


		public void Setup(GameController gameController, IResourceController resourceController, IFactionController factionController,
			StateController stateController)
		{
			_gameController = gameController;
			_resourceController = resourceController;
			_factionController = factionController;
			_stateController = stateController;
		}

		private void Start()
		{
			var canvas = GameObject.Find("Canvas").transform;

			_background = canvas.Find("Background").GetComponent<Image>();

			var factions = canvas.Find("Factions");
			_factionTabButtons = factions.GetComponentsInChildren<Button>();
			_factionTabImages = factions.GetComponentsInChildren<Image>();
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
			_factionPopulationCostText = factionTab.Find("PopulationCost").GetComponentInChildren<TMP_Text>();
			var populationAmount = factionTab.Find("PopulationAmount");
			var populationAmountText = populationAmount.GetComponentInChildren<TMP_Text>();
			populationAmount.GetComponent<Button>().onClick.AddListener(() =>
			{
				//TODO, 10, 100, 10%, 50%, 100%
				//_currentPopulationAmount *= 10;
				//if (_currentPopulationAmount > 100)
				//	_currentPopulationAmount = 1;
				_currentPopulationAmount = 1;
				populationAmountText.text = _currentPopulationAmount.ToString();
				UpdateFactionTabPopulationInfo();
			});
			factionTab.Find("BuyPopulation").GetComponent<Button>().onClick.AddListener(() =>
			{
				_currentFaction?.TryBuyPopulation(_currentPopulationAmount);
				//_currentFaction?.TryBuyPopulation(_currentPurchaseType);
				UpdateFactionTabPopulationInfo();
			});
			factionTab.Find("ToggleGeneration").GetComponent<Button>().onClick.AddListener(() => _currentFaction?.ToggleGeneration());

			var upgrades = factionTab.Find("Upgrades").Find("Upgrades");
			int upgradesChildCount = upgrades.childCount;
			_upgradeButtons = new Button[upgradesChildCount];
			_upgradeImages = upgrades.GetComponentsInChildren<Image>();
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

			_notificationGo = canvas.Find("NotificationPanel").gameObject;
			_notificationText = _notificationGo.GetComponentInChildren<TMP_Text>();
			_notificationText.text = "";
			_notificationGo.SetActive(false);

			_hoverPanelGo = canvas.Find("HoverPanel").gameObject;
			_hoverPanelNameText = _hoverPanelGo.transform.Find("Name").GetComponent<TMP_Text>();
			_hoverPanelEffectsText = _hoverPanelGo.transform.Find("Effects").GetComponent<TMP_Text>();
			_hoverPanelCostsText = _hoverPanelGo.transform.Find("Costs").GetComponent<TMP_Text>();
			_hoverPanelGo.SetActive(false);

			_escPanelGo = canvas.Find("EscPanel").gameObject;
			var escPanelBackground = _escPanelGo.transform.Find("Background");
			escPanelBackground.transform.Find("Resume").GetComponent<Button>().onClick.AddListener(() =>
			{
				_gameController.Pause(false);
				_escPanelGo.SetActive(false);
			});
			escPanelBackground.transform.Find("MainMenu").GetComponent<Button>().onClick.AddListener(() =>
			{
				_stateController.SaveCurrent();
				GameController.CleanUp();
				SceneManager.LoadScene("MainMenu");
			});
			escPanelBackground.transform.Find("Quit").GetComponent<Button>().onClick.AddListener(() =>
			{
				_stateController.SaveCurrent();
				GameController.CleanUp();
				Application.Quit();
			});
			_escPanelGo.SetActive(false);

			_testVersionPanel = canvas.Find("TestVersionPanel").gameObject;
			_testVersionPanel.GetComponentInChildren<Button>().onClick.AddListener(() => _testVersionPanel.SetActive(false));
			_testVersionPanel.SetActive(false);

			SwitchFactionTab(FactionType.Creation);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				_escPanelGo.SetActive(!_escPanelGo.activeSelf);
				_gameController.Pause(_escPanelGo.activeSelf);
			}

			if (_gameController.IsPaused)
				return;

			UpdateFactionTabPopulation();
			int i = 0;
			foreach (string resourceString in _resourceController.GetResourceStrings())
			{
				_resourceTexts[i].text = resourceString;
				i++;
			}
		}

		private void FixedUpdate()
		{
			if (Time.frameCount % 10 == 0)
				UpdateResourceRates();
		}

		public void UpdateTab(Faction faction)
		{
			_factionTabImages[(int)faction.Type - 1].color = faction.IsNew ? Colors.UINew : Colors.UINormal;
			_factionTabButtons[(int)faction.Type - 1].gameObject.SetActive(true);
		}

		public void DisplayNotification(INotification notification)
		{
			CancelInvoke(nameof(HideNotification));
			_notificationGo.SetActive(true);
			_notificationText.text = notification.NotificationText;
			Invoke(nameof(HideNotification), 5f);
			//TODO Tweening
		}

		private void HideNotification()
		{
			_notificationGo.SetActive(false);
			_notificationText.text = "";
		}

		public void DisplayHoverData(HoverType hoverType, int index)
		{
			_hoverPanelGo.SetActive(true);
			switch (hoverType)
			{
				case HoverType.Upgrade:
					var upgrade = _currentFaction.GetUpgrade(index);
					if (upgrade == null || !upgrade.IsUnlocked)
					{
						_hoverPanelNameText.text = "Unknown upgrade";
						_hoverPanelEffectsText.text = "";
						_hoverPanelCostsText.text = "";
						break;
					}

					upgrade.SetNotNew();
					_upgradeImages[index].color = upgrade.IsNew ? Colors.UINew : Colors.UINormal;
					_hoverPanelNameText.text = upgrade.Id;
					_hoverPanelEffectsText.text = upgrade.GetDataString();
					_hoverPanelCostsText.text = upgrade.GetCostsString();

					break;
				default:
					Log.Error("Invalid HoverType: " + hoverType);
					break;
			}
		}

		public void MoveHoverPanel(Vector3 mousePosition)
		{
			Vector3 offset = new Vector2(Screen.width * 0.145f, -Screen.height * 0.21f);
			_hoverPanelGo.transform.position = mousePosition + offset;
		}

		public void HideHoverPanel()
		{
			_hoverPanelGo.SetActive(false);
		}

		public void ShowTestVersionPanel()
		{
			_testVersionPanel.SetActive(true);
		}

		private void SwitchFactionTab(FactionType type)
		{
			_currentFaction = _factionController.Get(type);
			if (_currentFaction == null)
				return;

			_currentFaction.SetNotNew();
			_factionTabImages[(int)type - 1].color = _currentFaction.HasNewUpgrades ? Colors.UIFactionUpgradesAvailable : Colors.UINormal;

			_background.sprite = GetFactionBackground(type);

			_factionType.text = type.ToString();

			UpdateFactionTabInfo();
		}

		public void UpdateFactionTabInfo() //TODO Refactor, split this
		{
			UpdateFactionTabUpgrades();

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

			UpdateResourceRates();
			UpdateFactionTabPopulationInfo();
		}

		public void UpdateFactionTabUpgrades()
		{
			for (int i = 0; i < _upgradeButtons.Length; i++)
			{
				var upgrade = _currentFaction.GetUpgrade(i);

				if (upgrade == null || !upgrade.IsUnlocked)
				{
					_upgradeButtons[i].interactable = false;
					_upgradeButtonTexts[i].text = "Unknown";
					continue;
				}

				_upgradeImages[i].color = upgrade.IsNew ? Colors.UINew : Colors.UINormal;
				_upgradeButtons[i].interactable = !upgrade.IsBought;
				_upgradeButtonTexts[i].text = _currentFaction.GetUpgradeId(i);
			}
		}

		private void UpdateResourceRates()
		{
			double[] rates = _resourceController.Rates.Rates;

			_rates[0].text = "Rates:";
			for (int i = 0; i < rates.Length; i++)
			{
				double rate = rates[i];
				if (rate == 0)
					continue;
				/*if (_rates.Length <= i)
					continue;*/
				_rates[0].text += $" {(ResourceType)i}: {rate:F2}";
			}
		}

		private void UpdateFactionTabPopulationInfo()
		{
			if (_currentFaction == null)
				return;

			UpdateFactionTabPopulation();

			double multiplier = _currentFaction.GetPopulationCostMultiplier(_currentPopulationAmount);
			string costs = string.Join(", ", _currentFaction.FactionResources.CreateCost.Select(r =>
				(r.Value.Value * multiplier).ToString("F1") + " " + r.Key));
			_factionBuyPopulationText.text = $"Buy {_currentPopulationAmount} population";
			_factionPopulationCostText.text = costs;
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