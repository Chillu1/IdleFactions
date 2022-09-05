using System;
using System.Linq;

namespace IdleFactions
{
	public enum FactionType
	{
		None,
		Creation,
		Divinity,
		Void,
		Heat,
		Ocean,

		Nature,
		Nature2,
		Human,

		Goblin,
		Ogre,
		Warlock,
		Treant,
		Demon,
		Dwarf,
		Golem,
		Drowner,
		Necro,
		Skeleton
	}

	public static class FactionTypeHelper
	{
		public static readonly FactionType[] AllFactionTypes = Enum.GetValues(typeof(FactionType)).Cast<FactionType>().Skip(1).ToArray();
	}
}