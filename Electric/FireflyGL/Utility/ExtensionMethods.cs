using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public static class ExtensionMethods
	{
		public static void AppendMany<T>(this ICollection<T> collection, params T[] elements)
		{
			var list = collection as List<T>;
			if (list != null)
			{
				list.AddRange(elements);
				return;
			}
			foreach (var element in elements)
			{
				collection.Add(element);
			}
		}
	}
}
