using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers;

namespace Tavstal.TLibrary.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool ContainsIgnoreCase(this string str, string part)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, part, CompareOptions.IgnoreCase) >= 0;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null)
                return true;

            return str.Length == 0;
        }

        public static string Capitalize(this string str)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLowerInvariant());
        }

        public static bool IsLink(this string str)
        {
            return Uri.TryCreate(str, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

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
    }
}
