using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    public static class MathHelper
    {
        private static System.Random _random;
        private static object syncObj = new object();

        /// <summary>
        /// Calculates the center point of an array of Vector3 points.
        /// </summary>
        /// <param name="vectors">An array of Vector3 points.</param>
        /// <returns>The center point of the provided Vector3 points.</returns>
        public static Vector3 GetCenterOfVectors(Vector3[] vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Length;
        }

        /// <summary>
        /// Calculates the center point of a List of Vector3 points.
        /// </summary>
        /// <param name="vectors">A List of Vector3 points.</param>
        /// <returns>The center point of the provided Vector3 points.</returns>
        public static Vector3 GetCenterOfVectors(List<Vector3> vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Count == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Count;
        }

        /// <summary>
        /// Returns a random integer between the specified minimum and maximum values (inclusive).
        /// </summary>
        /// <param name="min">The minimum value of the random range.</param>
        /// <param name="max">The maximum value of the random range.</param>
        /// <returns>A random integer between the specified minimum and maximum values.</returns>
        public static int Next(int min, int max)
        {
            lock (syncObj)
            {
                if (_random == null)
                    _random = new System.Random(); // Or exception...
                return _random.Next(min, max);
            }
        }

        /// <summary>
        /// Returns a random integer between 0 (inclusive) and the specified maximum value (exclusive).
        /// </summary>
        /// <param name="max">The exclusive maximum value of the random range.</param>
        /// <returns>A random integer between 0 (inclusive) and the specified maximum value (exclusive).</returns>
        public static int Next(int max)
        {
            lock (syncObj)
            {
                if (_random == null)
                    _random = new System.Random(); // Or exception...
                return _random.Next(0, max);
            }
        }

        /// <summary>
        /// Clamps the specified value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="minValue">The minimum value to clamp to.</param>
        /// <param name="maxValue">The maximum value to clamp to.</param>
        /// <returns>The clamped value.</returns>
        public static int Clamp(int value, int minValue, int maxValue)
        {
            return maxValue < value ? maxValue : (value < minValue ? minValue : value);
        }

        /// <summary>
        /// Generates a random double value within the specified range.
        /// </summary>
        /// <param name="min">The minimum value of the range (inclusive).</param>
        /// <param name="max">The maximum value of the range (exclusive).</param>
        /// <returns>A random double value within the specified range.</returns>
        public static double Next(double min, double max)
        {
            lock (syncObj)
            {
                if (_random == null)
                    _random = new System.Random(); // Or exception...
                return (_random.NextDouble() * Math.Abs(max - min)) + min;
            }
        }

        /// <summary>
        /// Generates a random double value within the range of [0, max).
        /// </summary>
        /// <param name="max">The maximum value of the range (exclusive).</param>
        /// <returns>A random double value within the range of [0, max).</returns>
        public static double Next(double max)
        {
            lock (syncObj)
            {
                if (_random == null)
                    _random = new System.Random(); // Or exception...
                return (_random.NextDouble() * Math.Abs(max));
            }
        }

        /// <summary>
        /// Clamps a double value within the specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="minValue">The minimum value of the range.</param>
        /// <param name="maxValue">The maximum value of the range.</param>
        /// <returns>The clamped double value.</returns>
        public static double Clamp(double value, double minValue, double maxValue)
        {
            return maxValue < value ? maxValue : (value < minValue ? minValue : value);
        }

        /// <summary>
        /// Generates a random float value within the specified range.
        /// </summary>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <returns>A random float value within the specified range.</returns>
        public static float Next(float min, float max)
        {
            lock (syncObj)
            {
                if (_random == null)
                    _random = new System.Random(); // Or exception...
                return ((float)_random.NextDouble() * Math.Abs(max - min)) + min;
            }
        }

        /// <summary>
        /// Generates a random float value between 0 (inclusive) and the specified maximum value (exclusive).
        /// </summary>
        /// <param name="max">The maximum value (exclusive) of the range.</param>
        /// <returns>A random float value between 0 (inclusive) and the specified maximum value (exclusive).</returns>
        public static float Next(float max)
        {
            lock (syncObj)
            {
                if (_random == null)
                    _random = new System.Random(); // Or exception...
                return ((float)_random.NextDouble() * Math.Abs(max));
            }
        }

        /// <summary>
        /// Clamps a float value to the specified range.
        /// </summary>
        /// <param name="value">The float value to clamp.</param>
        /// <param name="minValue">The minimum value of the range.</param>
        /// <param name="maxValue">The maximum value of the range.</param>
        /// <returns>The clamped float value within the specified range.</returns>
        public static float Clamp(float value, float minValue, float maxValue)
        {
            return maxValue < value ? maxValue : (value < minValue ? minValue : value);
        }
    }
}
