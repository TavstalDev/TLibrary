using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Interfaces;
using YamlDotNet.Serialization;

namespace Tavstal.TLibrary.Compatibility
{
    public abstract class DatabaseSettingsBase : IDatabaseSettings
    {
        [YamlMember(Order = 0)]
        public string Host { get; set; } = "127.0.0.1";
        [YamlMember(Order = 1)]
        public int Port { get; set; } = 3306;
        [YamlMember(Order = 2)]
        public string DatabaseName { get; set; } = "unturned";
        [YamlMember(Order = 3)]
        public string UserName { get; set; } = "root";
        [YamlMember(Order = 4)]
        public string UserPassword { get; set; } = "ascent";
        [YamlMember(Order = 5)]
        public int TimeOut { get; set; } = 120;
    }
}
