using System.Collections.Generic;

namespace IdleFactions
{
	public class ProgressionData
	{
		private readonly IDictionary<ResourceType, Progression> _resourceProgressions;
		private readonly ISet<ResourceType> _calledResources;

		private const double ProgressionCooldownCheck = 0.5;
		private float _timer;

		private readonly FactionController _factionController;


		public ProgressionData(FactionController factionController)
		{
			_factionController = factionController;
			_resourceProgressions = new Dictionary<ResourceType, Progression>();
			_calledResources = new HashSet<ResourceType>();

			SetupProgressions();
		}

		public void Update(float dt)
		{
			_timer += dt;
			if (_timer <= ProgressionCooldownCheck)
				return;

			_timer = 0f;
			_calledResources.Clear();
		}

		public void OnAddResource(IStoredResource resource)
		{
			if (!_resourceProgressions.TryGetValue(resource.Type, out var progression))
				return;
			if (_calledResources.Contains(resource.Type))
				return;
			_calledResources.Add(resource.Type);

			switch (progression.Condition)
			{
				case ProgressionResourceCondition resourceCondition:
					if (resourceCondition.ResourceCost.Value <= resource.Value)
					{
						HandleProgressionAction(progression.Action);
						progression.Increment();
					}

					break;
			}
		}

		private void HandleProgressionAction(IProgressionAction action)
		{
			switch (action)
			{
				case ProgressionDiscoverAction discoverAction:
					_factionController.GetFaction(discoverAction.FactionType)!.Discover();
					break;
			}
		}

		private void SetupProgressions()
		{
			_resourceProgressions.Add(ResourceType.Essence, new Progression(
				new ProgressionResourceCondition(new ResourceCost(ResourceType.Essence, 1)), //BALANCE 10
				new ProgressionDiscoverAction(FactionType.Divinity))
			);
			_resourceProgressions.Add(ResourceType.Light, new Progression(
				new ProgressionResourceCondition(new ResourceCost(ResourceType.Light, 2)),
				new ProgressionDiscoverAction(FactionType.Void))
			);
		}
	}
}