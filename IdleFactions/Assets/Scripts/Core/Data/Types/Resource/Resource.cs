namespace IdleFactions
{
	public abstract class Resource : BaseResource, IResource
	{
		public double Value { get; protected set; }

		protected Resource(ResourceType type, double value) : base(type)
		{
			Value = value;
		}

		public override string ToString()
		{
			return $"{Type}: {Value:F1}";
		}
	}
}