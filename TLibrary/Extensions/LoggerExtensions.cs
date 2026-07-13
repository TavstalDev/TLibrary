using System;
using System.Text.RegularExpressions;
using Tavstal.TLibrary.Models.Logging;

namespace Tavstal.TLibrary.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="TLogger"/> that simplify logging at specific levels.
    /// These methods act as shortcuts for <see cref="TLogger.LogRich"/> and <see cref="TLogger.Log"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs a rich formatted message at the <see cref="ELogLevel.DEBUG"/> level.
        /// Rich messages use color codes (e.g., &amp;a, &amp;c) that are processed by the formatter.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void RichDebug(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.LogRich(ELogLevel.DEBUG, message, exception, includePrefixes);

        /// <summary>
        /// Logs a rich formatted message at the <see cref="ELogLevel.INFO"/> level.
        /// Rich messages use color codes (e.g., &amp;a, &amp;c) that are processed by the formatter.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void RichInfo(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.LogRich(ELogLevel.INFO, message, exception, includePrefixes);

        /// <summary>
        /// Logs a rich formatted message at the <see cref="ELogLevel.WARNING"/> level.
        /// Rich messages use color codes (e.g., &amp;a, &amp;c) that are processed by the formatter.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void RichWarning(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.LogRich(ELogLevel.WARNING, message, exception, includePrefixes);

        /// <summary>
        /// Logs a rich formatted message at the <see cref="ELogLevel.ERROR"/> level.
        /// Rich messages use color codes (e.g., &amp;a, &amp;c) that are processed by the formatter.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void RichError(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.LogRich(ELogLevel.ERROR, message, exception, includePrefixes);

        /// <summary>
        /// Logs a rich formatted command message at the <see cref="ELogLevel.COMMAND"/> level.
        /// Before logging, the plugin name (e.g., [PluginName]) and (( )) patterns are removed from the message.
        /// Rich messages use color codes (e.g., &amp;a, &amp;c) that are processed by the formatter.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The command message to log. The plugin name and (( )) patterns are stripped automatically.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void RichCommand(this TLogger logger, string message, Exception? exception = null,
            bool includePrefixes = true)
        {
            string cleanMsg = Regex.Replace(message, @$"\(\(.*?\)\)|\[{logger.PluginName}\]", "");
            logger.LogRich(ELogLevel.COMMAND, cleanMsg, exception, includePrefixes);
        }

        /// <summary>
        /// Logs a plain text message at the <see cref="ELogLevel.DEBUG"/> level with a dark magenta console color.
        /// Unlike the Rich variants, this method does not process color codes in the message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void Debug(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.Log(ELogLevel.DEBUG, message, exception, includePrefixes, ConsoleColor.DarkMagenta);

        /// <summary>
        /// Logs a plain text message at the <see cref="ELogLevel.INFO"/> level with a green console color.
        /// Unlike the Rich variants, this method does not process color codes in the message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void Info(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.Log(ELogLevel.INFO, message, exception, includePrefixes);

        /// <summary>
        /// Logs a plain text message at the <see cref="ELogLevel.WARNING"/> level with a yellow console color.
        /// Unlike the Rich variants, this method does not process color codes in the message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void Warning(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.Log(ELogLevel.WARNING, message, exception, includePrefixes, ConsoleColor.Yellow);

        /// <summary>
        /// Logs a plain text message at the <see cref="ELogLevel.ERROR"/> level with a red console color.
        /// Unlike the Rich variants, this method does not process color codes in the message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void Error(this TLogger logger, string message, Exception? exception = null, bool includePrefixes = true) =>
            logger.Log(ELogLevel.ERROR, message, exception, includePrefixes, ConsoleColor.Red);

        /// <summary>
        /// Logs a plain text command message at the <see cref="ELogLevel.COMMAND"/> level with a dark cyan console color.
        /// Before logging, the plugin name (e.g., [PluginName]) and (( )) patterns are removed from the message.
        /// Unlike the Rich variants, this method does not process color codes in the message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The command message to log. The plugin name and (( )) patterns are stripped automatically.</param>
        /// <param name="exception">An optional exception to attach to the message.</param>
        /// <param name="includePrefixes">When true, the plugin name and log level prefix are included in the output.</param>
        public static void Command(this TLogger logger, string message, Exception? exception = null,
            bool includePrefixes = true)
        {
            string cleanMsg = Regex.Replace(message, @$"\(\(.*?\)\)|\[{logger.PluginName}\]", "");
            logger.Log(ELogLevel.COMMAND, cleanMsg, exception, includePrefixes, ConsoleColor.DarkCyan);
        }
    }
}
