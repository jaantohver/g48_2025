using System;
using System.Collections.Generic;

namespace door.Droid
{
    public static class ListUtils
    {
        public static void Remove<T>(this List<T> list, List<T> items)
        {
            foreach (var item in items)
            {
                if (list.Contains(item))
                {
                    list.Remove(item);
                }
            }
        }

        public static bool EqualsAny<T>(this T self, params T[] items)
        {
            foreach (var item in items)
            {
                if (self.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

