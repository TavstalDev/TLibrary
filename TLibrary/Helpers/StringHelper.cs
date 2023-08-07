using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    public static class StringHelper
    {
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


        public static string HexConverter(Color color)
        {
            return "#" + ((int)color.r).ToString("X2") + ((int)color.g).ToString("X2") + ((int)color.b).ToString("X2");
        }
    }
}
