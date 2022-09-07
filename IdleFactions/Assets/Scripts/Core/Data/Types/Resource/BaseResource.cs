using System;
using JetBrains.Annotations;

namespace IdleFactions
{
	public abstract class Resource : IResource
	{
		public ResourceType Type { get; }
		public virtual double Value { get; }

		protected const double Tolerance = 0.001d;

		protected Resource(ResourceType type, double value = 1d)
		{
			Type = type;
			Value = value;
		}

		public static bool operator ==(Resource a, Resource b)
		{
			if (ReferenceEquals(a, b))
				return true;

			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
				return false;

			return a.Equals(b);
		}

		public static bool operator !=(Resource a, Resource b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return $"{Type}: {Value}";
		}

		public override bool Equals(object obj)
		{
			return obj != null && Equals((IResource)obj);
		}

		public bool Equals([NotNull] IResource other)
		{
			return Type == other.Type && Math.Abs(Value - other.Value) < Tolerance;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}