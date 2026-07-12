using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Tavstal.TLibrary.Models.Hooks
{
    /// <summary>
    /// Provides access to a plugin's configuration and localization.
    /// Used by hook handlers to retrieve plugin-specific settings and translated strings.
    /// </summary>
    public interface IPluginProvider
    {
        /// <summary>
        /// Retrieves a typed value from the plugin's configuration by its key.
        /// </summary>
        /// <typeparam name="T">The expected type of the config value.</typeparam>
        /// <param name="variableName">The key (property name) of the config entry to retrieve.</param>
        /// <returns>The deserialized config value of type <typeparamref name="T"/>, or <see langword="null"/> if the key does not exist.</returns>
        [CanBeNull] T GetConfigValue<T>(string variableName);
        
        /// <summary>
        /// Returns the entire plugin configuration as a <see cref="JObject"/>.
        /// </summary>
        /// <returns>A <see cref="JObject"/> representing the full config, or <see langword="null"/> if no config is loaded.</returns>
        JObject? GetConfig();
        
        /// <summary>
        /// Returns a localized (translated) string from the plugin's language files,
        /// formatted with the supplied arguments.
        /// </summary>
        /// <param name="translationKey">The key that identifies the translation entry.</param>
        /// <param name="args">Optional format arguments passed to <see cref="string.Format(string, object[])"/>.</param>
        /// <returns>The translated and formatted string, or the raw key if no translation is found.</returns>
        string Localize(string translationKey, params object[] args);
        
        /// <summary>
        /// Returns a localized (translated) string from the plugin's language files,
        /// optionally prefixed with the plugin's prefix, formatted with the supplied arguments.
        /// </summary>
        /// <param name="addPrefix">When <see langword="true"/>, the plugin's prefix is prepended to the result.</param>
        /// <param name="translationKey">The key that identifies the translation entry.</param>
        /// <param name="args">Optional format arguments passed to <see cref="string.Format(string, object[])"/>.</param>
        /// <returns>The translated and formatted string, or the raw key if no translation is found.</returns>
        string Localize(bool addPrefix, string translationKey, params object[] args);
    }
}
