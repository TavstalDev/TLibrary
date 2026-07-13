using Tavstal.TLibrary.Models.Logging;

namespace Tavstal.TLibrary.Models.Plugin
{
    /// <summary>
    /// Defines the core contract for all plugins, providing lifecycle management,
    /// logging, localization, and utility methods.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Called when the plugin is loaded.
        /// </summary>
        void OnLoad();

        /// <summary>
        /// Called when the plugin is unloaded.
        /// </summary>
        void OnUnLoad();

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        /// <returns>The display name of the plugin.</returns>
        string GetPluginName();

        /// <summary>
        /// Gets the logger instance used by the plugin.
        /// </summary>
        /// <returns>The <see cref="TLogger"/> instance for this plugin.</returns>
        TLogger GetLogger();

        /// <summary>
        /// Gets the current log level of the plugin.
        /// </summary>
        /// <returns>The active <see cref="ELogLevel"/>.</returns>
        ELogLevel GetLogLevel();

        /// <summary>
        /// Verifies and initializes the plugin's required files and directories.
        /// </summary>
        void CheckPluginFiles();

        /// <summary>
        /// Schedules an action to be executed after a specified delay.
        /// </summary>
        /// <param name="delay">The delay in seconds before the action is invoked.</param>
        /// <param name="action">The action to execute.</param>
        void InvokeAction(float delay, System.Action action);

        /// <summary>
        /// Localizes a translation key with optional prefix and format arguments.
        /// </summary>
        /// <param name="addPrefix">Whether to add a plugin prefix to the localized string.</param>
        /// <param name="translationKey">The key to look up in the locale.</param>
        /// <param name="args">Optional format arguments for the localized string.</param>
        /// <returns>The localized and formatted string.</returns>
        string Localize(bool addPrefix, string translationKey, params object[] args);

        /// <summary>
        /// Localizes a translation key with format arguments.
        /// </summary>
        /// <param name="translationKey">The key to look up in the locale.</param>
        /// <param name="args">Optional format arguments for the localized string.</param>
        /// <returns>The localized and formatted string.</returns>
        string Localize(string translationKey, params object[] args);
    }
}
