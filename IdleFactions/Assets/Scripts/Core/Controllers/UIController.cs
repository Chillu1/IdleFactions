using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFactions
{
	public class UIController : MonoBehaviour
	{
		private ResourceController _resourceController;
		private FactionController _factionController;

		private TMP_Text[] _resourceTexts;
		private Button[] _factionUpgradeButtons;

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
			_factionUpgradeButtons = new Button[childCount];
			for (int i = 0; i < childCount; i++)
			{
				var factionResource = factionResources.GetChild(i);
				_resourceTexts[i] = factionResource.GetComponent<TMP_Text>();
				_factionUpgradeButtons[i] = factionResource.Find("UpgradeButton").GetComponent<Button>();
				int j = i;
				_factionUpgradeButtons[i].onClick.AddListener(() => _factionController.GetFaction((FactionType)j + 1)?.TryUpgrade());
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