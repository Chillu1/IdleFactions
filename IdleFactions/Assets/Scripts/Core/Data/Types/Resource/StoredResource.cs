using System;

namespace IdleFactions
{
	public class ChangeableResource : Resource, IChangeableResource
	{
		public ChangeableResource(ResourceType type, double value = 0d) : base(type, value)
		{
		}

		public void Add(double value)
		{
			Value += value;
		}

		public void Remove(double value)
		{
			Value -= Math.Abs(value);
			if (Value < 0)
				Value = 0;
		}
	}
}