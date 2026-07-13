using System;
using System.IO;

namespace Tavstal.TLibrary.Helpers.General
{
    /// <summary>
    /// Internal helper class for logging.
    /// </summary>
    internal static class LoggerHelper
    {
        private static readonly string Name = "TLibrary";
        private static readonly bool IsDebug = false;

        /// <summary>
        /// Logs a message with a specified color and prefix.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color to use for the log message.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void Log(object message, ConsoleColor color = ConsoleColor.Green, string prefix = "[INFO] >")
        {

            string text = $"[{Name}] {prefix} {message}";
            try
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory, Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", text));
                    streamWriter.Close();
                }
                Console.WriteLine(text);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log("[LOG-FAILED]: " + text.Replace($"[{Name}] ", ""), color);
            }
        }

        /// <summary>
        /// Logs a warning message with a specified color and prefix.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        /// <param name="color">The color to use for the log message.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogWarning(object message, ConsoleColor color = ConsoleColor.Yellow, string prefix = "[WARNING] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs an error message with a specified color and prefix.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="color">The color to use for the log message.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogError(object message, ConsoleColor color = ConsoleColor.Red, string prefix = "[ERROR] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a debug message with a specified color and prefix if debug mode is enabled.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        /// <param name="color">The color to use for the log message.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogDebug(object message, ConsoleColor color = ConsoleColor.Magenta, string prefix = "[DEBUG] >")
        {
            if (IsDebug)
                Log(message, color, prefix);
        }
    }
}
