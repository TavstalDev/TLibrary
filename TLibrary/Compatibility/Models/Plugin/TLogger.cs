using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Logger helper used to log messages to console and log file.
    /// </summary>
    public class TLogger
    {
        /// <summary>
        /// Name of the plugin that uses the logger.
        /// </summary>
        private readonly string _pluginName;
        /// <summary>
        /// Shpould show debug messages?
        /// </summary>
        private readonly bool _isDebugEnabled;

        public TLogger(string pluginName, bool isDebugEnabled)
        {
            _pluginName = pluginName;
            _isDebugEnabled = isDebugEnabled;
        }

        public TLogger(IPlugin plugin, bool isDebugEnabled)
        {
            _pluginName = plugin.GetPluginName();
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
        /// Logs a rich message to the console and the log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public void LogRich(object message, string prefix = "&a[INFO] >&f")
        {
            string text = string.Format("&b[{0}] {1} {2}", _pluginName, prefix, message.ToString());
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

            string text = string.Format("[{0}] {1} {2}", _pluginName, prefix, message.ToString());
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
        public void LogException(object message, ConsoleColor color = ConsoleColor.DarkYellow, string prefix = "[EXCEPTION] >")
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
            ConsoleColor oldFGColor = Console.ForegroundColor;
            ConsoleColor oldBGColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            string text = $"######## {_pluginName} LATE INIT ########";
            try
            {
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory, Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", string.Format("[{0}] {1}", _pluginName, text)));
                    streamWriter.Close();
                }
                Console.WriteLine(text);
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text);
            }
            Console.ForegroundColor = oldFGColor;
            Console.BackgroundColor = oldBGColor;
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
                Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape("{"), Regex.Escape("}")), RegexOptions.RightToLeft);
                msg = regex.Replace(msg, "{" + "}");
            }

            Log(msg.Replace("{", "").Replace("}", ""), color, prefix);
        }
    }
}
