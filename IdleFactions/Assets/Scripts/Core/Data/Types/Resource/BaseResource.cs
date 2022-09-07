namespace IdleFactions
{
	public abstract class BaseResource : IBaseResource
	{
		public ResourceType Type { get; }

		protected BaseResource(ResourceType type)
		{
			Type = type;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}