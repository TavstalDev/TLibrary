﻿using Rocket.API;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Managers;
using System.Reflection;
using Tavstal.TLibrary.Helpers;
using SDG.Unturned;
using UnityEngine;
using System.Data;
using Tavstal.TLibrary.Compatibility.Database;
using System.IO;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Extensions;
using Steamworks;
using UnityEngine.Networking;
using System.Collections;
using System.Globalization;
using Tavstal.TLibrary.Compatibility.Interfaces;
using System.Runtime.CompilerServices;

namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Abstract Plugin Base Class
    /// </summary>
    /// <typeparam name="PluginConfig"></typeparam>
    public abstract class PluginBase<PluginConfig> : RocketPlugin, IPlugin where PluginConfig : ConfigurationBase
    {
        /// <summary>
        /// Plugin Configuration
        /// </summary>
        public PluginConfig Config { get; set; }
        public virtual PluginBase<PluginConfig> Instance { get; set; }
        public virtual IDatabaseManager DatabaseManager { get; set; }
        public static HookManager HookManager { get; set; }

        private static TLogger _logger;
        public static TLogger Logger { get { return _logger; } }

        private static readonly System.Version _version = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly DateTime _buildDate = new DateTime(2000, 1, 1).AddDays(_version.Build).AddSeconds(_version.Revision * 2);
        public static System.Version Version { get { return _version; } }
        public static DateTime BuildDate { get { return _buildDate; } }

        protected override void Load()
        {
            _logger = TLogger.CreateInstance(this, false);
            base.Load();
            Instance = this;
            CheckPluginFiles();
            OnLoad();
        }

        protected override void Unload()
        {
            base.Unload();
            OnUnLoad();
        }

        public string GetPluginName()
        {
            return this.Name;
        }

        public TLogger GetLogger()
        {
            return _logger;
        }

        public virtual void OnLoad()
        {

        }

        public virtual void OnUnLoad() 
        {
            
        }

        public virtual void CheckPluginFiles()
        {
            string rocketConfigFile = Path.Combine(this.Directory, $"{GetPluginName()}.configuration.xml");
            if (File.Exists(rocketConfigFile))
                File.Delete(rocketConfigFile);

            string rocketTranslationFile = Path.Combine(this.Directory, $"{GetPluginName()}.en.translation.xml");
            if (File.Exists(rocketTranslationFile))
                File.Delete(rocketTranslationFile);

            string translationsDirectory = Path.Combine(this.Directory, "Translations");
            if (!System.IO.Directory.Exists(translationsDirectory))
                System.IO.Directory.CreateDirectory(translationsDirectory);

            Config = Activator.CreateInstance<PluginConfig>();
            Config.FileName = "Configuration.json";
            Config.FilePath = this.Directory;
            if (Config.CheckConfigFile())
                Config = PluginExtensions.ReadConfig<PluginConfig>(Config) ?? Config;
            else
            {
                CultureInfo ci = CultureInfo.InstalledUICulture;
                string langISO = ci.TwoLetterISOLanguageName.ToLower();
                if (langISO != "en" && LanguagePacks != null)
                {
                    if (LanguagePacks.ContainsKey(ci.TwoLetterISOLanguageName))
                    {
                        Config.Locale = langISO;
                        Config.SaveConfig();
                    }
                }
            }

            Dictionary<string, string> localLocalization = new Dictionary<string, string>();
            if (DefaultLocalization != null)
                localLocalization = DefaultLocalization;

            string defaultTranslationFile = Path.Combine(translationsDirectory, "locale.en.json");
            if (!File.Exists(defaultTranslationFile))
            {
                PluginExtensions.SaveTranslation(DefaultLocalization, translationsDirectory, "locale.en.json");
            }

            if (LanguagePacks != null)
                if (Config.DownloadLocalePacks && LanguagePacks.Count > 0)
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

            

            string locale = Config.Locale;
            if (File.Exists(Path.Combine(translationsDirectory, $"locale.{locale}.json")))
            {
                var localLocale = PluginExtensions.ReadTranslation(translationsDirectory, $"locale.{locale}.json");
                if (localLocale != null)
                {
                    if (localLocale.Count > 0)
                        localLocalization = localLocale;
                    else if (localLocale.Count == 0 && locale == "en")
                    {
                        PluginExtensions.SaveTranslation(DefaultLocalization, translationsDirectory, "locale.en.json");
                    }
                }
            }

            foreach (var l in CommonLocalization)
            {
                if (!localLocalization.ContainsKey(l.Key))
                    Localization.Add(l.Key, l.Value);
            }
            foreach (var l in localLocalization)
            {
                Localization.Add(l.Key, l.Value);
            }

        }

        public void InvokeAction(float delay, System.Action action)
        {
            StartCoroutine(InvokeRoutine(action, delay));
        }

        internal static IEnumerator InvokeRoutine(System.Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }

        [Obsolete("Use Localize instead", true)]
        protected new string Translate(string translationKey, params object[] placeholder)
        {
            Logger.LogWarning($"OLD TRANSLATION METHOD WAS USED FOR '{translationKey}'");
            return Localize(false, translationKey, placeholder);
        }

        public string Localize(bool AddPrefix, string translationKey, params object[] args)
        {
            string localization = string.Empty;
            if (!Localization.TryGetValue(translationKey, out localization))
                localization = $"<color=#FFAA00>[WARNING]</color> <color=#FFFF55>Untranslated key found in {GetPluginName()}:</color> <color=#FF5555>{translationKey}</color>";

            if (AddPrefix)
            {
                string prefixLocalization = string.Empty;
                Localization.TryGetValue("prefix", out prefixLocalization);
                return prefixLocalization + string.Format(localization, args);
            }
            else
            {
                return string.Format(localization, args);
            }
        }

        public string Localize(string translationKey, params object[] args)
        { 
           return Localize(false, translationKey, args);
        }

        public Dictionary<string, string> Localization { get; private set; } = new Dictionary<string, string>();

        private Dictionary<string, string> CommonLocalization => new Dictionary<string, string>
        {
            { "error_command_caller_not_console", "&cThis command must be executed by the console." },
            { "error_command_caller_not_player", "&cThis command must be executed by a player." },
            { "error_command_no_permission", "&cYou do not have enough permission to execute this command." },
            { "error_command_syntax", "&cWrong syntax! Usage: /{0} {1}" },
            { "error_subcommand_not_found", "&cThe '/{0}' command does not have '{1}' subcommand." }
        };
        
        public virtual Dictionary<string, string> DefaultLocalization { get; set; }
        public virtual Dictionary<string, string> LanguagePacks { get; set; }
    }
}