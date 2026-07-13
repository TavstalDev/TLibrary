using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Config;

namespace Tavstal.TLibrary.Extensions
{
    /// <summary>
    /// Provides extension methods for plugins.
    /// </summary>
    public static class PluginExtensions
    {
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
        public static T GetValue<T>(this IConfiguration config, string name)
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
