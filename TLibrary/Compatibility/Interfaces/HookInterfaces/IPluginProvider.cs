using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Basic plugin provider interface used for hooks
    /// </summary>
    public interface IPluginProvider
    {
        /// <summary>
        /// Gets the config value from the config object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="VariableName"></param>
        /// <returns></returns>
        T GetConfigValue<T>(string VariableName);
        /// <summary>
        /// Gets the config object as a <see cref="JObject"/>
        /// </summary>
        /// <returns></returns>
        JObject GetConfig();
        /// <summary>
        /// Gets a localized string from the plugin
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        string Localize(string translationKey, params object[] args);
        /// <summary>
        /// Gets a localized string from the plugin
        /// </summary>
        /// <param name="addPrefix">Should add prefix to the translation?</param>
        /// <param name="translationKey"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        string Localize(bool addPrefix, string translationKey, params object[] args);
    }
}
