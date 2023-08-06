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

        public static T ReadConfig<T>(this ConfigurationBase configuration) where T : ConfigurationBase
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string text = File.ReadAllText(fullPath);

            return deserializer.Deserialize<T>(text);
        }

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

        public static void SaveConfig(this ConfigurationBase configuration)
        {
            string fullPath = Path.Combine(configuration.FilePath, configuration.FileName);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(configuration);
            File.WriteAllText(fullPath, yaml);
        }

        public static Dictionary<string, string> ReadTranslation(string filePath, string fileName)
        {
            string fullPath = Path.Combine(filePath, fileName);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string text = File.ReadAllText(fullPath);

            return deserializer.Deserialize<Dictionary<string, string>>(text);
        }

        public static void SaveTranslation(Dictionary<string, string> locale, string filePath, string fileName)
        {
            string fullPath = Path.Combine(filePath, fileName);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(locale);
            File.WriteAllText(fullPath, yaml);
        }

        public static T GetField<T>(this IConfigurationBase config, string name, BindingFlags flags = BindingFlags.Public)
        {
            try
            {
                return (T)config.GetType().GetField(name, flags).GetValue(config);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in GetField, PluginExtensions:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }

        public static T GetProperty<T>(this IConfigurationBase config, string name, BindingFlags flags = BindingFlags.Public)
        {
            try
            {
                return (T)config.GetType().GetProperty(name, flags).GetValue(config);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in GetProperty, PluginExtensions:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }

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
