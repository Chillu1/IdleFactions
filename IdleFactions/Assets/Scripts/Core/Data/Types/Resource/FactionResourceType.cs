namespace IdleFactions
{
	public enum FactionResourceType
	{
		None,
		Generate,
		CreateCost,
		GenerateCost,
		LiveCost,

		GenerateAdded,
		GenerateCostAdded,
		LiveCostAdded,
	}

	public enum FactionResourceAddedType
	{
		None,
		GenerateAdded = FactionResourceType.GenerateAdded,
		GenerateCostAdded = FactionResourceType.GenerateCostAdded,
		LiveCostAdded = FactionResourceType.LiveCostAdded,
	}
}