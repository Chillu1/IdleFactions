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
		Magic,
		Mana,
		Fire,
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
}