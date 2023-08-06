using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary;
using Tavstal.TLibrary.Compatibility.Classes.Plugin;

namespace PluginTest
{
    public class Main : PluginBase<Config>
    {
        public override Dictionary<string, string> DefaultLocalization => new Dictionary<string, string>()
        {
            { "locale_test_1", "Test Message 1" },
            { "locale_test_2", "Test Message 2" },
            { "locale_test_3", "Test Message 3" }
        };
        public override Dictionary<string, string> LanguagePacks => new Dictionary<string, string>()
        {
            { "de", "https://pastebin.com/raw/kzYWvRFd" },
            { "es", "https://pastebin.com/raw/1My2kwiB" }
        };
        public static new Main Instance;
        public new DatabaseManager DatabaseManager { get; set; }

        public override void OnLoad()
        {
            base.OnLoad();
            DatabaseManager = new DatabaseManager(Config);
            LoggerHelper.Log($"# {PluginName} has been loaded successfully.");
            Level.onPostLevelLoaded += LateInit;
        }

        public override void OnUnLoad()
        {
            base.OnUnLoad();
            Level.onPostLevelLoaded -= LateInit;
        }

        public void LateInit(int level)
        {

        }
    }
}
