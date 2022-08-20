using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleFactions.Utils
{
	public static class Extensions
	{
		public static T RandomElement<T>(this IEnumerable<T> enumerable) //For lists & arrays. Might be a bit slow
		{
			return enumerable.RandomElement(new Random());
		}

		public static T RandomElement<T>(this IEnumerable<T> enumerable, Random random) //For loops
		{
			int count = enumerable.Count();
			if (count == 0)
				throw new ArgumentException("Enumerable can't be empty");

			int index = random.Next(0, count);
			return enumerable.ElementAt(index);
		}

		//Pretty slow random elements, prob TEMP
		public static IEnumerable<T> RandomElements<T>(this IReadOnlyList<T> source, int amount)
		{
			return RandomElements(source, new Random(), amount);
		}

		public static IEnumerable<T> RandomElements<T>(this IReadOnlyList<T> source, Random random, int amount)
		{
			return source.OrderBy(x => random.Next()).Take(amount);
		}

		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
		{
			return source ?? Enumerable.Empty<T>();
		}

		public static float NextFloat(this Random random, float minValue, float maxValue)
		{
			return (float)(random.NextDouble() * (maxValue - minValue) + minValue);
		}
	}

	public static class TypeExtensions
	{
		public static bool IsSameOrSubclass(this Type potentialBase, Type potentialDescendant)
		{
			return potentialDescendant.IsSubclassOf(potentialBase)
			       || potentialDescendant == potentialBase;
		}
	}
}