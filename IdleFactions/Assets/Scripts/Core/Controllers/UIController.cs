using TMPro;
using UnityEngine;

namespace IdleFactions
{
	public class UIController : MonoBehaviour
	{
		private ResourceController _resourceController;
		private TMP_Text[] _resourceTexts;

		public void Setup(ResourceController resourceController)
		{
			_resourceController = resourceController;
		}
		
		private void Start()
		{
			var canvas = GameObject.Find("Canvas");
			
			var factionResources = canvas.transform.Find("FactionResources");
			_resourceTexts = factionResources.GetComponentsInChildren<TMP_Text>();
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