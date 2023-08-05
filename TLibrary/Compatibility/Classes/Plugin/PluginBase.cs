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
using Tavstal.TLibrary.Compatibility.Classes.Database;

namespace Tavstal.TLibrary.Compatibility.Classes.Plugin
{
    public abstract class PluginBase<RocketPluginConfiguration> : RocketPlugin<IRocketPluginConfiguration>
    {
        public static DatabaseManagerBase DatabaseManager { get; set; }
        public static HookManager hookManager { get; set; }
        private static System.Version _version = Assembly.GetExecutingAssembly().GetName().Version;
        private static DateTime _buildDate = new DateTime(2000, 1, 1).AddDays(_version.Build).AddSeconds(_version.Revision * 2);
        public static System.Version Version { get { return _version; } }
        public static DateTime BuildDate { get { return _buildDate; } }

        protected override void Load()
        {
            base.Load();
            OnLoad();
            try
            {
                var connection = DatabaseManager.CreateConnection();

                if (DatabaseHelper.DoesTableExist<PlayerData>(connection))
                    DatabaseHelper.CheckTable<PlayerData>(connection);
                else
                    DatabaseHelper.CreateTable<PlayerData>(connection);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
            }
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

        [Obsolete("Use CTranslate instead", true)]
        protected new string Translate(string translationKey, params object[] placeholder)
        {
            LoggerHelper.LogWarning($"OLD TRANSLATION METHOD WAS USED FOR '{translationKey}'");
            return Translations.Instance.Translate(translationKey, placeholder);
        }

        public string CTranslate(bool AddPrefix, string translationKey, params object[] placeholder)
        {
            if (AddPrefix)
                return Translations.Instance.Translate("prefix") + Translations.Instance.Translate(translationKey, placeholder);
            else
                return Translations.Instance.Translate(translationKey, placeholder);
        }
    }
}
