using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    public static class ArrayExtensions
    {
        public static bool IsValidIndex<T>(this T[] self, int index)
        {
            return self.Length - 1 >= index;
        }

        public static bool IsValidIndex<T>(this List<T> self, int index)
        {
            return self.Count - 1 >= index;
        }

        public static bool Remove<T>(this List<T> array, Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentException();
            }

            int index = -1;
            for (int i = 0; i < array.Count; i++)
            {
                if (match(array[i]))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                array.RemoveAt(index);
                return true;
            }
            else
                return false;
        }

        public static bool Remove<T>(this T[] array, Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentException();
            }

            List<T> localArray = array.ToList();

            int index = -1;
            for (int i = 0; i < localArray.Count; i++)
            {
                if (match(localArray[i]))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                localArray.RemoveAt(index);
                array = localArray.ToArray();
                return true;
            }
            else
                return false;
        }

        public static T GetLast<T>(this List<T> list)
        {
            if (list == null)
                return default;

            if (list.Count == 0)
                return default;

            return list.ElementAt(list.Count - 1);
        }

        public static T GetLast<T>(this T[] list)
        {
            if (list == null)
                return default;

            if (list.Length == 0)
                return default;

            return list.ElementAt(list.Length - 1);
        }
    }
}
