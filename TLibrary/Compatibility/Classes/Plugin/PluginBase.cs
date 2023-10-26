using Rocket.API;
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

namespace Tavstal.TLibrary.Compatibility
{
    public abstract class PluginBase<PluginConfig> : RocketPlugin where PluginConfig : ConfigurationBase
    {
        /// <summary>
        /// Equals with RocketPlugin.Name
        /// It's used to prevent using RocketPlugin.name instead of RocketPlugin.Name
        /// </summary>
        public string PluginName => this.Name;
        public PluginConfig Config { get; set; }
        public virtual PluginBase<PluginConfig> Instance { get; set; }
        public virtual IDatabaseManager DatabaseManager { get; set; }
        public static HookManager HookManager { get; set; }

        private static readonly System.Version _version = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly DateTime _buildDate = new DateTime(2000, 1, 1).AddDays(_version.Build).AddSeconds(_version.Revision * 2);
        public static System.Version Version { get { return _version; } }
        public static DateTime BuildDate { get { return _buildDate; } }

        protected override void Load()
        {
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

        public virtual void OnLoad()
        {

        }

        public virtual void OnUnLoad() 
        {
            
        }

        public virtual void CheckPluginFiles()
        {
            string rocketConfigFile = Path.Combine(this.Directory, $"{PluginName}.configuration.xml");
            if (File.Exists(rocketConfigFile))
                File.Delete(rocketConfigFile);

            string rocketTranslationFile = Path.Combine(this.Directory, $"{PluginName}.en.translation.xml");
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

            if (DefaultLocalization != null)
                Localization = DefaultLocalization;

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
                                LoggerHelper.LogError("Failed to download language packs.");
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
                        Localization = localLocale;
                    else if (localLocale.Count == 0 && locale == "en")
                    {
                        PluginExtensions.SaveTranslation(DefaultLocalization, translationsDirectory, "locale.en.json");
                    }
                }
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
            LoggerHelper.LogWarning($"OLD TRANSLATION METHOD WAS USED FOR '{translationKey}'");
            return Localize(false, translationKey, placeholder);
        }

        public string Localize(bool AddPrefix, string translationKey, params object[] args)
        {
            string localization = string.Empty;
            if (!Localization.TryGetValue(translationKey, out localization))
                localization = $"<color=#FFAA00>[WARNING]</color> <color=#FFFF55>Untranslated key found in {PluginName}:</color> <color=#FF5555>{translationKey}</color>";

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
        public virtual Dictionary<string, string> DefaultLocalization { get; set; }
        public virtual Dictionary<string, string> LanguagePacks { get; set; }
    }
}
