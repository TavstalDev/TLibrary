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
        public ELogLevel LogLevel { get; set; }
        [JsonProperty(Order = 1)] 
        public string Locale { get; set; } = "en";
        [JsonProperty(Order = 2)]
        public bool DownloadLocalePacks { get; set; }

        /// <param name="filename">Example: myfile.txt</param>
        /// <param name="path">Example: D:\MyDirectory</param>
        protected ConfigurationBase(string filename, string path)
        {
            FilePath = path;
            FileName = filename;
            // ReSharper disable once VirtualMemberCallInConstructor
            LoadDefaults();
        }

        /// <summary>
        /// Creates a configuration base with default empty values.
        /// </summary>
        protected ConfigurationBase()
        {
            FilePath = string.Empty;
            FileName = string.Empty;
        }

        /// <summary>
        /// Called when the configuration is created to set default values.
        /// Override this to define custom defaults for your configuration.
        /// </summary>
        public virtual void LoadDefaults() { }

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> using the parameterless constructor.
        /// </summary>
        /// <typeparam name="T">The type of configuration to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T Create<T>() where T : ConfigurationBase => 
            Activator.CreateInstance<T>(); 

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> with the given file name and path.
        /// </summary>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <param name="path">The directory path of the configuration file.</param>
        /// <typeparam name="T">The type of configuration to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T Create<T>(string fileName, string path) where T : ConfigurationBase =>
            (T)Activator.CreateInstance(typeof(T), fileName, path);
    }
}
