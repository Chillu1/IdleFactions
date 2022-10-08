using System.Collections.Generic;
using System.Linq;
using IdleFactions.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public class ProgressionController : ISavable, ILoadable
	{
		public const string JSONKey = "Progression";
		private const string JSONKeyResourceProgression = "ResourceProgression";
		private const string JSONKeyFactionProgression = "FactionProgression";

		private readonly IDictionary<ResourceType, Progression> _resourceProgressions;
		private readonly ISet<ResourceType> _calledResources;

		private readonly IDictionary<FactionType, Progression> _factionProgressions;
		private readonly ISet<FactionType> _calledFaction;

		private const double ProgressionCooldownCheck = 0.5;
		private float _timer;

		private readonly IFactionController _factionController;
		private readonly UIController _uiController;

		public ProgressionController(ProgressionData progressionData, IFactionController factionController, UIController uiController)
		{
			_factionController = factionController;
			_uiController = uiController;
			_calledResources = new HashSet<ResourceType>();
			_calledFaction = new HashSet<FactionType>();

			_resourceProgressions = new Dictionary<ResourceType, Progression>(progressionData.GetResourceProgressions());
			_factionProgressions = new Dictionary<FactionType, Progression>(progressionData.GetFactionProgressions());
		}

		public void Update(float dt)
		{
			_timer += dt;
			if (_timer <= ProgressionCooldownCheck)
				return;

			_timer = 0f;
			_calledResources.Clear();
			_calledFaction.Clear();
		}

		public void OnAddResource(IChangeableResource resource)
		{
			if (!_resourceProgressions.TryGetValue(resource.Type, out var progression) || progression.IsCompleted)
				return;
			if (_calledResources.Contains(resource.Type))
				return;
			_calledResources.Add(resource.Type);

			switch (progression.CurrentEntry.Condition)
			{
				case ProgressionCondition.Resource condition:
					if (condition.Value <= resource.Value)
					{
						HandleProgressionAction(progression.CurrentEntry.Action);
						progression.Increment();
					}

					break;
			}
		}


		public void OnAddFactionPopulation(FactionType type, double population)
		{
			if (!_factionProgressions.TryGetValue(type, out var progression) || progression.IsCompleted)
				return;
			if (_calledFaction.Contains(type))
				return;
			_calledFaction.Add(type);

			switch (progression.CurrentEntry.Condition)
			{
				case ProgressionCondition.Faction condition:
					if (condition.Value <= population)
					{
						HandleProgressionAction(progression.CurrentEntry.Action);
						progression.Increment();
					}

					break;
			}
		}

		private void HandleProgressionAction(IProgressionAction progressionAction)
		{
			switch (progressionAction)
			{
				case ProgressionAction.ChooseFaction action:
					_uiController.OpenFactionSelection(action.FactionTypes);
					break;
				case ProgressionAction.DiscoverFaction action:
					_factionController.Get(action.FactionType)!.Discover();
					break;
				case ProgressionAction.UnlockUpgrade action:
					_factionController.Get(action.FactionType)!.GetUpgrade(action.UpgradeId)!.Unlock();
					_uiController.UpdateFactionTabUpgrades(); //TODO Move?
					break;
				case ProgressionAction.TempUI:
					_uiController.ShowTestVersionPanel();
					break;
			}
		}

		public void Save(JsonTextWriter writer)
		{
			writer.WritePropertyName(JSONKey);
			writer.WriteStartObject();

			writer.WritePropertyName(JSONKeyResourceProgression);
			writer.WriteStartArray();
			foreach (var progression in _resourceProgressions.Values)
				progression.Save(writer);
			writer.WriteEndArray();

			writer.WritePropertyName(JSONKeyFactionProgression);
			writer.WriteStartArray();
			foreach (var progression in _factionProgressions.Values)
				progression.Save(writer);
			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public void Load(JObject data)
		{
			var progressions = data.Value<JObject>(JSONKey);

			LoadProgression(JSONKeyResourceProgression, _resourceProgressions);
			LoadProgression(JSONKeyFactionProgression, _factionProgressions);

			void LoadProgression<TKey>(string key, IDictionary<TKey, Progression> dictProgressions)
			{
				foreach (var progressionData in progressions!.Value<JArray>(key).EmptyIfNull())
				{
					var progression = dictProgressions.Values.First(p => p.Id == progressionData.Value<string>(nameof(Progression.Id)));
					progression.Load(progressionData.Value<JObject>());
					progression.LoadSavedActions(HandleProgressionAction);
				}
			}
		}
	}
}