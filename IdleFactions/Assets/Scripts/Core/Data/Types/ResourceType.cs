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

		Nature,
		Food,
		Wood,
		Wildlife,
		Plant,
		Stone,
		Ore,
		Magic,
		Mana,
		Fire,

		/// <summary>
		///		Used for creation of: Human, Warlock, Dwarf, Necro, Elves
		/// </summary>
		Soul,
		Energy,

		Body,
		Bones,
		Skeleton,
		Gold,
		Heat,
		Metal,

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