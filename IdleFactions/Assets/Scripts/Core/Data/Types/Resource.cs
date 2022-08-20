using System;

namespace IdleFactions
{
	public class Resource // : IResource
	{
		public ResourceType Type { get; }

		public double Value => _baseValue * _multiplier;

		private double _baseValue;
		private double _multiplier = 1d;

		public Resource(ResourceType type)
		{
			Type = type;
		}

		public Resource(ResourceCost cost)
		{
			Type = cost.Type;
			_baseValue = cost.Value;
		}

		public void Add(double value)
		{
			_baseValue += value;
		}

		public void Remove(double value)
		{
			_baseValue -= Math.Abs(value);
			if (_baseValue < 0)
				_baseValue = 0;
		}

		public void TimesMultiplier(double multiplier)
		{
			_multiplier *= multiplier;
		}

		public override string ToString()
		{
			return $"Resource: {Type}. Value: {_baseValue:F2}";
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}