using UnityEngine;

namespace IdleFactions
{
	public class DataInitializer : MonoBehaviour
	{
		public DataController DataController { get; private set; }

		private static DataInitializer _instance;

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);
			if (GetInstanceID() != _instance.GetInstanceID())
				Destroy(gameObject);
		}

		public void Start()
		{
			if (_instance == null || (GetInstanceID() == _instance.GetInstanceID() && _instance.DataController == null))
				DataController = new DataController();
		}
	}
}