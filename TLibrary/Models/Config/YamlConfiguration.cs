using System;
using System.IO;
using Tavstal.TLibrary.Helpers.General;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tavstal.TLibrary.Models.Config
{
    /// <summary>
    /// Represents an abstract base class for YAML-based configuration files, providing serialization,
    /// deserialization, and file management capabilities.
    /// </summary>
    public abstract class YamlConfiguration : IConfiguration
    {
        [YamlIgnore]
        public string FilePath { get; set; }
        [YamlIgnore]
        public string FileName { get; set; }
        
        [YamlMember(Order = 0)]
        public GeneralConfig General { get; set; } = new GeneralConfig();
        
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
        public GeneralConfig GetGeneral() => General;

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

        /// <inheritdoc/>
        public void Save()
        {
            string fullPath = Path.Combine(FilePath, FileName);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(this);
            File.WriteAllText(fullPath, yaml);
        }
        
        
        /// <summary>
        /// Creates a new instance of the specified configuration type using its default constructor.
        /// </summary>
        /// <typeparam name="T">The configuration type to create, which must implement <see cref="IConfiguration"/>.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>, or <see langword="null"/> if creation fails.</returns>
        public static T? Create<T>() where T : class, IConfiguration => 
            Activator.CreateInstance<T>(); 
        
        /// <summary>
        /// Creates a new instance of the specified configuration type with the given file name and path.
        /// </summary>
        /// <typeparam name="T">The configuration type to create, which must implement <see cref="IConfiguration"/>.</typeparam>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <param name="path">The directory path where the configuration file is located.</param>
        /// <returns>A new instance of <typeparamref name="T"/>, or <see langword="null"/> if creation fails.</returns>
        public static T? Create<T>(string fileName, string path) where T : class, IConfiguration =>
            (T)Activator.CreateInstance(typeof(T), fileName, path);
    }
}