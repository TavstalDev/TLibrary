using System;
using System.IO;
using System.Text.RegularExpressions;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TLibrary.Models.Plugin
{
    /// <summary>
    /// Logger helper used to log messages to console and log file.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TLogger
    {
        /// <summary>
        /// Name of the plugin that uses the logger.
        /// </summary>
        private readonly string _pluginName;

        /// <summary>
        /// The name of the module associated with the logger.
        /// </summary>
        private readonly string _moduleName;

        /// <summary>
        /// Should show debug messages?
        /// </summary>
        private bool _isDebugEnabled;

        /// <summary>
        /// Should show debug messages?
        /// </summary>
        public bool IsDebugEnabled => _isDebugEnabled;

        /// <summary>
        /// Initializes a new instance of the TLogger class with the specified plugin name and debug mode.
        /// </summary>
        /// <param name="pluginName">The name of the plugin that uses the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        public TLogger(string pluginName, bool isDebugEnabled) : this(pluginName, "", isDebugEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TLogger class with the specified plugin and debug mode.
        /// </summary>
        /// <param name="plugin">The plugin that uses the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        public TLogger(IPlugin plugin, bool isDebugEnabled) : this(plugin.GetPluginName(), "", isDebugEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TLogger class with the specified plugin name, module type, and debug mode.
        /// </summary>
        /// <param name="pluginName">The name of the plugin that uses the logger.</param>
        /// <param name="module">The type of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        public TLogger(string pluginName, Type module, bool isDebugEnabled) : this(pluginName, module.Name,
            isDebugEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TLogger class with the specified plugin, module type, and debug mode.
        /// </summary>
        /// <param name="plugin">The plugin that uses the logger.</param>
        /// <param name="module">The type of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        public TLogger(IPlugin plugin, Type module, bool isDebugEnabled) : this(plugin, module.Name, isDebugEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TLogger class with the specified plugin name, module name, and debug mode.
        /// </summary>
        /// <param name="pluginName">The name of the plugin that uses the logger.</param>
        /// <param name="moduleName">The name of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        public TLogger(string pluginName, string moduleName, bool isDebugEnabled)
        {
            _pluginName = pluginName;
            _moduleName = moduleName;
            _isDebugEnabled = isDebugEnabled;
        }

        /// <summary>
        /// Initializes a new instance of the TLogger class with the specified plugin, module name, and debug mode.
        /// </summary>
        /// <param name="plugin">The plugin that uses the logger.</param>
        /// <param name="moduleName">The name of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        public TLogger(IPlugin plugin, string moduleName, bool isDebugEnabled)
        {
            _pluginName = plugin.GetPluginName();
            _moduleName = moduleName;
            _isDebugEnabled = isDebugEnabled;
        }

        /// <summary>
        /// Static method to create a new instance of the logger.
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="isDebugEnabled"></param>
        /// <returns></returns>
        public static TLogger CreateInstance(string pluginName, bool isDebugEnabled)
        {
            return new TLogger(pluginName, isDebugEnabled);
        }
        
        /// <summary>
        /// Creates a new instance of the TLogger class with the specified plugin name, module name, and debug mode.
        /// </summary>
        /// <param name="pluginName">The name of the plugin that uses the logger.</param>
        /// <param name="moduleName">The name of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        /// <returns>A new instance of the TLogger class.</returns>
        public static TLogger CreateInstance(string pluginName, string moduleName, bool isDebugEnabled)
        {
            return new TLogger(pluginName, moduleName, isDebugEnabled);
        }

        /// <summary>
        /// Creates a new instance of the TLogger class with the specified plugin name, module type, and debug mode.
        /// </summary>
        /// <param name="pluginName">The name of the plugin that uses the logger.</param>
        /// <param name="module">The type of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        /// <returns>A new instance of the TLogger class.</returns>
        public static TLogger CreateInstance(string pluginName, Type module, bool isDebugEnabled)
        {
            return new TLogger(pluginName, module.Name, isDebugEnabled);
        }

        /// <summary>
        /// Static method to create a new instance of the logger.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="isDebugEnabled"></param>
        /// <returns></returns>
        public static TLogger CreateInstance(IPlugin plugin, bool isDebugEnabled)
        {
            return new TLogger(plugin.GetPluginName(), isDebugEnabled);
        }
        
        /// <summary>
        /// Creates a new instance of the TLogger class with the specified plugin, module name, and debug mode.
        /// </summary>
        /// <param name="plugin">The plugin that uses the logger.</param>
        /// <param name="moduleName">The name of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        /// <returns>A new instance of the TLogger class.</returns>
        public static TLogger CreateInstance(IPlugin plugin, string moduleName, bool isDebugEnabled)
        {
            return new TLogger(plugin.GetPluginName(), moduleName, isDebugEnabled);
        }

        /// <summary>
        /// Creates a new instance of the TLogger class with the specified plugin, module type, and debug mode.
        /// </summary>
        /// <param name="plugin">The plugin that uses the logger.</param>
        /// <param name="module">The type of the module associated with the logger.</param>
        /// <param name="isDebugEnabled">A boolean value indicating whether debug mode is enabled.</param>
        /// <returns>A new instance of the TLogger class.</returns>
        public static TLogger CreateInstance(IPlugin plugin, Type module, bool isDebugEnabled)
        {
            return new TLogger(plugin.GetPluginName(), module.Name, isDebugEnabled);
        }

        /// <summary>
        /// Sets the debug mode for the application.
        /// </summary>
        /// <param name="isActive">A boolean value indicating whether the debug mode should be active or not.</param>
        public void SetDebugMode(bool isActive)
        {
            _isDebugEnabled = isActive;
        }
        
        /// <summary>
        /// Logs a rich formatted message to the console and log file.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "&amp;a[INFO] >&amp;f".</param>
        public void LogRich(object message, string prefix = "&a[INFO] >&f")
        {
            string text;
            if (string.IsNullOrEmpty(prefix))
            {
                text = _moduleName.IsNullOrEmpty() ? $"&b[{_pluginName}] {message}" : $"&b[{_pluginName}] [{_moduleName}] {message}";
            }
            else
            {
                text = _moduleName.IsNullOrEmpty() ? $"&b[{_pluginName}] {prefix} {message}" : $"&b[{_pluginName}] [{_moduleName}] {prefix} {message}";
            }

            try
            {
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ",
                        Helpers.General.FormatHelper.ClearFormaters(text)));
                    streamWriter.Close();
                }

                Helpers.General.FormatHelper.SendFormatedConsole(text);
                Console.ResetColor();
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text);
            }
        }

        /// <summary>
        /// Logs a rich formatted warning message to the console and log file.
        /// </summary>
        /// <param name="message">The warning message to be logged.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "&amp;e[WARNING] >&amp;f".</param>
        public void RichWarning(object message, string prefix = "&e[WARNING] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted exception message to the console and log file.
        /// </summary>
        /// <param name="message">The exception message to be logged.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "&amp;6[EXCEPTION] >&amp;f".</param>
        public void RichException(object message, string prefix = "&6[EXCEPTION] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted error message to the console and log file.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "&amp;c[ERROR] >&amp;f".</param>
        public void RichError(object message, string prefix = "&c[ERROR] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted command message to the console and log file.
        /// </summary>
        /// <param name="message">The command message to be logged.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "&amp;9[COMMAND] >&amp;f".</param>
        public void RichCommand(object message, string prefix = "&9[COMMAND] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich formatted debug message to the console and log file if debug mode is enabled.
        /// </summary>
        /// <param name="message">The debug message to be logged.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "&amp;d[DEBUG] >&amp;f".</param>
        public void RichDebug(object message, string prefix = "&d[DEBUG] >&f")
        {
            if (_isDebugEnabled)
                LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a message to the console and log file.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="color">The color of the console text. Defaults to <see cref="ConsoleColor.Green"/>.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "[INFO] >".</param>
        public void Log(object message, ConsoleColor color = ConsoleColor.Green, string prefix = "[INFO] >")
        {
            var text = string.IsNullOrEmpty(prefix) ? $"[{_pluginName}] {message}" : $"[{_pluginName}] {prefix} {message}";
            try
            {
                Console.ForegroundColor = color;
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", text));
                    streamWriter.Close();
                }

                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text.Replace($"[{_pluginName}] ", ""), color);
            }
        }

        /// <summary>
        /// Logs an informational message to the console and log file.
        /// </summary>
        /// <param name="message">The informational message to be logged.</param>
        /// <param name="color">The color of the console text. Defaults to <see cref="ConsoleColor.Green"/>.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "[INFO] >".</param>
        public void Info(object message, ConsoleColor color = ConsoleColor.Green, string prefix = "[INFO] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a warning message to the console and log file.
        /// </summary>
        /// <param name="message">The warning message to be logged.</param>
        /// <param name="color">The color of the console text. Defaults to <see cref="ConsoleColor.Yellow"/>.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "[WARNING] >".</param>
        public void Warning(object message, ConsoleColor color = ConsoleColor.Yellow, string prefix = "[WARNING] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs an exception message to the console and log file.
        /// </summary>
        /// <param name="message">The exception message to be logged.</param>
        /// <param name="color">The color of the console text. Defaults to <see cref="ConsoleColor.DarkYellow"/>.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "[EXCEPTION] >".</param>
        public void Exception(object message, ConsoleColor color = ConsoleColor.DarkYellow, string prefix = "[EXCEPTION] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs an error message to the console and log file.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        /// <param name="color">The color of the console text. Defaults to <see cref="ConsoleColor.Red"/>.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "[ERROR] >".</param>
        public void Error(object message, ConsoleColor color = ConsoleColor.Red, string prefix = "[ERROR] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a debug message to the console and log file if debug mode is enabled.
        /// </summary>
        /// <param name="message">The debug message to be logged.</param>
        /// <param name="color">The color of the console text. Defaults to <see cref="ConsoleColor.Magenta"/>.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "[DEBUG] >".</param>
        public void Debug(object message, ConsoleColor color = ConsoleColor.Magenta, string prefix = "[DEBUG] >")
        {
            if (_isDebugEnabled)
                Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a late initialization message to the console and log file.
        /// </summary>
        public void LateInit()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            string text = $"\n" +
                          "╔═══════════════════════════════════════╗\n" + 
                         $"║       INITIATING: {_pluginName,-20}║\n" + 
                          "╚═══════════════════════════════════════╝\n";
            try
            {
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", text));
                    streamWriter.Close();
                }

                Console.WriteLine(text);
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Logs a command message to the console and log file after processing its format.
        /// </summary>
        /// <param name="message">The command message to be logged.</param>
        /// <param name="color">The color of the console text. Defaults to <see cref="ConsoleColor.Blue"/>.</param>
        /// <param name="prefix">The prefix for the log message. Defaults to "[Command] >".</param>
        public void Command(object message, ConsoleColor color = ConsoleColor.Blue, string prefix = "[Command] >")
        {
            string msg = message.ToString().Replace("((", "{").Replace("))", "}").Replace("[TShop]", "");
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
