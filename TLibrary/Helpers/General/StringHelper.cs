using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.General
{
    /// <summary>
    /// Provides helper methods for working with strings.
    /// </summary>
    public static class StringHelper
    {
        private static readonly string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly string CharsNoNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Returns a random character using letters and numbers.
        /// </summary>
        /// <returns>A random character.</returns>
        public static char GetRandomChar() =>
            Chars.ElementAt(MathHelper.Next(Chars.Length - 1));

        /// <summary>
        /// Creates a random string with the given length.
        /// You can choose if it includes numbers or only letters.
        /// </summary>
        /// <param name="length">How long the string should be. Default is 32.</param>
        /// <param name="includeNums">If true, the string can have numbers. Default is true.</param>
        /// <returns>A random string.</returns>
        public static string Generate(int length = 32, bool includeNums = true)
        {
            string key = "";

            for (int i = 0; i < length; i++)
            {
                var c = includeNums ? Chars.ElementAt(MathHelper.Next(Chars.Length - 1)) : CharsNoNums.ElementAt(MathHelper.Next(CharsNoNums.Length - 1));
                key += c;
            }

            return key;
        }

        /// <summary>
        /// Creates a random string with only lowercase letters.
        /// </summary>
        /// <param name="length">How long the string should be. Default is 32.</param>
        /// <returns>A random string made of lowercase letters only.</returns>
        public static string GenerateLowercase(int length = 32)
        {
            string key = "";
            for (int i = 0; i < length; i++)
            {
                char c = Chars.ToLower().ElementAt(MathHelper.Next(Chars.Length - 1));
                key += c;
            }

            return key;
        }

        /// <summary>
        /// Creates a random string with only uppercase letters.
        /// </summary>
        /// <param name="length">How long the string should be. Default is 32.</param>
        /// <returns>A random string made of uppercase letters only.</returns>
        public static string GenerateUppercase(int length = 32)
        {
            string key = "";
            for (int i = 0; i < length; i++)
            {
                char c = Chars.ToUpper().ElementAt(MathHelper.Next(Chars.Length - 1));
                key += c;
            }

            return key;
        }

        /// <summary>
        /// Creates a random string with only numbers (0-9).
        /// </summary>
        /// <param name="length">How long the string should be. Default is 32.</param>
        /// <returns>A random string made of numbers only.</returns>
        public static string GenerateNumbers(int length = 32)
        {
            string charSet = "0123456789";
            string key = "";

            for (int i = 0; i < length; i++)
            {
                char c = charSet.ToUpper().ElementAt(MathHelper.Next(charSet.Length - 1));
                key += c;
            }

            return key;
        }

        /// <summary>
        /// Turns a DateTime into a string using placeholders.
        /// You can use placeholders like {year}, {month}, {day}, {hour}, {min}, {sec}, and more.
        /// </summary>
        /// <param name="format">The format text with placeholders.</param>
        /// <param name="time">The date and time to format.</param>
        /// <param name="isAnalogue">If true, uses 12-hour clock. Default is false.</param>
        /// <param name="culture">The language for month and day names. Default is "en".</param>
        /// <returns>The formatted date and time as text.</returns>
        public static string GetDateTimeStringByFormat(string format, DateTime time, bool isAnalogue = false, string culture = "en")
        {
            CultureInfo cultureInfo = new CultureInfo(culture);
            return format.Replace("{year}", time.ToString("yyyy", cultureInfo))
                .Replace("{month}", time.ToString("MM", cultureInfo))
                .Replace("{monthname}", time.ToString("MMM", cultureInfo))
                .Replace("{monthfullname}", time.ToString("MMMM", cultureInfo))
                .Replace("{day}", time.ToString("dd", cultureInfo))
                .Replace("{dayname}", time.ToString("ddd", cultureInfo))
                .Replace("{dayfullname}", time.ToString("dddd", cultureInfo))
                .Replace("{hour}", isAnalogue ? time.ToString("hh", cultureInfo) : time.ToString("HH", cultureInfo))
                .Replace("{min}", time.ToString("mm", cultureInfo))
                .Replace("{minute}", time.ToString("mm", cultureInfo))
                .Replace("{sec}", time.ToString("ss", cultureInfo))
                .Replace("{second}", time.ToString("ss", cultureInfo))
                .Replace("{12hoursuffix}", time.ToString("tt", cultureInfo));
        }

        /// <summary>
        /// Returns the day name (like Monday) for the given DateTime in the chosen language.
        /// </summary>
        /// <param name="time">The date and time to get the day name from.</param>
        /// <param name="locale">The language code (like "en" for English, "de" for German).</param>
        /// <returns>The day name as text.</returns>
        public static string GetDayName(this DateTime time, string locale) =>
            time.ToString("dddd", CultureInfo.CreateSpecificCulture(locale));

        /// <summary>
        /// Returns the month name (like January) for the given DateTime in the chosen language.
        /// </summary>
        /// <param name="time">The date and time to get the month name from.</param>
        /// <param name="locale">The language code (like "en" for English, "de" for German).</param>
        /// <returns>The month name as text.</returns>
        public static string GetMonthName(this DateTime time, string locale) =>
            time.ToString("MMMM", CultureInfo.CreateSpecificCulture(locale));

        /// <summary>
        /// Turns a number into a two-digit clock format (e.g. 1 becomes "01", 10 stays "10").
        /// </summary>
        /// <param name="num">The number to format.</param>
        /// <returns>The number as a two-digit text.</returns>
        private static string MakeClockInt(int num) =>
            num > 9 ? num.ToString() : "0" + num;

        /// <summary>
        /// Returns a clock text (like "02:30 PM") from the given DateTime.
        /// </summary>
        /// <param name="time">The date and time to format.</param>
        /// <returns>The clock text.</returns>
        public static string GetClockStringFromDateTime(this DateTime time) =>
            GetDateTimeStringByFormat("{hour}:{min} {12hoursuffix}", time);

        /// <summary>
        /// Turns a DateTime into a text using placeholders.
        /// This is an extension method that you can call directly on a DateTime.
        /// </summary>
        /// <param name="time">The date and time to format.</param>
        /// <param name="locale">The language code (like "en" for English, "de" for German).</param>
        /// <param name="isAnalogue">If true, uses 12-hour clock.</param>
        /// <param name="format">The format text with placeholders.</param>
        /// <returns>The formatted date and time as text.</returns>
        public static string GetDateTimeStringByFormat(this DateTime time, string locale, bool isAnalogue, string format)
        {
            string suffix = string.Empty;
            if (isAnalogue)
                suffix = time.Hour >= 12 ? "PM" : "AM";

            var newHourString = time.Hour > 12 ? MakeClockInt(time.Hour - 12) : MakeClockInt(time.Hour);

            return format.Replace("{year}", time.Year.ToString())
                .Replace("{month}", MakeClockInt(time.Month))
                .Replace("{monthname}", time.GetMonthName(locale))
                .Replace("{day}", MakeClockInt(time.Day))
                .Replace("{dayname}", time.GetDayName(locale))
                .Replace("{hour}", isAnalogue ? newHourString : MakeClockInt(time.Hour))
                .Replace("{min}", MakeClockInt(time.Minute))
                .Replace("{minute}", MakeClockInt(time.Minute))
                .Replace("{sec}", MakeClockInt(time.Second))
                .Replace("{second}", MakeClockInt(time.Second))
                .Replace("{12hoursuffix}", suffix);
        }

        /// <summary>
        /// Turns a TimeSpan into a text using placeholders.
        /// You can use placeholders like {hour}, {min}, {sec}, {milisec}, {12hoursuffix}, and more.
        /// </summary>
        /// <param name="time">The time span to format.</param>
        /// <param name="isAnalogue">If true, uses 12-hour clock.</param>
        /// <param name="format">The format text with placeholders.</param>
        /// <returns>The formatted time as text.</returns>
        public static string GetTimeSpanStringByFormat(this TimeSpan time, bool isAnalogue, string format)
        {
            string suffix = string.Empty;
            if (isAnalogue)
                suffix = time.Hours >= 12 ? "PM" : "AM";

            var newHourString = time.Hours > 12 ? MakeClockInt(time.Hours - 12) : MakeClockInt(time.Hours);

            return format.Replace("{hour}", isAnalogue ? newHourString : MakeClockInt(time.Hours))
                .Replace("{min}", MakeClockInt(time.Minutes))
                .Replace("{minute}", MakeClockInt(time.Minutes))
                .Replace("{sec}", MakeClockInt(time.Seconds))
                .Replace("{second}", MakeClockInt(time.Seconds))
                .Replace("{milisec}", MakeClockInt(time.Milliseconds))
                .Replace("{ms}", MakeClockInt(time.Milliseconds))
                .Replace("{12hoursuffix}", suffix);
        }

        /// <summary>
        /// Converts a Unity Color to a hex string (e.g. "#FF0000").
        /// </summary>
        /// <param name="color">The Color to convert.</param>
        /// <returns>The hex string.</returns>
        public static string HexConverter(Color color) =>
            "#" + ((int)color.r).ToString("X2") + ((int)color.g).ToString("X2") + ((int)color.b).ToString("X2");

        /// <summary>
        /// Turns a hex color text (like "#FF0000") into a Unity Color.
        /// </summary>
        /// <param name="color">The hex color text to convert.</param>
        /// <returns>The Color value.</returns>
        public static Color HexConverter(string color)
        {
            ColorUtility.TryParseHtmlString(color, out Color result);
            return result;
        }

        /// <summary>
        /// Converts a hex color string to a decimal number.
        /// </summary>
        /// <param name="color">The hex color string (e.g. "#FF0000").</param>
        /// <returns>The decimal value.</returns>
        public static int ColorToDecimal(string color) =>
            Convert.ToInt32(color.Replace("#", ""), 16);

        /// <summary>
        /// Converts a Unity Color to a decimal number.
        /// </summary>
        /// <param name="color">The Color to convert.</param>
        /// <returns>The decimal value.</returns>
        public static int ColorToDecimal(Color color) =>
            ColorToDecimal(HexConverter(color));
    }
}
