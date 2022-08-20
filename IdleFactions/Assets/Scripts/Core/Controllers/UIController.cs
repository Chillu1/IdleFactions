using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFactions
{
	public class UIController : MonoBehaviour
	{
		private const int MaxButtonUpgrades = 6; //TEMP

		private ResourceController _resourceController;
		private FactionController _factionController;

		private TMP_Text[] _resourceTexts;
		private Button[][] _factionUpgradeButtons;

		public void Setup(ResourceController resourceController, FactionController factionController)
		{
			_resourceController = resourceController;
			_factionController = factionController;
		}

		private void Start()
		{
			var canvas = GameObject.Find("Canvas");

			var factionResources = canvas.transform.Find("FactionResources");

			int childCount = factionResources.childCount;
			_resourceTexts = new TMP_Text[childCount];
			_factionUpgradeButtons = new Button[childCount][];
			for (int i = 0; i < childCount; i++)
			{
				var factionResource = factionResources.GetChild(i);
				_resourceTexts[i] = factionResource.GetComponent<TMP_Text>();

				int j = i;
				for (int k = 0; k < MaxButtonUpgrades; k++)
				{
					//Log.Info(factionResource.GetChild(0).name + "_" + factionResource.GetChild(0).GetChild(0).name);
					_factionUpgradeButtons[i] = new Button[MaxButtonUpgrades];
					_factionUpgradeButtons[i][k] =
						factionResource.Find("Upgrades").Find("UpgradeButton (" + k + ")").GetComponent<Button>();
					_factionUpgradeButtons[i][k].GetComponentInChildren<TMP_Text>().text =
						_factionController.GetFaction((FactionType)j + 1)?.GetUpgradeId(k);
					_factionUpgradeButtons[i][k].onClick.AddListener(() => _factionController.GetFaction((FactionType)j + 1)?.TryUpgrade());
				}
			}
		}

		private void Update()
		{
			for (int i = 0; i < _resourceTexts.Length; i++)
			{
				var resourceText = _resourceTexts[i];
				resourceText.text = _resourceController.GetResource(i)?.ToString();
			}
		}
	}
}