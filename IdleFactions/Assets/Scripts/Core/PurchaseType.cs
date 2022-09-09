using System;

namespace IdleFactions
{
	[Flags]
	public enum PurchaseType
	{
		One = 1,
		Ten = 2,
		Hundred = 4,
		TenPercent = 8,
		TwentyFivePercent = 16,
		FiftyPercent = 32,
		HundredPercent = 64,

		AsMuchAsPossible = 128 //TODO Rename
	}
}