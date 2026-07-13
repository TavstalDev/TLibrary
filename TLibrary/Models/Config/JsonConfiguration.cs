using System;
using System.IO;
using Newtonsoft.Json;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Logging;

namespace Tavstal.TLibrary.Models.Config
{
    /// <summary>
    /// Abstract class for all configurations.
    /// </summary>
    public abstract class JsonConfiguration : IConfiguration
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
        
        protected JsonConfiguration(string filename, string path)
        {
            FilePath = path;
            FileName = filename;
            // ReSharper disable once VirtualMemberCallInConstructor
            LoadDefaults();
        }
        
        protected JsonConfiguration()
        {
            FilePath = string.Empty;
            FileName = string.Empty;
        }

        /// <inheritdoc/>
        public string GetFileName() => FileName;
        
        /// <inheritdoc/>
        public string GetFilePath() => FilePath;
        
        /// <inheritdoc/>
        public ELogLevel GetLogLevel() => LogLevel;

        /// <inheritdoc/>
        public void SetLogLevel(ELogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        /// <inheritdoc/>
        public string GetLocale() => Locale;

        /// <inheritdoc/>
        public void SetLocale(string locale)
        {
            Locale = locale;
        }

        /// <inheritdoc/>
        public bool GetDownloadLocalePacks() => DownloadLocalePacks;

        /// <inheritdoc/>
        public void SetDownloadLocalePacks(bool downloadLocalePacks)
        {
             DownloadLocalePacks = downloadLocalePacks;
        }

        /// <inheritdoc/>
        public abstract void LoadDefaults();
        
        /// <inheritdoc/>
        public bool Verify()
        {
            string fullPath = Path.Combine(FilePath, FileName);
            if (!File.Exists(fullPath))
            {
                Save();
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public T? ReadConfig<T>() where T : class
        {
            string fullPath = Path.Combine(FilePath, FileName);
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
                File.Move(fullPath, Path.Combine(FilePath, FileName.Insert(FileName.IndexOf(".json", StringComparison.Ordinal), $"_save_{DateTime.Now.ToString("s").Replace("-", "").Replace(":", "")}")));
                Save();
                return null;
            }
        }

        /// <inheritdoc/>
        public void Save()
        {
            string fullPath = Path.Combine(FilePath, FileName);
            File.WriteAllText(fullPath, JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            }));
        }

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> using the parameterless constructor.
        /// </summary>
        /// <typeparam name="T">The type of configuration to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T? Create<T>() where T : class, IConfiguration => 
            Activator.CreateInstance<T>(); 

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> with the given file name and path.
        /// </summary>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <param name="path">The directory path of the configuration file.</param>
        /// <typeparam name="T">The type of configuration to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public static T? Create<T>(string fileName, string path) where T : class, IConfiguration =>
            (T)Activator.CreateInstance(typeof(T), fileName, path);
    }
}
