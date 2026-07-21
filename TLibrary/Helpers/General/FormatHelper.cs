using System.Collections.Generic;
using Tavstal.TLibrary.Models;

namespace Tavstal.TLibrary.Helpers.General
{
    /// <summary>
    /// Helper used for formatting rich text.
    /// </summary>
    public static class FormatHelper
    {
        /// <summary>
        /// The default list of text color and decoration formats used for rich text.
        /// </summary>
        public static readonly List<TextFormat> DefaultFormats = new List<TextFormat>()
        {
            new TextFormat("&0", "<color=#000000>", "</color>", false),
            new TextFormat("&1", "<color=#0000AA>", "</color>", false),
            new TextFormat("&2", "<color=#00AA00>", "</color>", false),
            new TextFormat("&3", "<color=#00AAAA>", "</color>", false),
            new TextFormat("&4", "<color=#AA0000>", "</color>", false),
            new TextFormat("&5", "<color=#AA00AA>", "</color>", false),
            new TextFormat("&6", "<color=#FFAA00>", "</color>", false),
            new TextFormat("&7", "<color=#AAAAAA>", "</color>", false),
            new TextFormat("&8", "<color=#555555>", "</color>", false),
            new TextFormat("&9", "<color=#5555FF>", "</color>", false),
            new TextFormat("&a", "<color=#55FF55>", "</color>", false),
            new TextFormat("&b", "<color=#55FFFF>", "</color>", false),
            new TextFormat("&c", "<color=#FF5555>", "</color>", false),
            new TextFormat("&d", "<color=#FF55FF>", "</color>", false),
            new TextFormat("&e", "<color=#FFFF55>", "</color>", false),
            new TextFormat("&f", "<color=#FFFFFF>", "</color>", false),
            new TextFormat("&l", "<b>", "</b>", true),
            new TextFormat("&o", "<i>", "</i>", true),
        };

        /// <summary>
        /// The default list of console color formats.
        /// </summary>
        public static readonly Dictionary<string, string> ConsoleFormats = new Dictionary<string, string>
        {
            {"&0", "\x1b[30m"},   // Black
            {"&1", "\x1b[34m"},   // Dark Blue
            {"&2", "\x1b[32m"},   // Dark Green
            {"&3", "\x1b[36m"},   // Dark Cyan
            {"&4", "\x1b[31m"},   // Dark Red
            {"&5", "\x1b[35m"},   // Dark Magenta
            {"&6", "\x1b[33m"},   // Gold/Orange (Dark Yellow)
            {"&7", "\x1b[37m"},   // Gray
            {"&8", "\x1b[90m"},   // Dark Gray (Bright Black)
            {"&9", "\x1b[94m"},   // Blue
            {"&a", "\x1b[92m"},   // Green
            {"&b", "\x1b[96m"},   // Cyan
            {"&c", "\x1b[91m"},   // Red
            {"&d", "\x1b[95m"},   // Magenta
            {"&e", "\x1b[93m"},   // Yellow
            {"&f", "\x1b[97m"},   // White
            {"&r", "\x1b[0m"},    // Reset
        };

        /// <summary>
        /// Formats the text using the default color and decoration codes.
        /// </summary>
        /// <param name="text">The text with format codes (e.g. "&amp;cHello").</param>
        /// <returns>The formatted rich text string.</returns>
        public static string FormatTextV2(string text)
        {
            string formated = string.Empty;
            List<TextFormat> activeFormats = new List<TextFormat>();

            char lastChar = ' ';
            bool isHex = false;
            string hexString = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                char s = text[i];
                // Should be formatted
                if (lastChar == '&' && s != ' ')
                {
                    string key = new string(new[] { lastChar, s });
                    // &r means reset, resets the format if it has active formats already
                    if (key == "&r")
                    {
                        if (activeFormats.Count > 0)
                        {
                            activeFormats.Reverse();
                            foreach (TextFormat f in activeFormats)
                                formated += f.EndTag;
                            activeFormats = new List<TextFormat>();
                        }
                    }
                    else if (s == '#') // Hex found, tries to format to hex color
                    {
                        isHex = true;
                        s = '&'; // sets current char to & because it will be later on set to lastChar
                    }
                    else if (isHex && hexString.Length != 6)
                    {
                        hexString += s;

                        if (hexString.Length == 6)
                        {
                            if (!int.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out int _))
                            {
                                isHex = false;
                                hexString = string.Empty;
                            }
                        }
                        s = '&'; // sets current char to & because it will be later on set to lastChar
                    }
                    else
                    {
                        TextFormat newFormat = isHex ? 
                            new TextFormat(null, $"<color=#{hexString}>", "</color>", false) 
                            : 
                            DefaultFormats.Find(x => x.Key == key);

                        if (newFormat != null)
                        {
                            if (activeFormats.Count > 0)
                            {
                                activeFormats.Reverse();
                                foreach (TextFormat f in activeFormats)
                                    formated += f.EndTag;
                            }

                            TextFormat anotherFormat = activeFormats.Find(x => x.IsDecoration != newFormat.IsDecoration);
                            if (anotherFormat != null)
                            {
                                activeFormats = new List<TextFormat>()
                                {
                                    newFormat,
                                    anotherFormat
                                };
                                formated += newFormat.StartTag + anotherFormat.StartTag;
                            }
                            else
                            {
                                activeFormats = new List<TextFormat>()
                                {
                                    newFormat,
                                };
                                formated += newFormat.StartTag;
                            }

                            // If the currentFormat is hex then it ads the current char because normaly the current char should be the part of the key.
                            // For example: I would like to TEXT to be white, so I combine &#FFFFFF + TEXT. But If I remove this function this will happen:
                            // &#FFFFFFTEXT -> EXT
                            if (isHex)
                            {
                                isHex = false;
                                hexString = string.Empty;
                                formated += s;
                            }
                        }
                    }
                    lastChar = s;
                }
                else if (isHex) // Empty char found while trying to build a hex string
                {
                    isHex = false;
                    hexString = string.Empty;
                }
                else // Ads the character to the formatted string
                {
                    lastChar = s;
                    if (s != '&')
                        formated += s;
                }

                // If it gets to the end of the string, it will close the formatting
                if (i == text.Length - 1 && activeFormats.Count > 0)
                {
                    //int endIndex = formated.Length - 1;
                    //formated += s;
                    foreach (TextFormat f in activeFormats)
                        formated += f.EndTag;
                }
            }

            return formated;
        }
        
        public static string FormatTextConsole(string text)
        {
            foreach (var format in ConsoleFormats)
                text = text.Replace(format.Key, format.Value);
            return text + "\x1b[0m";
        }

        /// <summary>
        /// Removes all format codes from the text, leaving only plain text.
        /// </summary>
        /// <param name="text">The text with format codes.</param>
        /// <returns>The plain text without format codes.</returns>
        public static string ClearFormaters(string text)
        {
            string formated = string.Empty;
            char lastChar = ' ';
            foreach (var s in text)
            {
                if (lastChar == '&' && s != ' ')
                {
                    lastChar = s;
                }
                else // Ads the character to the formatted string
                {
                    lastChar = s;
                    if (s != '&')
                        formated += s;
                }
            }

            return formated;
        }
    }
}
