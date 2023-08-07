using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Classes.Plugin;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Newtonsoft.Json.Linq;
using Rocket.API;
using System.Reflection;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Compatibility.Interfaces;

namespace Tavstal.TLibrary.Extensions
{
    public static class PluginExtensions
    {

        /// <summary>
        /// Checks whether the provided <paramref name="configuration"/> has a valid config file, and if not, creates and saves it.
        /// </summary>
        /// <param name="configuration">The configuration instance to be checked and potentially saved.</param>
        /// <returns>True if the configuration file is valid; otherwise, false.</returns>
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
        /// Reads and returns a configuration object of type <typeparamref name="T"/> from the provided <paramref name="configuration"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object to read.</typeparam>
        /// <param name="configuration">The configuration instance containing the configuration data.</param>
        /// <returns>The configuration object of type <typeparamref name="T"/>.</returns>
        public static T ReadConfig<T>(this ConfigurationBase configuration) where T : ConfigurationBase
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string text = File.ReadAllText(fullPath);

            return deserializer.Deserialize<T>(text);
        }

        /// <summary>
        /// Reads and deserializes a configuration object of type <typeparamref name="T"/> from the provided <paramref name="configuration"/> instance.
        /// The deserialized configuration object is assigned back to the <paramref name="configuration"/> reference.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object to read.</typeparam>
        /// <param name="configuration">The reference to the configuration instance to be updated.</param>
        public static void ReadConfig<T>(ref ConfigurationBase configuration) where T : ConfigurationBase
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            if (!File.Exists(fullPath))
            {
                configuration.LoadDefaults();
                configuration.SaveConfig();
                return;
            }


            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string text = File.ReadAllText(fullPath);

            configuration = deserializer.Deserialize<T>(text);
        }

        /// <summary>
        /// Saves the provided <paramref name="configuration"/> object by serializing it and writing to a file.
        /// </summary>
        /// <param name="configuration">The configuration object to be saved.</param>
        public static void SaveConfig(this ConfigurationBase configuration)
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(configuration);
            File.WriteAllText(fullPath, yaml);
        }

        /// <summary>
        /// Reads translations from the specified file located at the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path where the translation file is located.</param>
        /// <param name="fileName">The name of the translation file.</param>
        /// <returns>A dictionary containing translation key-value pairs.</returns>
        public static Dictionary<string, string> ReadTranslation(string filePath, string fileName)
        {
            string fullPath = Path.Combine(filePath, fileName);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string text = File.ReadAllText(fullPath);

            return deserializer.Deserialize<Dictionary<string, string>>(text);
        }

        /// <summary>
        /// Saves the provided <paramref name="locale"/> translations to the specified file located at the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="locale">The dictionary containing translation key-value pairs to be saved.</param>
        /// <param name="filePath">The path where the translation file should be saved.</param>
        /// <param name="fileName">The name of the translation file.</param>
        public static void SaveTranslation(Dictionary<string, string> locale, string filePath, string fileName)
        {
            string fullPath = Path.Combine(filePath, fileName);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(locale);
            File.WriteAllText(fullPath, yaml);
        }

        /// <summary>
        /// Retrieves the value of the configuration property with the specified <paramref name="name"/> from the given <paramref name="config"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="config">The configuration object containing the property.</param>
        /// <param name="name">The name of the property to retrieve.</param>
        /// <returns>The value of the specified property.</returns>
        public static T GetValue<T>(this IConfigurationBase config, string name)
        {
            try
            {
                T value = default;

                var token = JObject.FromObject(config).SelectToken(name.Replace(":", "."));
                if (token != null)
                    value = token.Value<T>();

                return value;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in GetValue, PluginExtensions:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }
    }
}
