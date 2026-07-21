using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Rocket.Core.Plugins;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Extensions.General;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Managers;
using Tavstal.TLibrary.Models.Config;
using Tavstal.TLibrary.Models.Logging;
using UnityEngine;
using UnityEngine.Networking;
using YamlDotNet.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable StaticMemberInGenericType
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Tavstal.TLibrary.Models.Plugin
{
    /// <summary>
    /// Abstract Plugin Base Class
    /// </summary>
    public abstract class PluginBase<PluginConfig> : RocketPlugin, IPlugin where PluginConfig : class, IConfiguration
    {
        /// <summary>
        /// The root directory path for the plugin.
        /// </summary>
        private string _rootDirectory = string.Empty;
        /// <summary>
        /// Gets the root directory path for the plugin.
        /// </summary>
        public string RootDirectory => _rootDirectory;

        /// <summary>
        /// The directory path specific to the plugin.
        /// </summary>
        private string _pluginDirectory = string.Empty;
        /// <summary>
        /// Gets the directory path specific to the plugin.
        /// </summary>
        public string PluginDirectory => _pluginDirectory;

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        private string _pluginName = string.Empty;
        /// <summary>
        /// Gets or sets the name of the plugin. The name can only be set if it is currently null or empty.
        /// </summary>
        public string PluginName 
        { 
            get => _pluginName; 
            set 
            { 
                if (_pluginName.IsNullOrEmpty()) 
                    _pluginName = value; 
            } 
        }


        /// <summary>
        /// Plugin Configuration
        /// </summary>
        public PluginConfig Config { get; private set; } = null!;

        /// <summary>
        /// Hook Manager used to work with plugin hooks
        /// </summary>
        public static HookManager HookManager { get; set; } = null!;

        /// <summary>
        /// Rich logger used to replace Rocket's logger
        /// </summary>
        private static TLogger _logger { get; set; } = null!;
        /// <summary>
        /// Rich logger used to replace Rocket's logger
        /// </summary>
        public static TLogger Logger => _logger;
        
        private static Version? _libraryVersion;
        private static Version? _version;
        private static DateTime _buildDate;
        
        public static Version? LibraryVersion => _libraryVersion;
        public static Version? Version => _version;
        public static DateTime BuildDate => _buildDate;

        public sealed override void LoadPlugin()
        {
            if (!Name.IsNullOrEmpty())
                _pluginName = Name;
            _rootDirectory = System.IO.Directory.GetCurrentDirectory();
            _pluginDirectory = Path.Combine(_rootDirectory, "Plugins", GetPluginName());
            _logger = new TLogger(this, "", ELogLevel.INFO);
            
            // Get Library Version
            try
            {
                _libraryVersion  = VersionHelper.GetVersion(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get library version:", ex);
            }
            
            // Get Plugin Build Version
            try
            {
                _version = VersionHelper.GetVersion(Assembly);
                var buildDateAttribute = Assembly
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .FirstOrDefault(x => x.Key == "BuildDate");
                if (buildDateAttribute != null)
                    _buildDate = DateTime.Parse(buildDateAttribute.Value);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get plugin build version:", ex);
            }
            
            OnPreLoad();
            base.LoadPlugin();
        }

        public virtual void OnPreLoad() {}
        
        /// <summary>
        /// Used when the plugin loads
        /// </summary>
        protected override void Load()
        {
            try
            {
                CheckPluginFiles();
                OnLoad();
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while loading {Name}", ex);
            }
        }

        /// <summary>
        /// Used when the plugin unloads
        /// </summary>
        protected override void Unload()
        {
            OnUnLoad();
            Localization = new Dictionary<string, string>(); // Clearing because of /rocket reload caused error
        }

        /// <inheritdoc/>
        public IConfiguration GetConfiguration() => Config;

        /// <inheritdoc/>
        public TLogger GetLogger() => _logger;

        /// <inheritdoc/>
        public ELogLevel GetLogLevel() => Config.GetGeneral().LogLevel;

        /// <inheritdoc/>
        public string GetPluginName() => _pluginName;

        /// <summary>
        /// Called after the plugins Load() function was called
        /// </summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Called after the plugins Unload() function was called
        /// </summary>
        public virtual void OnUnLoad() { }

        /// <summary>
        /// Used to check the plugin files.
        /// <br/>Deleting rocket generated files
        /// <br/>And generating and loading the JSON configurations and translations
        /// </summary>
        public virtual void CheckPluginFiles()
        {
            // Delete Rocket Config
            string rocketConfigFile = Path.Combine(_pluginDirectory, $"{PluginName}.configuration.xml");
            if (File.Exists(rocketConfigFile))
                File.Delete(rocketConfigFile);

            // Delete Rocket Translation
            string rocketTranslationFile = Path.Combine(_pluginDirectory, $"{PluginName}.en.translation.xml");

            if (File.Exists(rocketTranslationFile))
                File.Delete(rocketTranslationFile);

            // Create Translations Directory
            string translationsDirectory = Path.Combine(_pluginDirectory, "Translations");
            if (!System.IO.Directory.Exists(translationsDirectory))
                System.IO.Directory.CreateDirectory(translationsDirectory);

            // Handle Configuration
            if (typeof(JsonConfiguration).IsAssignableFrom(typeof(PluginConfig)))
            {
                var localConfig = JsonConfiguration.Create<PluginConfig>("Configuration.json", _pluginDirectory);
                if (localConfig == null)
                {
                    Logger.Error($"Failed to create json configuration base.");
                    UnloadPlugin();
                    return;
                }
                localConfig.LoadDefaults();

                if (localConfig.Verify())
                {
                    var localPluginConfig = localConfig.ReadConfig<PluginConfig>();
                    if (localPluginConfig == null)
                    {
                        Logger.Error($"Failed to read configuration, it might be outdated. Please check the configuration file and try again.");
                        UnloadPlugin();
                        return;
                    }
                    Config = localPluginConfig;
                }
                else
                {
                    Config = localConfig;
                    Config.Save();
                }
            }
            else if (typeof(YamlConfiguration).IsAssignableFrom(typeof(PluginConfig)))
            {
                var localConfig = YamlConfiguration.Create<PluginConfig>("configuration.yml", _pluginDirectory);
                if (localConfig == null)
                {
                    Logger.Error($"Failed to create yaml configuration base.");
                    UnloadPlugin();
                    return;
                }
                localConfig.LoadDefaults();

                if (localConfig.Verify())
                {
                    var localPluginConfig = localConfig.ReadConfig<PluginConfig>();
                    if (localPluginConfig == null)
                    {
                        Logger.Error($"Failed to read configuration, it might be outdated. Please check the configuration file and try again.");
                        UnloadPlugin();
                        return;
                    }
                    Config = localPluginConfig;
                }
                else
                {
                    Config = localConfig;
                    Config.Save();
                }
            }
            else
            {
                Logger.Error($"The configuration type '{typeof(PluginConfig).Name}' is not supported. Please use either 'JsonConfiguration' or 'YamlConfiguration'.");
                UnloadPlugin();
                return;
            }
            
            // Log missing fields
            foreach (var field in Config.GetType().GetProperties())
            {
                if (field.GetCustomAttribute<JsonIgnoreAttribute>() != null || field.GetCustomAttribute<YamlIgnoreAttribute>() != null)
                    continue;
                
                if (field.GetValue(Config) != null)
                    continue;
                
                Logger.Warning($"The config field '{field.Name}' is missing in '{PluginName}' configuration.");
            }
            
            foreach (var field in Config.GetType().GetFields())
            {
                if (field.GetCustomAttribute<JsonIgnoreAttribute>() != null || field.GetCustomAttribute<YamlIgnoreAttribute>() != null)
                    continue;
                
                if (field.GetValue(Config) != null)
                    continue;
                
                Logger.Warning($"The config field '{field.Name}' is missing in '{PluginName}' configuration.");
            }
            
            TLogger.SetLogLevel(GetPluginName(), Config.GetGeneral().LogLevel);

            Dictionary<string, string> localLocalization = CommonLocalization;

            foreach (var l in DefaultLocalization)
                localLocalization[l.Key] = l.Value;

            string defaultTranslationFile = Path.Combine(translationsDirectory, "locale.en.json");
            if (!File.Exists(defaultTranslationFile))
            {
                PluginExtensions.SaveTranslation(localLocalization, translationsDirectory, "locale.en.json");
            }

            if (Config.GetGeneral().DownloadLocalePacks && LanguagePacks.Count > 0)
                foreach (var pack in LanguagePacks)
                {
                    string path = Path.Combine(translationsDirectory, $"locale.{pack.Key}.json");
                    if (File.Exists(path))
                        continue;

                    UnityWebRequest www = UnityWebRequest.Get(pack.Value);
                    www.SendWebRequest();
                    InvokeAction(3, () =>
                    {
                        if (www.result == UnityWebRequest.Result.ConnectionError
                            || www.result == UnityWebRequest.Result.DataProcessingError
                            || www.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Logger.Error("Failed to download language packs.");
                        }
                        else
                            File.WriteAllText(path, www.downloadHandler.text);
                    });

                }
                
            
            string locale = Config.GetGeneral().Locale;
            if (File.Exists(Path.Combine(translationsDirectory, $"locale.{locale}.json")))
            {
                var localLocale = PluginExtensions.ReadTranslation(translationsDirectory, $"locale.{locale}.json");
                if (localLocale != null)
                {
                    if (localLocale.Count > 0)
                    {
                        if (DefaultLocalization.Count + CommonLocalization.Count - localLocale.Count != 0)
                        {
                            foreach (var l in localLocale)
                            {
                                localLocalization[l.Key] = l.Value;
                            }
                            PluginExtensions.SaveTranslation(localLocalization, translationsDirectory, $"locale.{locale}.json");
                        }
                        else
                            localLocalization = localLocale;

                    }
                    else if (localLocale.Count == 0 && locale == "en")
                    {
                        PluginExtensions.SaveTranslation(DefaultLocalization, translationsDirectory, "locale.en.json");
                    }
                }
            }

            Localization = localLocalization;
        }

        /// <summary>
        /// Used to invoke async actions
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public void InvokeAction(float delay, Action action)
        {
            StartCoroutine(InvokeRoutine(action, delay));
        }

        /// <summary>
        /// Used to invoke async actions
        /// </summary>
        /// <param name="f"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private static IEnumerator InvokeRoutine(Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }

        /// <summary>
        /// Rocket's built in translation method, it shouldn't be used.
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        [Obsolete("Use Localize instead", true)]
        protected new string Translate(string translationKey, params object[] placeholder)
        {
            Logger.Warning($"OLD TRANSLATION METHOD WAS USED FOR '{translationKey}'");
            throw new Exception("The 'Translate' method was used instead of 'Localize'.");
        }

        /// <summary>
        /// Custom method used to translate stuff with multi-language support
        /// </summary>
        /// <param name="AddPrefix"></param>
        /// <param name="translationKey"></param>
        /// <param name="args"></param>
        /// <returns>A <see cref="string"/> containing the translated text on success, or the translationKey on failure.</returns>
        public string Localize(bool AddPrefix, string translationKey, params object[] args)
        {
            try
            {
                if (!Localization.TryGetValue(translationKey, out string localization))
                    localization =
                        $"<color=#FFAA00>[WARNING]</color> <color=#FFFF55>Untranslated key found in {PluginName}:</color> <color=#FF5555>{translationKey}</color>";

                if (AddPrefix)
                {
                    if (Localization.TryGetValue("prefix", out string prefixLocalization))
                        return prefixLocalization + string.Format(localization, args);
                }

                return string.Format(localization, args);
            }
            catch (Exception ex)
            {
               Logger.Error($"Failed to localize '{translationKey}' key:", ex);
               return string.Empty;
            }
        }

        /// <summary>
        /// Custom method used to translate stuff with multi-language support
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="args"></param>
        /// <returns>A <see cref="string"/> containing the translated text on success, or the translationKey on failure.</returns>
        public string Localize(string translationKey, params object[] args)
        { 
           return Localize(false, translationKey, args);
        }

        /// <summary>
        /// The final localization dictionary, this will contain the loaded translations.
        /// </summary>
        public Dictionary<string, string> Localization { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Translation keys used in the library, can be overwritten by declararing the keys in <see cref="DefaultLocalization"/>
        /// </summary>
        private Dictionary<string, string> CommonLocalization => new Dictionary<string, string>
        {
            { "commands_common_error_player_caller", "&cThis command must be executed by the console." },
            { "commands_common_error_console_caller", "&cThis command must be executed by a player." },
            { "commands_common_error_permission", "&cYou do not have enough permission to execute this command." },
            { "commands_common_error_syntax", "&cWrong syntax! Usage: /{0} {1}" },
            { "commands_common_error_invalid_subcommand", "&cThe '/{0}' command does not have '{1}' subcommand." },
            { "commands_common_error_parse_int", "&cYou must provide a valid integer as argument for '{0}'." },
            { "commands_common_error_parse_byte", "&cYou must provide a valid byte as argument for '{0}'. (Value is between 0-255)" },
            { "commands_common_error_parse_decimal", "&cYou must provide a valid decimal as argument for '{0}'." },
            { "commands_common_error_parse_string", "&cYou must provide a valid string as argument for '{0}'." },
            { "commands_common_error_parse_number", "&cYou must provide a valid number as argument for '{0}'." },
            { "commands_common_usage", "&aUsage: /{0} {1}" },
        };

        /// <summary>
        /// Default translations in the native language.
        /// </summary>
        public virtual Dictionary<string, string> DefaultLocalization { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Officialy supported language packs.
        /// </summary>
        public virtual Dictionary<string, string> LanguagePacks { get; set; } = new Dictionary<string, string>();
    }
}
