using System;
using System.IO;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tavstal.TLibrary.Models.Config
{
    public abstract class YamlConfiguration : IConfiguration
    {
        [YamlIgnore]
        public string FilePath { get; set; }
        [YamlIgnore]
        public string FileName { get; set; }
        
        [YamlMember(Order = 0, Description = "Log level for the plugin, default is 'INFO'.\nValues: DEBUG, INFO, WARNING, ERROR, COMMAND")]
        public ELogLevel LogLevel { get; set; }
        
        [YamlMember(Order = 1, Description = "Locale used for the plugin, default is 'en'.")] 
        public string Locale { get; set; } = "en";
        
        [YamlMember(Order = 2, Description = "Should the plugin download locale packs from the server?")]
        public bool DownloadLocalePacks { get; set; }
        
        protected YamlConfiguration(string filename, string path)
        {
            FilePath = path;
            FileName = filename;
            // ReSharper disable once VirtualMemberCallInConstructor
            LoadDefaults();
        }
        
        protected YamlConfiguration()
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
        
        public T? ReadConfig<T>() where T : class
        {
            string fullPath = Path.Combine(FilePath, FileName);
            try
            {
                string text = File.ReadAllText(fullPath);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(HyphenatedNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();
                return deserializer.Deserialize<T>(text);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Failed to read the configuration file.");
                LoggerHelper.LogError($"YAML ERROR: {ex.Message}");
                if (ex.InnerException != null) 
                    LoggerHelper.LogError($"Inner Exception: {ex.InnerException.Message}");
                File.Move(fullPath, Path.Combine(FilePath, FileName.Insert(FileName.IndexOf(".yml", StringComparison.Ordinal), $"_save_{DateTime.Now.ToString("s").Replace("-", "").Replace(":", "")}")));
                Save();
                return null;
            }
        }

        public void Save()
        {
            string fullPath = Path.Combine(FilePath, FileName);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(this);
            File.WriteAllText(fullPath, yaml);
        }
        
        public static T? Create<T>() where T : class, IConfiguration => 
            Activator.CreateInstance<T>(); 
        
        public static T? Create<T>(string fileName, string path) where T : class, IConfiguration =>
            (T)Activator.CreateInstance(typeof(T), fileName, path);
    }
}