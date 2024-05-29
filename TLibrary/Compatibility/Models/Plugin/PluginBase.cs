using Rocket.Core.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Abstract Plugin Base Class
    /// </summary>
    /// <typeparam name="PluginConfig"></typeparam>
    public abstract class PluginBase<PluginConfig> : RocketPlugin, IPlugin where PluginConfig : ConfigurationBase
    {
        private PluginConfig _config;
        /// <summary>
        /// Plugin Configuration
        /// </summary>
        public PluginConfig Config { get { return _config; } }
        /// <summary>
        /// Instance of the Plugin
        /// </summary>
        public virtual PluginBase<PluginConfig> Instance { get; set; }
        /// <summary>
        /// Optional Database Manager
        /// </summary>
        public virtual IDatabaseManager DatabaseManager { get; set; }
        /// <summary>
        /// Hook Manager used to work with plugin hooks
        /// </summary>
        public static HookManager HookManager { get; set; }

        /// <summary>
        /// Rich logger used to replace Rocket's logger
        /// </summary>
        private static TLogger _logger;
        /// <summary>
        /// Rich logger used to replace Rocket's logger
        /// </summary>
        public static TLogger Logger { get { return _logger; } }

        private static readonly System.Version _version = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly DateTime _buildDate = new DateTime(2000, 1, 1).AddDays(_version.Build).AddSeconds(_version.Revision * 2);
        private static FileVersionInfo _versionInfo;
        public static System.Version Version { get { return _version; } }
        public static DateTime BuildDate { get { return _buildDate; } }
        public static FileVersionInfo VersionInfo { get { return _versionInfo; } }

        /// <summary>
        /// Used when the plugin loads
        /// </summary>
        protected override void Load()
        {
            _logger = TLogger.CreateInstance(this, false);
            _versionInfo = FileVersionInfo.GetVersionInfo(this.Assembly.Location);
            base.Load();
            Instance = this;
            CheckPluginFiles();
            OnLoad();
        }

        /// <summary>
        /// Used when the plugin unloads
        /// </summary>
        protected override void Unload()
        {
            base.Unload();
            OnUnLoad();
            Localization = new Dictionary<string, string>(); // Clearing because of /rocket reload caused error
        }

        /// <summary>
        /// Returns the name of the plugin
        /// </summary>
        /// <returns>A <see cref="string"></see> containing the name of the plugin.</returns>
        public string GetPluginName()
        {
            return this.Name;
        }

        /// <summary>
        /// Returns the logger
        /// </summary>
        /// <returns>Object of a <see cref="TLogger"/></returns>
        public TLogger GetLogger()
        {
            return _logger;
        }

        /// <summary>
        /// Called after the plugins Load() function was called
        /// </summary>
        public virtual void OnLoad()
        {

        }

        /// <summary>
        /// Called after the plugins Unload() function was called
        /// </summary>
        public virtual void OnUnLoad() 
        {
            
        }

        /// <summary>
        /// Used to check the plugin files.
        /// <br/>Deleting rocket generated files
        /// <br/>And generating and loading the json configurations and translations
        /// </summary>
        public virtual void CheckPluginFiles()
        {
            // Delete Rocket Config
            string rocketConfigFile = Path.Combine(this.Directory, $"{GetPluginName()}.configuration.xml");
            if (File.Exists(rocketConfigFile))
                File.Delete(rocketConfigFile);

            // Delete Rocket Translation
            string rocketTranslationFile = Path.Combine(this.Directory, $"{GetPluginName()}.en.translation.xml");
            if (File.Exists(rocketTranslationFile))
                File.Delete(rocketTranslationFile);

            // Create Translations Directory
            string translationsDirectory = Path.Combine(this.Directory, "Translations");
            if (!System.IO.Directory.Exists(translationsDirectory))
                System.IO.Directory.CreateDirectory(translationsDirectory);

            // Handle Configuration
            _config = ConfigurationBase.Create<PluginConfig>("Configuration.json", this.Directory);

            if (_config.CheckConfigFile())
                _config = PluginExtensions.ReadConfig<PluginConfig>(_config);
            else
            {
                CultureInfo ci = CultureInfo.InstalledUICulture;
                string langISO = ci.TwoLetterISOLanguageName.ToLower();
                if (langISO != "en" && LanguagePacks != null)
                {
                    if (LanguagePacks.ContainsKey(ci.TwoLetterISOLanguageName))
                    {
                        _config.Locale = langISO;
                        _config.SaveConfig();
                    }
                }
            }

            _logger.SetDebugMode(_config.DebugMode);

            Dictionary<string, string> localLocalization = CommonLocalization ?? new Dictionary<string, string>();
            if (DefaultLocalization != null)
                foreach (var l in DefaultLocalization)
                {
                    if (localLocalization.ContainsKey(l.Key))
                        localLocalization[l.Key] = l.Value;   
                    else
                        localLocalization.Add(l.Key, l.Value);
                }

            string defaultTranslationFile = Path.Combine(translationsDirectory, "locale.en.json");
            if (!File.Exists(defaultTranslationFile))
            {
                PluginExtensions.SaveTranslation(localLocalization, translationsDirectory, "locale.en.json");
            }

            if (LanguagePacks != null)
                if (_config.DownloadLocalePacks && LanguagePacks.Count > 0)
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
                                Logger.LogError("Failed to download language packs.");
                            }
                            else
                                File.WriteAllText(path, www.downloadHandler.text);
                        });

                    }

            

            string locale = _config.Locale;
            if (File.Exists(Path.Combine(translationsDirectory, $"locale.{locale}.json")))
            {
                var localLocale = PluginExtensions.ReadTranslation(translationsDirectory, $"locale.{locale}.json");
                if (localLocale != null)
                {
                    if (localLocale.Count > 0)
                    {
                        if ((DefaultLocalization.Count + CommonLocalization.Count) - localLocale.Count != 0)
                        {
                            foreach (var l in localLocale)
                            {
                                if (localLocalization.ContainsKey(l.Key))
                                    localLocalization[l.Key] = l.Value;
                                else
                                    localLocalization.Add(l.Key, l.Value);
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
        public void InvokeAction(float delay, System.Action action)
        {
            StartCoroutine(InvokeRoutine(action, delay));
        }

        /// <summary>
        /// Used to invoke async actions
        /// </summary>
        /// <param name="f"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private static IEnumerator InvokeRoutine(System.Action f, float delay)
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
            Logger.LogWarning($"OLD TRANSLATION METHOD WAS USED FOR '{translationKey}'");
            throw new Exception("The 'Translate' method was used instead of 'Localize'.");
        }

        /// <summary>
        /// Custom method used to translate stuff with multi language support
        /// </summary>
        /// <param name="AddPrefix"></param>
        /// <param name="translationKey"></param>
        /// <param name="args"></param>
        /// <returns>A <see cref="string"/> containing the translated text on success, or the translationKey on failure.</returns>
        public string Localize(bool AddPrefix, string translationKey, params object[] args)
        {
            string localization = string.Empty;
            if (!Localization.TryGetValue(translationKey, out localization))
                localization = $"<color=#FFAA00>[WARNING]</color> <color=#FFFF55>Untranslated key found in {GetPluginName()}:</color> <color=#FF5555>{translationKey}</color>";

            if (AddPrefix)
            {
                string prefixLocalization = string.Empty;
                if (Localization.TryGetValue("prefix", out prefixLocalization))
                    return prefixLocalization + string.Format(localization, args);
                else 
                    return string.Format(localization, args);
            }
            else
            {
                return string.Format(localization, args);
            }
        }

        /// <summary>
        /// Custom method used to translate stuff with multi language support
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
            { "error_command_caller_not_console", "&cThis command must be executed by the console." },
            { "error_command_caller_not_player", "&cThis command must be executed by a player." },
            { "error_command_no_permission", "&cYou do not have enough permission to execute this command." },
            { "error_command_syntax", "&cWrong syntax! Usage: /{0} {1}" },
            { "error_subcommand_not_found", "&cThe '/{0}' command does not have '{1}' subcommand." },
            { "success_command_help", "&aUsage: /{0} {1}" },
        };
        
        /// <summary>
        /// Default translations in the native language.
        /// </summary>
        public virtual Dictionary<string, string> DefaultLocalization { get; set; }
        /// <summary>
        /// Officialy supported language packs.
        /// </summary>
        public virtual Dictionary<string, string> LanguagePacks { get; set; }
    }
}
