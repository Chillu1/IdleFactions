namespace IdleFactions
{
	public class Progression
	{
		public IProgressionEntry CurrentEntry { get; private set; }
		public bool IsCompleted { get; private set; }

		private readonly IProgressionEntry[] _entries;
		private int _currentEntryIndex;

		public Progression(params IProgressionEntry[] entries)
		{
			_entries = entries;
			CurrentEntry = _entries[_currentEntryIndex];
		}

		public void Increment()
		{
			if (IsCompleted)
				return;

			_currentEntryIndex++;

			if (_currentEntryIndex >= _entries.Length)
			{
				IsCompleted = true;
				return;
			}

			CurrentEntry = _entries[_currentEntryIndex];
		}
	}
}