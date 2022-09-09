using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public class Progression : ISavable, ILoadable, IShallowClone<Progression>
	{
		public string Id { get; }
		public IProgressionEntry CurrentEntry { get; private set; }
		public bool IsCompleted { get; private set; }

		private readonly IProgressionEntry[] _entries;
		private int _currentEntryIndex;

		public Progression(string id, params IProgressionEntry[] entries)
		{
			Id = id;
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

		public void Save(JsonTextWriter writer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(nameof(Id));
			writer.WriteValue(Id);

			writer.WritePropertyName(nameof(_currentEntryIndex));
			writer.WriteValue(_currentEntryIndex);

			writer.WritePropertyName(nameof(IsCompleted));
			writer.WriteValue(IsCompleted);
			writer.WriteEndObject();
		}

		public void Load(JObject data)
		{
			_currentEntryIndex = data.Value<int>(nameof(_currentEntryIndex));

			IsCompleted = data.Value<bool>(nameof(IsCompleted));

			CurrentEntry = IsCompleted ? _entries[_entries.Length - 1] : _entries[_currentEntryIndex];
		}

		public void LoadSavedActions(Action<IProgressionAction> handleProgressionAction)
		{
			for (int i = 0; i < _currentEntryIndex; i++)
			{
				handleProgressionAction(_entries[i].Action);
			}
		}

		public Progression ShallowClone()
		{
			return new Progression(Id, _entries);
		}
	}
}