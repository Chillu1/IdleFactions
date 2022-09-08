using UnityEngine;

namespace IdleFactions
{
	public static class Colors
	{
		public static Color ValueGain = new Color(0.5f, 1f, 0.50f);
		public static Color ValueLoss = new Color(1f, 0.5f, 0.5f);

		public static Color GetColor(ValueRate rate)
		{
			switch (rate)
			{
				case ValueRate.Neutral:
					return Color.white;
				case ValueRate.Positive:
					return ValueGain;
				case ValueRate.Negative:
					return ValueLoss;
				default:
					return Color.white;
			}
		}
	}
}