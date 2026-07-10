using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Extensions
{
    /// <summary>
    /// Provides extension methods for plugins.
    /// </summary>
    public static class PluginExtensions
    {
        /// <summary>
        /// Checks if the config file exists. If not, creates and saves it.
        /// </summary>
        /// <param name="configuration">The config to check.</param>
        /// <returns>True if the config file exists, false if a new one was created.</returns>
        public static bool CheckConfigFile(this ConfigurationBase configuration)
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            if (!File.Exists(fullPath))
            {
                configuration.SaveConfig();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads and returns a config object from a file.
        /// </summary>
        /// <typeparam name="T">The type of the config to read.</typeparam>
        /// <param name="configuration">The config to read from.</param>
        /// <returns>The config object, or null if it failed.</returns>
        public static T? ReadConfig<T>(this ConfigurationBase configuration) where T : ConfigurationBase
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            try
            {
                string text = File.ReadAllText(fullPath);
                return JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                });
            }
            catch
            {
                LoggerHelper.LogError("Failed to read the configuration file, it might be outdated.\nSaving current one and generating a new file...");
                File.Move(fullPath, Path.Combine(configuration.FilePath, configuration.FileName.Insert(configuration.FileName.IndexOf(".json", StringComparison.Ordinal), $"_save_{DateTime.Now.ToString("s").Replace("-", "").Replace(":", "")}")));
                configuration.SaveConfig();
                return null;
            }
        }

        /// <summary>
        /// Saves the config object to its file.
        /// </summary>
        /// <param name="configuration">The config to save.</param>
        public static void SaveConfig(this ConfigurationBase configuration)
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            File.WriteAllText(fullPath, JsonConvert.SerializeObject(configuration, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            }));
        }

        /// <summary>
        /// Reads translations from a file.
        /// </summary>
        /// <param name="filePath">The folder path of the translation file.</param>
        /// <param name="fileName">The name of the translation file.</param>
        /// <returns>A dictionary with translation keys and values.</returns>
        public static Dictionary<string, string>? ReadTranslation(string filePath, string fileName)
        {
            string fullPath = Path.Combine(filePath, fileName);
            string text = File.ReadAllText(fullPath);

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(text, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            });
        }

        /// <summary>
        /// Saves translations to a file.
        /// </summary>
        /// <param name="locale">The dictionary with translation keys and values to save.</param>
        /// <param name="filePath">The folder path to save the translation file in.</param>
        /// <param name="fileName">The name of the translation file.</param>
        public static void SaveTranslation(Dictionary<string, string> locale, string filePath, string fileName)
        {
            string fullPath = Path.Combine(filePath, fileName);
            var yaml = JsonConvert.SerializeObject(locale, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            });
            File.WriteAllText(fullPath, yaml);
        }

        /// <summary>
        /// Gets the value of a config property by its name.
        /// </summary>
        /// <typeparam name="T">The type of the value to get.</typeparam>
        /// <param name="config">The config to get the value from.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property value, or null if not found.</returns>
        public static T GetValue<T>(this IConfigurationBase config, string name)
        {
            try
            {
                T value = default(T);
                var token = JObject.FromObject(config).SelectToken(name.Replace(":", "."));
                if (token != null)
                    value = token.Value<T>();
                return value!;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError($"Error in GetValue({name}), PluginExtensions:");
                LoggerHelper.LogError(ex);
                return default!;
            }
        }
    }
}
