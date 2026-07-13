using System;

namespace Tavstal.TLibrary.Extensions.General
{
    /// <summary>
    /// Provides extension methods for converting <see cref="ConsoleColor"/> values to ANSI escape codes.
    /// </summary>
    public static class ConsoleColorExtensions
    {
        /// <summary>
        /// Converts a <see cref="ConsoleColor"/> to its corresponding ANSI foreground escape code.
        /// </summary>
        /// <param name="color">The console color to convert.</param>
        /// <returns>The ANSI escape sequence string for the foreground color.</returns>
        public static string ToAnsiForeground(this ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => "\x1b[30m",
                ConsoleColor.DarkBlue => "\x1b[34m",
                ConsoleColor.DarkGreen => "\x1b[32m",
                ConsoleColor.DarkCyan => "\x1b[36m",
                ConsoleColor.DarkRed => "\x1b[31m",
                ConsoleColor.DarkMagenta => "\x1b[35m",
                ConsoleColor.DarkYellow => "\x1b[33m",
                ConsoleColor.Gray => "\x1b[37m",
                ConsoleColor.DarkGray => "\x1b[90m",
                ConsoleColor.Blue => "\x1b[94m",
                ConsoleColor.Green => "\x1b[92m",
                ConsoleColor.Cyan => "\x1b[96m",
                ConsoleColor.Red => "\x1b[91m",
                ConsoleColor.Magenta => "\x1b[95m",
                ConsoleColor.Yellow => "\x1b[93m",
                ConsoleColor.White => "\x1b[97m",
                _ => "\x1b[39m" // Default foreground
            };
        }

        /// <summary>
        /// Converts a <see cref="ConsoleColor"/> to its corresponding ANSI background escape code.
        /// </summary>
        /// <param name="color">The console color to convert.</param>
        /// <returns>The ANSI escape sequence string for the background color.</returns>
        public static string ToAnsiBackground(this ConsoleColor color)
        {
            // Background codes are just foreground codes + 10
            return color switch
            {
                ConsoleColor.Black => "\x1b[40m",
                ConsoleColor.DarkBlue => "\x1b[44m",
                ConsoleColor.DarkGreen => "\x1b[42m",
                ConsoleColor.DarkCyan => "\x1b[46m",
                ConsoleColor.DarkRed => "\x1b[41m",
                ConsoleColor.DarkMagenta => "\x1b[45m",
                ConsoleColor.DarkYellow => "\x1b[43m",
                ConsoleColor.Gray => "\x1b[47m",
                ConsoleColor.DarkGray => "\x1b[100m",
                ConsoleColor.Blue => "\x1b[104m",
                ConsoleColor.Green => "\x1b[102m",
                ConsoleColor.Cyan => "\x1b[106m",
                ConsoleColor.Red => "\x1b[101m",
                ConsoleColor.Magenta => "\x1b[105m",
                ConsoleColor.Yellow => "\x1b[103m",
                ConsoleColor.White => "\x1b[107m",
                _ => "\x1b[49m" // Default background
            };
        }
    }
}