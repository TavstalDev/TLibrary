using System;
using System.Globalization;
using System.Net.Mail;

namespace Tavstal.TLibrary.Extensions.General
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if two strings are the same, ignoring uppercase and lowercase.
        /// </summary>
        /// <param name="str1">The first string.</param>
        /// <param name="str2">The second string.</param>
        /// <returns>True if the strings match (ignoring case), false if not.</returns>
        public static bool EqualsIgnoreCase(this string str1, string str2) =>
            string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Checks if the string contains a certain word or text, ignoring uppercase and lowercase.
        /// </summary>
        /// <param name="str">The string to search in.</param>
        /// <param name="part">The text to look for.</param>
        /// <returns>True if the text is found (ignoring case), false if not.</returns>
        public static bool ContainsIgnoreCase(this string str, string part) => 
            CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, part, CompareOptions.IgnoreCase) >= 0;

        /// <summary>
        /// Checks if a string is null or empty.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True if the string is null or empty, false if it has text.</returns>
        public static bool IsNullOrEmpty(this string str) =>
            string.IsNullOrEmpty(str);
        
        /// <summary>
        /// Makes the first letter of each word uppercase and the rest lowercase.
        /// </summary>
        /// <param name="str">The string to change.</param>
        /// <returns>The string with capital first letters.</returns>
        public static string Capitalize(this string str) => 
            CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLowerInvariant());
        
        /// <summary>
        /// Makes only the first character of the string uppercase.
        /// </summary>
        /// <param name="input">The string to change.</param>
        /// <returns>The string with the first letter in uppercase.</returns>
        public static string UppercaseFirstChar(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        /// <summary>
        /// Checks if a string is a valid web link (http or https).
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True if it is a valid link, false if not.</returns>
        public static bool IsLink(this string str) =>
            Uri.TryCreate(str, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        /// <summary>
        /// Checks if a string is a valid email address.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True if the string is a valid email address, otherwise false.</returns>
        public static bool IsValidEmail(this string str)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                MailAddress mail = new MailAddress(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
