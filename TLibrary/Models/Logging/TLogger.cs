using System;
using System.IO;
using Tavstal.TLibrary.Extensions.General;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Models.Logging
{
    /// <summary>
    /// Logger helper used to log messages to console and log file.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TLogger : IDisposable
    {
        /// <summary>
        /// Name of the plugin that uses the logger.
        /// </summary>
        private readonly string _pluginName;
        
        public string PluginName => _pluginName;

        /// <summary>
        /// The name of the module associated with the logger.
        /// </summary>
        private readonly string _moduleName;

        /// <summary>
        /// Should show debug messages?
        /// </summary>
        private ELogLevel _logLevel;

        /// <summary>
        /// Should show debug messages?
        /// </summary>
        public ELogLevel LogLevel => _logLevel;
        
        public delegate void LogLevelChangedHandler(string pluginName, ELogLevel newLevel);
        public static event LogLevelChangedHandler? OnLogLevelChanged;
        public static void SetLogLevel(string pluginName, ELogLevel logLevel) =>
            OnLogLevelChanged?.Invoke(pluginName, logLevel);
        
        public TLogger(string pluginName, ELogLevel logLevel) : this(pluginName, "", logLevel) { }
        
        public TLogger(IPlugin plugin,  ELogLevel logLevel) : this(plugin.GetPluginName(), "", logLevel) { }
        
        public TLogger(string pluginName, Type module,  ELogLevel logLevel) : this(pluginName, module.Name,
            logLevel) { }
        
        public TLogger(IPlugin plugin, Type module, ELogLevel logLevel) : this(plugin, module.Name, logLevel) { }
        
        public TLogger(IPlugin plugin, string moduleName, ELogLevel logLevel) : this(plugin.GetPluginName(), moduleName, logLevel) { }
        
        public TLogger(string pluginName, string moduleName,  ELogLevel logLevel)
        {
            _pluginName = pluginName;
            _moduleName = moduleName;
            _logLevel = logLevel;
            OnLogLevelChanged += HandleLogLevelChanged;
        }
        
        public void Dispose()
        {
            OnLogLevelChanged -= HandleLogLevelChanged;
        }
        
        public void LogRich(ELogLevel logLevel, string message, Exception? exception = null, bool includePrefixes = true)
        {
            if (_logLevel > logLevel)
                return;

            string text;
            string color = GetLogLevelColor(logLevel);
            if (includePrefixes)
            {
                string prefix = GetLogLevelPrefix(logLevel);
                string moduleName = string.IsNullOrEmpty(_moduleName) ? "" : $" [{_moduleName}]";
                text = $"{color}[{_pluginName}]{moduleName} {prefix} {message}";
            }
            else
                text = $"{color}{message}";

            if (exception != null)
                text += Environment.NewLine + $"└── Exception: {exception}";

            try
            {
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", FormatHelper.ClearFormaters(text)));
                    streamWriter.Close();
                }

                Console.WriteLine(FormatHelper.FormatTextConsole(text));
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log("[LOG-FAILED]: " + text);
            }
        }
        
        public void Log(ELogLevel logLevel, string message, Exception? exception = null, bool includePrefixes = true, ConsoleColor color = ConsoleColor.Green)
        {
            if (_logLevel > logLevel)
                return;
            
            string text;
            if (includePrefixes)
            {
                string prefix = GetLogLevelPrefix(logLevel);
                string moduleName = string.IsNullOrEmpty(_moduleName) ? "" : $" [{_moduleName}]";
                text = $"[{_pluginName}]{moduleName} {prefix} {message}";
            }
            else
                text = message;

            if (exception != null)
                text += Environment.NewLine + $"└── Exception: {exception}";
            
            try
            {
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory,
                           Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", text));
                    streamWriter.Close();
                }

                Console.WriteLine(color.ToAnsiForeground() + text + "\x1b[0m");
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log("[LOG-FAILED]: " + text, color);
            }
        }
        
        private void HandleLogLevelChanged(string pluginName, ELogLevel newLevel)
        {
            if (pluginName != _pluginName || newLevel == _logLevel)
                return;
            _logLevel = newLevel;
        }

        private string GetLogLevelPrefix(ELogLevel logLevel)
        {
            return logLevel switch
            {
                ELogLevel.DEBUG => "[DEBUG] >",
                ELogLevel.INFO => "[INFO] >",
                ELogLevel.WARNING => "[WARN] >",
                ELogLevel.ERROR => "[ERROR] >",
                ELogLevel.COMMAND => "[COMMAND] >",
                _ => string.Empty
            };
        }

        private string GetLogLevelColor(ELogLevel logLevel)
        {
            return logLevel switch
            {
                ELogLevel.DEBUG => "&d",
                ELogLevel.INFO => "&a",
                ELogLevel.WARNING => "&6",
                ELogLevel.ERROR => "&c",
                ELogLevel.COMMAND => "&3",
                _ => string.Empty
            };
        }
    }
}
