namespace IdleFactions
{
	public class FactionResource : ChangeableResource, IFactionResource
	{
		public new double Value => base.Value * _multiplier;

		private double _multiplier = 1d;

		public FactionResource(ResourceType type, double value = 0d) : base(type, value)
		{
		}

		public void TimesMultiplier(double multiplier)
		{
			_multiplier *= multiplier;
		}

		public override string ToString()
		{
			return $"{Type}: {Value:F2}";
		}
	}
}