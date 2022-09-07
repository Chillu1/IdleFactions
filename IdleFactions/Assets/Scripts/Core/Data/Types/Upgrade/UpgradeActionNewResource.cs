namespace IdleFactions
{
	public readonly struct UpgradeActionNewResource : IUpgradeAction // TODO Only allow added?
	{
		public FactionResourceType FactionResourceType { get; }
		public IFactionResource Resource { get; }

		public UpgradeActionNewResource(FactionResourceType factionResourceType, FactionResource resource)
		{
			FactionResourceType = factionResourceType;
			Resource = resource;
		}
	}
}