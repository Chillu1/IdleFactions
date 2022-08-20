using System;
using System.Linq;

namespace IdleFactions
{
	public enum FactionType
	{
		None,
		Divinity,
		Void,
		Ocean,

		Nature,
		Nature2,
		Human
	}

	public static class FactionTypeHelper
	{
		public static readonly FactionType[] AllFactionTypes = Enum.GetValues(typeof(FactionType)).Cast<FactionType>().Skip(1).ToArray();
	}
}