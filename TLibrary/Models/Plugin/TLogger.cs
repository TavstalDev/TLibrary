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
        /// Logs a rich message to the console and the log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public void LogRich(object message, string prefix = "&a[INFO] >&f")
        {
            string text;
            if (_moduleName.IsNullOrEmpty())
                text = $"&b[{_pluginName}] {prefix} {message}";
            else
                text = $"&b[{_pluginName}] [{_moduleName}] {prefix} {message}";
            try
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ",
                        Helpers.General.FormatHelper.ClearFormaters(text)));
                    streamWriter.Close();
                }

                Helpers.General.FormatHelper.SendFormatedConsole(text);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text);
            }
        }

        /// <summary>
        /// Logs a rich message to the console as warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public void LogRichWarning(object message, string prefix = "&e[WARNING] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich message to the console as exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public void LogRichException(object message, string prefix = "&6[EXCEPTION] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich message to the console as error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public void LogRichError(object message, string prefix = "&c[ERROR] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich message to the console as command response
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public void LogRichCommand(object message, string prefix = "&9[COMMAND] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich message to the console as debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public void LogRichDebug(object message, string prefix = "&d[DEBUG] >&f")
        {
            if (_isDebugEnabled)
                LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a message to the console and log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public void Log(object message, ConsoleColor color = ConsoleColor.Green, string prefix = "[INFO] >")
        {

            string text = $"[{_pluginName}] {prefix} {message}";
            try
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", text));
                    streamWriter.Close();
                }

                Console.WriteLine(text);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text.Replace($"[{_pluginName}] ", ""), color);
            }
        }

        /// <summary>
        /// Logs a message to the console as warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public void LogWarning(object message, ConsoleColor color = ConsoleColor.Yellow, string prefix = "[WARNING] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a message to the console as exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public void LogException(object message, ConsoleColor color = ConsoleColor.DarkYellow,
            string prefix = "[EXCEPTION] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a message to the console as error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public void LogError(object message, ConsoleColor color = ConsoleColor.Red, string prefix = "[ERROR] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a message to the console as command response
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public void LogDebug(object message, ConsoleColor color = ConsoleColor.Magenta, string prefix = "[DEBUG] >")
        {
            if (_isDebugEnabled)
                Log(message, color, prefix);
        }

        /// <summary>
        /// Logs the late init message to the console and log file
        /// </summary>
        public void LogLateInit()
        {
            ConsoleColor oldFgColor = Console.ForegroundColor;
            ConsoleColor oldBgColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            string text = $"######## {_pluginName} LATE INIT ########";
            try
            {
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", $"[{_pluginName}] {text}"));
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
        /// Logs the late init message to the console and log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public void LogCommand(object message, ConsoleColor color = ConsoleColor.Blue, string prefix = "[Command] >")
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
