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

		Golem,
		Nature,
		Treant,

		//Nature2, //Should we even have a second nature-wildlife faction?
		Human,

		Demon,
		Dwarf,
		Goblin,
		Orc,

		Mage, //Wizard?
		Warlock,

		Ogre,
		Drowner,
		Necro,
		Skeleton,
		Elf

		//Arcane?
		//Gnome?
		//Beast?
		//Fairy?
		//Giant?
		//Halfling?
		//Troll?
		//Angel?
		//Dragon?
		//Elemental?
	}

	public static class FactionTypeHelper
	{
		public static readonly FactionType[] AllFactionTypes = Enum.GetValues(typeof(FactionType)).Cast<FactionType>().Skip(1).ToArray();
	}
}