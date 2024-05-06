using Rocket.API;
using System;
using System.Reflection;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Extensions
{
    /// <summary>
    /// Extensions used for RocketPlugins.
    /// </summary>
    public static class RocketPluginExtensions
    {
        /// <summary>
        /// Get field from RocketPluginConfiguration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get property from RocketPluginConfiguration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
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
    }
}
