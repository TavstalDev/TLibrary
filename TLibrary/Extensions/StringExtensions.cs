using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Compares two strings for equality, ignoring case.
        /// </summary>
        /// <param name="str1">The first string to compare.</param>
        /// <param name="str2">The second string to compare.</param>
        /// <returns><c>true</c> if the strings are equal, ignoring case; otherwise, <c>false</c>.</returns>
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Determines whether the given substring is present within the string, ignoring case.
        /// </summary>
        /// <param name="str">The string to search within.</param>
        /// <param name="part">The substring to search for.</param>
        /// <returns><c>true</c> if the substring is found within the string, ignoring case; otherwise, <c>false</c>.</returns>
        public static bool ContainsIgnoreCase(this string str, string part)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, part, CompareOptions.IgnoreCase) >= 0;
        }

        /// <summary>
        /// Determines whether a string is null or empty.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns><c>true</c> if the string is null or empty; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null)
                return true;

            return str.Length == 0;
        }

        /// <summary>
        /// Capitalizes the first letter of a string.
        /// </summary>
        /// <param name="str">The string to capitalize.</param>
        /// <returns>A new string with the first letter capitalized.</returns>
        public static string Capitalize(this string str)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLowerInvariant());
        }

        /// <summary>
        /// Checks if a string is a valid hyperlink.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True if the string is a valid hyperlink, otherwise false.</returns>
        public static bool IsLink(this string str)
        {
            return Uri.TryCreate(str, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Checks if a string is a valid email address.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True if the string is a valid email address, otherwise false.</returns>
        public static bool IsValidEmail(this string str)
        {
            try
            {
                MailAddress mail = new MailAddress(str);
                return mail != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a random string of the specified length.
        /// </summary>
        /// <param name="length">The length of the random string to generate.</param>
        /// <returns>A random string of the specified length.</returns>
        public static string GenerateRandomString(int length)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string key = "";

            for (int i = 0; i < length; i++)
            {
                char c = chars.ElementAt(MathHelper.Next(0, chars.Length - 1));
                key += c;
            }

            return key;
        }

        /// <summary>
        /// Generates a random character.
        /// </summary>
        /// <returns>A random character.</returns>
        public static char GetRandomChar()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return chars.ElementAt(MathHelper.Next(chars.Length - 1));
        }

        /// <summary>
        /// Shuffles the characters in a string randomly.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The shuffled string.</returns>
        public static string Shuffle(this string str)
        {
            char[] array = str.ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = MathHelper.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }
    }
}
