using Newtonsoft.Json;

namespace IdleFactions
{
	public sealed class PrestigeResource : ISavable
	{
		public FactionType Type { get; }

		private double _amount;

		public PrestigeResource(FactionType type, double amount = 0)
		{
			Type = type;
			_amount = amount;
		}

		public void Add(double amount) => _amount += amount;

		public void Add(PrestigeResource resource) => _amount += resource._amount;

		public bool CanAfford(double amount) => _amount >= amount;

		public bool TrySpend(double amount)
		{
			if (!CanAfford(amount))
				return false;

			_amount -= amount;
			return true;
		}

		public void Save(JsonTextWriter writer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(nameof(Type));
			writer.WriteValue(Type);
			writer.WritePropertyName("Amount");
			writer.WriteValue(_amount);
			writer.WriteEndObject();
		}
	}
}