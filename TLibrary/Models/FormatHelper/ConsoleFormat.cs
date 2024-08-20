using System;

namespace Tavstal.TLibrary.Models.FormatHelper
{
    /// <summary>
    /// Format for console output.
    /// </summary>
    public class ConsoleFormat
    {
        /// <summary>
        /// Key of the format
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Color of the format
        /// </summary>
        public ConsoleColor Color { get; set; }

        public ConsoleFormat() { }

        public ConsoleFormat(string key, ConsoleColor color)
        {
            Key = key;
            Color = color;
        }
    }
}
