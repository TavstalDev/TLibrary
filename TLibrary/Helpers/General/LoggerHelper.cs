using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

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
        /// Logs a rich formatted message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogRich(object message, string prefix = "&a[INFO] >&f")
        {
            string text = $"&b[{Name}] {prefix} {message}";
            try
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory, Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", FormatHelper.ClearFormaters(text)));
                    streamWriter.Close();
                }
                FormatHelper.SendFormatedConsole(text);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text);
            }
        }

        /// <summary>
        /// Logs a rich formatted warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogRichWarning(object message, string prefix = "&e[WARNING] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted exception message.
        /// </summary>
        /// <param name="message">The exception message to log.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogRichException(object message, string prefix = "&6[EXCEPTION] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogRichError(object message, string prefix = "&c[ERROR] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted command message.
        /// </summary>
        /// <param name="message">The command message to log.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogRichCommand(object message, string prefix = "&9[COMMAND] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted debug message if debug mode is enabled.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogRichDebug(object message, string prefix = "&d[DEBUG] >&f")
        {
            if (IsDebug)
                LogRich(message, prefix);
        }

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
                Rocket.Core.Logging.Logger.Log(text.Replace($"[{Name}] ", ""), color);
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
        /// Logs an exception message with a specified color and prefix.
        /// </summary>
        /// <param name="message">The exception message to log.</param>
        /// <param name="color">The color to use for the log message.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogException(object message, ConsoleColor color = ConsoleColor.DarkYellow, string prefix = "[EXCEPTION] >")
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

        /// <summary>
        /// Logs a late initialization message.
        /// </summary>
        public static void LogLateInit()
        {
            ConsoleColor oldFgColor = Console.ForegroundColor;
            ConsoleColor oldBgColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            string text = $"######## {Name} LATE INIT ########";
            try
            {
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory, Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", $"[{Name}] {text}"));
                    streamWriter.Close();
                }
                Console.WriteLine(text);
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text);
            }
            Console.ForegroundColor = oldFgColor;
            Console.BackgroundColor = oldBgColor;
        }

        /// <summary>
        /// Logs a command message with a specified color and prefix.
        /// </summary>
        /// <param name="message">The command message to log.</param>
        /// <param name="color">The color to use for the log message.</param>
        /// <param name="prefix">The prefix to use for the log message.</param>
        public static void LogCommand(object message, ConsoleColor color = ConsoleColor.Blue, string prefix = "[Command] >")
        {
            string msg = message.ToString().Replace("((", "{").Replace("))", "}");
            int amount = msg.Split('{').Length;
            for (int i = 0; i < amount; i++)
            {
                Regex regex = new Regex($"{Regex.Escape("{")}(.*?){Regex.Escape("}")}", RegexOptions.RightToLeft);
                msg = regex.Replace(msg, "{" + "}");
            }

            Log(msg.Replace("{", "").Replace("}", ""), color, prefix);
        }
    }
}
