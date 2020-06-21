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
    }
}
