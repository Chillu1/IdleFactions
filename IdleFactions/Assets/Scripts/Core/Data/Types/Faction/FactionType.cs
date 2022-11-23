using System;
using System.Collections.Generic;
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
		//Fairy/Nymph/Sprite?
		//Giant?
		//Halfling?
		//Troll?
		//Angel?
		//Dragon?
		//Elemental?
		//Undead/Zombie?
		//Drake/Wyvern/Dragon?
		//Druid?
		//Titan?
		//Faceless?
		//Drow?
		//DarkElf?//Some kind of choice/path of normal elves?
		//Ethereal?
		//LizardFolk?
		//Kobold?
		//Gnoll?
		//Vampire?
		//Shade?
		//Faun?
		//Imp/Gog?
		//Magi?
		//Magog?
		//Lich?
		//Elder?
		//Pyromancer? //Should probably be an upgrade
		//Aquamancer? //Should probably be an upgrade
		//Leprechaun?
		//Wight?
		//Wraith?
		//Psion?
		//Spirit?
		//Undine/fishfolk?
	}

	public static class FactionTypeHelper
	{
		public static readonly FactionType[] AllFactionTypes = Enum.GetValues(typeof(FactionType)).Cast<FactionType>().Skip(1).ToArray();

		public static readonly ISet<FactionType> SoulFactionTypes = new HashSet<FactionType>(new[]
		{
			FactionType.Human, FactionType.Dwarf, FactionType.Goblin, FactionType.Orc, FactionType.Mage, FactionType.Warlock,
			FactionType.Ogre, FactionType.Drowner, FactionType.Necro, FactionType.Elf
		});
	}
}