using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Classes.Plugin;
using YamlDotNet.Serialization;

namespace PluginTest
{
    public class DBSettings : DatabaseSettingsBase
    {
        [YamlMember(Order = 6)]
        public string TableTest = "test_table";
    }
}
