using System;

namespace IdleFactions
{
	public class StoredResource : IChangeableResource
	{
		public ResourceType Type { get; }
		public double Value { get; private set; }

		public StoredResource(ResourceType type)
		{
			Type = type;
		}

		public void Add(double value)
		{
			Value += value;
		}

		public bool TryRemove(double value)
		{
			if (Value >= value)
			{
				Remove(value);
				return true;
			}

			return false;
		}

		public void Remove(double value)
		{
			Value -= Math.Abs(value);
			if (Value < 0)
				Value = 0;
		}

		public override string ToString()
		{
			return $"{Type}: {Value:F1}";
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}