using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers;

namespace Tavstal.TLibrary.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Checks if the provided index is a valid index within the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="self">The array to check.</param>
        /// <param name="index">The index to check for validity.</param>
        /// <returns>True if the index is valid, false otherwise.</returns>
        public static bool IsValidIndex<T>(this T[] self, int index)
        {
            return self.Length - 1 >= index;
        }

        /// <summary>
        /// Checks if the provided index is a valid index within the List.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List.</typeparam>
        /// <param name="self">The List to check.</param>
        /// <param name="index">The index to check for validity.</param>
        /// <returns>True if the index is valid, false otherwise.</returns>
        public static bool IsValidIndex<T>(this List<T> self, int index)
        {
            return self.Count - 1 >= index;
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate from the List.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List.</typeparam>
        /// <param name="array">The List from which to remove elements.</param>
        /// <param name="match">The predicate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the List.</returns>
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

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate from the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array from which to remove elements.</param>
        /// <param name="match">The predicate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the array.</returns>
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

        /// <summary>
        /// Returns the last element of a list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list from which to retrieve the last element.</param>
        /// <returns>The last element in the list.</returns>
        public static T GetLast<T>(this List<T> list)
        {
            if (list == null)
                return default;

            if (list.Count == 0)
                return default;

            return list.ElementAt(list.Count - 1);
        }

        /// <summary>
        /// Returns the last element of an array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array from which to retrieve the last element.</param>
        /// <returns>The last element in the array.</returns>
        public static T GetLast<T>(this T[] list)
        {
            if (list == null)
                return default;

            if (list.Length == 0)
                return default;

            return list.ElementAt(list.Length - 1);
        }

        /// <summary>
        /// Shuffles the elements of the array in random order.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="list">The array to be shuffled.</param>
        public static void Shuffle<T>(this T[] list)
        {
            int count = list.Length;
            while (count > 0)
            {
                count--;
                int index = MathHelper.Next(count + 1);
                T value = list[index];
                list[index] = list[count];
                list[count] = value;
            }
        }

        /// <summary>
        /// Shuffles the elements of the list in random order.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to be shuffled.</param>
        public static void Shuffle<T>(this List<T> list)
        {
            int count = list.Count;
            while (count > 0)
            {
                count--;
                int index = MathHelper.Next(count + 1);
                T value = list[index];
                list[index] = list[count];
                list[count] = value;
            }
        }
    }
}
