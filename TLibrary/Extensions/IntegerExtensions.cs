using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Extensions
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// Clamps the specified integer value between the given minimum and maximum values.
        /// </summary>
        /// <param name="value">The integer value to clamp.</param>
        /// <param name="minValue">The minimum value to clamp to.</param>
        /// <param name="maxValue">The maximum value to clamp to.</param>
        /// <returns>The clamped integer value.</returns>
        public static int Clamp(this int value, int minValue, int maxValue)
        {
            return MathHelper.Clamp(value, minValue, maxValue);
        }

        /// <summary>
        /// Clamps the specified double value between the given minimum and maximum values.
        /// </summary>
        /// <param name="value">The double value to clamp.</param>
        /// <param name="minValue">The minimum value to clamp to.</param>
        /// <param name="maxValue">The maximum value to clamp to.</param>
        /// <returns>The clamped double value.</returns>
        public static double Clamp(this double value, double minValue, double maxValue)
        {
            return MathHelper.Clamp(value, minValue, maxValue);
        }

        /// <summary>
        /// Clamps the specified float value between the given minimum and maximum values.
        /// </summary>
        /// <param name="value">The float value to clamp.</param>
        /// <param name="minValue">The minimum value to clamp to.</param>
        /// <param name="maxValue">The maximum value to clamp to.</param>
        /// <returns>The clamped float value.</returns>
        public static float Clamp(this float value, float minValue, float maxValue)
        {
            return MathHelper.Clamp(value, minValue, maxValue);
        }
    }
}
