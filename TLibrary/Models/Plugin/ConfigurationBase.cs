using System;
using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Plugin
{
    /// <summary>
    /// Abstract class for all configurations.
    /// </summary>
    public abstract class ConfigurationBase : IConfigurationBase
    {
        [JsonIgnore]
        public string FilePath { get; set; }
        [JsonIgnore]
        public string FileName { get; set; }
        [JsonProperty(Order = 0)]
        public bool DebugMode { get; set; }
        [JsonProperty(Order = 1)]
        public string Locale { get; set; }
        [JsonProperty(Order = 2)]
        public bool DownloadLocalePacks { get; set; }

        /// <param name="filename">Example: myfile.txt</param>
        /// <param name="path">Example: D:\MyDirectory</param>
        public ConfigurationBase(string filename, string path)
        {
            FilePath = path;
            FileName = filename;
            LoadDefaults();
        }

        public ConfigurationBase()
        {

        }

        public void LoadDefaults()
        {

        }

        public static T Create<T>() where T : ConfigurationBase
        {
            return Activator.CreateInstance<T>(); 
        }

        public static T Create<T>(string fileName, string path) where T : ConfigurationBase
        {
            return (T)Activator.CreateInstance(typeof(T), fileName, path);
        }
    }
}
