using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers;
using Rocket.API;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Tavstal.TLibrary.Extensions
{
    public static class RocketPluginExtensions
    {
        public static T GetField<T>(this IRocketPluginConfiguration config, string name, BindingFlags flags = BindingFlags.Public)
        {
            try
            {
                return (T)config.GetType().GetField(name, flags).GetValue(config);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in GetField, RocketPluginExtensions:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }

        public static T GetProperty<T>(this IRocketPluginConfiguration config, string name, BindingFlags flags = BindingFlags.Public)
        {
            try
            {
                return (T)config.GetType().GetProperty(name, flags).GetValue(config);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in GetProperty, RocketPluginExtensions:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }

        public static T GetValue<T>(this IRocketPluginConfiguration config, string name, BindingFlags flags = BindingFlags.Public)
        {
            try
            {
                T value = default;
                var obj = JObject.FromObject(config);

                if (obj.TryGetValue(name, out JToken result))
                    value = result.Value<T>();

                return value;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in GetValue, RocketPluginExtensions:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }
    }
}
