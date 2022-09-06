namespace IdleFactions
{
	public readonly struct UpgradeActionNewResource : IUpgradeAction // TODO Only allowe added?
	{
		public ResourceNeedsType ResourceNeedsType { get; }
		public IResource Resource { get; }

		public UpgradeActionNewResource(ResourceNeedsType resourceNeedsType, Resource resource)
		{
			ResourceNeedsType = resourceNeedsType;
			Resource = resource;
		}
	}
}