using System;
using System.Collections.Generic;

namespace Riff.Extensions
{
    public static class ListInterfaceExtensions
    {
        public static int IndexOf<TElement>(this IList<TElement> list, Predicate<TElement> match)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static void Append<TElement>(this IList<TElement> list, IList<TElement> list2)
        {
            if (list == null || list2 == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var element in list2)
            {
                list.Add(element);
            }
        }
    }
}
