using System;
using BreakInfinity;

namespace IdleFactions
{
	public class StoredResource : IStoredResource
	{
		public ResourceType Type { get; }
		public BigDouble Value { get; private set; }

		public StoredResource(ResourceType type)
		{
			Type = type;
		}

		public void Add(BigDouble value)
		{
			Value += value;
		}

		public bool TryRemove(BigDouble value)
		{
			if (Value >= value)
			{
				Remove(value);
				return true;
			}

			return false;
		}

		public void Remove(BigDouble value)
		{
			Value -= BigDouble.Abs(value);
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