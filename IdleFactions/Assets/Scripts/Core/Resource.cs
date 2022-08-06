using System;

namespace IdleFactions
{
	public class Resource
	{
		public ResourceType Type { get; }
        
        public double Value { get; private set; }

        public Resource(ResourceType type)
        {
	        Type = type;
        }
        
        public static Resource operator +(Resource resource, double value)
        {
	        resource.Value += value;
	        return resource;
		}

        public static Resource operator -(Resource resource, double value)
        {
	        resource.Value -= Math.Abs(value);
			return resource;
        }
        
        public static bool operator <(Resource resource, double value) => resource.Value < value;
        public static bool operator >(Resource resource, double value) => resource.Value > value;
        public static bool operator <=(Resource resource, double value) => resource.Value <= value;
        public static bool operator >=(Resource resource, double value) => resource.Value >= value;

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