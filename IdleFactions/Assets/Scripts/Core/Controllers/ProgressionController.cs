using System.Collections.Generic;

namespace IdleFactions
{
	public class ProgressionController
	{
		private readonly IDictionary<ResourceType, Progression> _resourceProgressions;
		private readonly ISet<ResourceType> _calledResources;

		private const double ProgressionCooldownCheck = 0.5;
		private float _timer;

		private readonly IFactionController _factionController;

		public ProgressionController(ProgressionData progressionData, IFactionController factionController)
		{
			_factionController = factionController;
			_calledResources = new HashSet<ResourceType>();

			_resourceProgressions = new Dictionary<ResourceType, Progression>(progressionData.GetProgressions());
		}

		public void Update(float dt)
		{
			_timer += dt;
			if (_timer <= ProgressionCooldownCheck)
				return;

			_timer = 0f;
			_calledResources.Clear();
		}

		public void OnAddResource(IChangeableResource resource)
		{
			if (!_resourceProgressions.TryGetValue(resource.Type, out var progression))
				return;
			if (_calledResources.Contains(resource.Type))
				return;
			_calledResources.Add(resource.Type);

			switch (progression.CurrentEntry.Condition)
			{
				case ProgressionResourceCondition resourceCondition:
					if (resourceCondition.Value <= resource.Value)
					{
						HandleProgressionAction(progression.CurrentEntry.Action);
						progression.Increment();
					}

					break;
			}
		}

		private void HandleProgressionAction(IProgressionAction action)
		{
			switch (action)
			{
				case ProgressionDiscoverFactionAction discoverAction:
					_factionController.Get(discoverAction.FactionType)!.Discover();
					break;
				case ProgressionDiscoverUpgradeAction discoverAction:
					_factionController.Get(discoverAction.FactionType)!.GetUpgrade(discoverAction.UpgradeId)!.Unlock();
					break;
			}
		}
	}
}