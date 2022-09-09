using System;
using Newtonsoft.Json;

namespace IdleFactions
{
	/// <summary>
	///		Stored resource
	/// </summary>
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

		public void Save(JsonTextWriter writer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(nameof(Type));
			writer.WriteValue(Type);
			writer.WritePropertyName(nameof(Value));
			writer.WriteValue(Value);
			writer.WriteEndObject();
		}
	}
}