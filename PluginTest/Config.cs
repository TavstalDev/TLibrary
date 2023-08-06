using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary;
using Tavstal.TLibrary.Compatibility.Classes.Plugin;
using YamlDotNet.Serialization;

namespace PluginTest
{
    public class Config : ConfigurationBase
    {
        [YamlMember(Order = 2, Description = "Database related settings")]
        public DBSettings Database { get; set; }

        public override void LoadDefaults()
        {
            base.LoadDefaults();
            Locale = "en";
            DownloadLocalePacks = true;
            Database = new DBSettings();
            Database.DatabaseName = "unturned";
            Database.UserPassword = "Admin123";
        }
    }
}
