using System;
using System.Linq;

namespace IdleFactions
{
	public enum ResourceType
	{
		None,

		/// <summary>
		///		Use for every single "creature"/entity
		/// </summary>
		Essence,
		Light,
		Dark,
		Lava,
		Water,

		Stone,
		Plant,
		Wood,
		Food,
		Wildlife,

		/// <summary>
		///		Used for creation of: Human, Warlock, Dwarf, Necro, Elves
		/// </summary>
		Soul,
		Fire,
		Ore,
		Heat, //Just use fire instead? Or switch fire for heat
		Metal,
		Gold,

		Magic,
		Mana,

		Energy, //should also be used as Electricity?
		Steam, //? Only used by humans for electricity?
		Electricity,

		Body,
		Bones,
		Skeleton,

		//Specials
		/// <summary>
		///		Special possible resource that is generated with time
		/// </summary>
		Time,

		/// <summary>
		///		Temp resource to represent infinity (impossible to get)
		/// </summary>
		Infinity,
	}

	public static class ResourceTypeHelper
	{
		public static readonly ResourceType[] ResourceTypes =
			Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>().Skip(1).ToArray();
	}
}