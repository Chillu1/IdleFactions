using System;

namespace IdleFactions
{
	public class Resource : IResource
	{
		public ResourceType Type { get; }

		public double Value { get; private set; }

		public Resource(ResourceType type)
		{
			Type = type;
		}

		public void Add(double value)
		{
			Value += value;
		}

		public void Remove(double neededResourceAmount)
		{
			Value -= Math.Abs(neededResourceAmount);
			if (Value < 0)
				Value = 0;
		}

		public override string ToString()
		{
			return $"Resource: {Type}. Value: {Value:F2}";
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}