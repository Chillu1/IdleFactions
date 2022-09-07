using System;
using BreakInfinity;

namespace IdleFactions
{
	public class Resource : IResource
	{
		public ResourceType Type { get; }

		public BigDouble Value => _baseValue * _multiplier;

		private BigDouble _baseValue;
		private double _multiplier = 1d;

		public Resource(ResourceType type, BigDouble value = default)
		{
			Type = type;
			_baseValue = value;
		}

		public void Add(BigDouble value)
		{
			_baseValue += value;
		}

		public void Remove(BigDouble value)
		{
			_baseValue -= BigDouble.Abs(value);
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