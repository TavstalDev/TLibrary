using System;
using System.Collections.Generic;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Extensions.General
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
        public static bool IsValidIndex<T>(this T[]? self, int index)
        {
            if (self == null)
                return false;
            return self.Length - 1 >= index;
        }

        /// <summary>
        /// Checks if the provided index is a valid index within the List.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List.</typeparam>
        /// <param name="self">The List to check.</param>
        /// <param name="index">The index to check for validity.</param>
        /// <returns>True if the index is valid, false otherwise.</returns>
        public static bool IsValidIndex<T>(this List<T>? self, int index)
        {
            if (self == null)
                return false;
            return self.Count - 1 >= index;
        }

        /// <summary>
        /// Checks if the provided index is a valid index within the Dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="self">The Dictionary to check.</param>
        /// <param name="index">The index to check for validity.</param>
        /// <returns>True if the index is valid, false otherwise.</returns>
        public static bool IsValidIndex<TKey, TValue>(this Dictionary<TKey, TValue>? self, int index)
        {
            if (self == null)
                return false;
            return self.Count - 1 >= index;
        }
        
        /// <summary>
        /// Randomly shuffles the elements of the array using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="list">The array to shuffle.</param>
        public static void Shuffle<T>(this T[] list)
        {
            int count = list.Length;
            while (count > 0)
            {
                count--;
                int index = MathHelper.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }
        }
        
        /// <summary>
        /// Randomly shuffles the elements of the List using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List.</typeparam>
        /// <param name="list">The List to shuffle.</param>
        /// <exception cref="ArgumentNullException">Thrown when the list is null.</exception>
        public static void Shuffle<T>(this List<T> list)
        {
            if (list == null)
                throw new ArgumentNullException();

            int count = list.Count;
            while (count > 0)
            {
                count--;
                int index = MathHelper.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }
        }
    }
}
