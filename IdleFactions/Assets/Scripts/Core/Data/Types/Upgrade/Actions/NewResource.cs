namespace IdleFactions
{
	public static partial class UpgradeAction
	{
		public readonly struct NewResource : IUpgradeAction
		{
			public FactionResourceType FactionResourceType { get; }
			public IFactionResource Resource { get; }

			public NewResource(FactionResourceAddedType factionResourceAddedType, IFactionResource resource)
			{
				FactionResourceType = (FactionResourceType)factionResourceAddedType;
				Resource = resource;
			}

			public override string ToString()
			{
				return $"New {FactionResourceType} resource: {Resource}";
			}
		}
	}
}