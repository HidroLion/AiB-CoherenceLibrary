using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ArrayUtils
{
	public static string Format(this IEnumerable array, string separator = ",", string prefix = "[", string suffix = "]")
		{
			return prefix + string.Join(separator, array
				.Cast<object>()
				.Select(x =>
					x == null ? "NULL" :
					(x is IEnumerable && !(x is string)) ? Format(x as IEnumerable) :
					x.ToString())
				.ToArray()) + suffix;
		}

		public static bool Some<T>(this IEnumerable<T> array, Func<T, bool> predicate = null)
		{
			if (predicate == null)
				return array.Cast<T>().FirstOrDefault() != null;
			return  array.Cast<T>().FirstOrDefault(predicate) != null;
		}
}