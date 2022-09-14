using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleFactions.Behaviours
{
	public enum HoverType
	{
		None,
		Upgrade,
	}

	public class HoverPanel : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
	{
		public HoverType HoverType;

		private int _index;
		private UIController _uiController;

		private static int _currentIndex;
		private static bool _isHovering;
		private float _timer;
		private static bool _timeIsOver;

		//TODO Doesn't work rn, refactor this temp
		private const double HoverTime = 0.3;

		private void Start()
		{
			_uiController = FindObjectOfType<UIController>();
			_index = transform.GetSiblingIndex();
		}

		private void Update()
		{
			if (!_isHovering || _timeIsOver /* || _index != _currentIndex*/)
				return;

			_timer += Time.deltaTime;
			if (_timer > HoverTime)
			{
				_timeIsOver = true;
				_uiController.DisplayHoverData(HoverType, _currentIndex);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_currentIndex = _index;
			if (_timeIsOver)
				_uiController.DisplayHoverData(HoverType, _currentIndex);
			_isHovering = true;
		}

		public void OnPointerMove(PointerEventData eventData)
		{
			if (_currentIndex != _index)
			{
				_currentIndex = _index;
				_uiController.DisplayHoverData(HoverType, _currentIndex);
			}

			_uiController.MoveHoverPanel(Input.mousePosition);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_timeIsOver = false;
			_isHovering = false;
			_timer = 0;
			_uiController.HideHoverPanel();
		}
	}
}