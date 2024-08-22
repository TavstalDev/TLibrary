using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.General
{
    public static class StringHelper
    {
        private static readonly string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly string _charsNoNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static char GetRandomChar()
        {
            return _chars.ElementAt(MathHelper.Next(_chars.Length - 1));
        }

        public static string Generate(int length = 32, bool includeNums = true)
        {
            string key = "";

            for (int i = 0; i < length; i++)
            {
                char c;
                if (includeNums)
                    c = _chars.ElementAt(MathHelper.Next(_chars.Length - 1));
                else
                    c = _charsNoNums.ElementAt(MathHelper.Next(_charsNoNums.Length - 1));

                if (key.Contains(c))
                    i--;
                else
                    key += c;
            }

            return key;
        }

        public static string GenerateLowercase(int length = 32)
        {
            string key = "";

            for (int i = 0; i < length; i++)
            {
                char c = _chars.ToLower().ElementAt(MathHelper.Next(_chars.Length - 1));
                if (key.Contains(c))
                    i--;
                else
                    key += c;
            }

            return key;
        }

        public static string GenerateUppercase(int length = 32)
        {
            string key = "";

            for (int i = 0; i < length; i++)
            {
                char c = _chars.ToUpper().ElementAt(MathHelper.Next(_chars.Length - 1));
                if (key.Contains(c))
                    i--;
                else
                    key += c;
            }

            return key;
        }

        public static string GenerateNumbers(int length = 32)
        {
            string charSet = "0123456789";
            string key = "";

            for (int i = 0; i < length; i++)
            {
                char c = charSet.ToUpper().ElementAt(MathHelper.Next(charSet.Length - 1));
                if (key.Contains(c))
                    i--;
                else
                    key += c;
            }

            return key;
        }

        /// <summary>
        /// Get date time string by format and culture.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="time"></param>
        /// <param name="isAnalogue">should use 12 hour clock?</param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
        /// Gets the name of the day for the specified date and time.
        /// </summary>
        /// <param name="time">The DateTime object representing the date and time.</param>
        /// <param name="locale"></param>
        /// <returns>The name of the day for the specified date and time.</returns>
        public static string GetDayName(this DateTime time, string locale)
        {
            return time.ToString("dddd", CultureInfo.CreateSpecificCulture(locale));
        }

        /// <summary>
        /// Gets the name of the month for the specified date and time.
        /// </summary>
        /// <param name="time">The DateTime object representing the date and time.</param>
        /// <param name="locale"></param>
        /// <returns>The name of the month for the specified date and time.</returns>
        public static string GetMonthName(this DateTime time, string locale)
        {
            return time.ToString("MMMM", CultureInfo.CreateSpecificCulture(locale));
        }

        /// <summary>
        /// Converts an integer value to a formatted clock representation (1 - > 01, 10 -> 10).
        /// </summary>
        /// <param name="num">The integer value representing hours and minutes.</param>
        /// <returns>The formatted clock representation as a string.</returns>
        private static string MakeClockInt(int num)
        {
            return num > 9 ? num.ToString() : "0" + num;
        }

        /// <summary>
        /// Gets the formatted clock string (HH:mm) from the specified DateTime object.
        /// </summary>
        /// <param name="time">The DateTime object representing the date and time.</param>
        /// <returns>The formatted clock string as a string.</returns>
        public static string GetClockStringFromDateTime(this DateTime time)
        {
            return GetDateTimeStringByFormat("{hour}:{min} {12hoursuffix}", time);
        }

        /// <summary>
        /// Gets a formatted date and time string based on the specified format and DateTime object.
        /// </summary>
        /// <param name="format">The custom format string for date and time formatting.</param>
        /// <param name="time">The DateTime object representing the date and time.</param>
        /// <param name="locale"></param>
        /// <param name="isAnalogue"></param>
        /// <returns>The formatted date and time string based on the provided format.</returns>
        public static string GetDateTimeStringByFormat(this DateTime time, string locale, bool isAnalogue, string format)
        {
            string suffix = string.Empty;
            if (isAnalogue)
            {
                if (time.Hour > 12)
                    suffix = "PM";
                else
                    suffix = "AM";
            }

            string newHourString;
            if (time.Hour > 12)
                newHourString = MakeClockInt(time.Hour - 12);
            else
                newHourString = MakeClockInt(time.Hour);

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
        /// Gets a formatted time span string based on the specified format and TimeSpan object.
        /// </summary>
        /// <param name="isAnalogue"></param>
        /// <param name="format">The custom format string for time span formatting.</param>
        /// <param name="time">The TimeSpan object representing the duration of time.</param>
        /// <returns>The formatted time span string based on the provided format.</returns>
        public static string GetTimeSpanStringByFormat(this TimeSpan time, bool isAnalogue, string format)
        {
            string suffix = string.Empty;
            if (isAnalogue)
            {
                if (time.Hours > 12)
                    suffix = "PM";
                else
                    suffix = "AM";
            }

            string newHourString;
            if (time.Hours > 12)
                newHourString = MakeClockInt(time.Hours - 12);
            else
                newHourString = MakeClockInt(time.Hours);

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
        /// Converts color to hex string.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string HexConverter(Color color)
        {
            return "#" + ((int)color.r).ToString("X2") + ((int)color.g).ToString("X2") + ((int)color.b).ToString("X2");
        }

        /// <summary>
        /// Converts hex string to color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color HexConverter(string color)
        {
            ColorUtility.TryParseHtmlString(color, out Color result);
            return result;
        }

        /// <summary>
        /// Convert hex to decimal
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ColorToDecimal(string color)
        {
            return Convert.ToInt32(color.Replace("#", ""), 16);
        }

        /// <summary>
        /// Convert unity color to decimal
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ColorToDecimal(Color color)
        {
            return ColorToDecimal(HexConverter(color));
        }
    }
}
