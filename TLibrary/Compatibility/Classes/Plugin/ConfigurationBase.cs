using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using System.Diagnostics.SymbolStore;
using System.CodeDom;

namespace Tavstal.TLibrary.Compatibility
{
    public class ConfigurationBase : IConfigurationBase
    {
        [YamlIgnore]
        public string FilePath { get; set; }
        [YamlIgnore]
        public string FileName { get; set; }
        [YamlMember(Order = 0, Description = "Set the language of the plugin. (en, de, es etc.) WARNING: It does not translate 100% of the UI.")]
        public string Locale { get; set; }
        [YamlMember(Order = 1, Description = "Download official language packs")]
        public bool DownloadLocalePacks { get; set; }

        /// <param name="filename">Example: myfile.txt</param>
        /// <param name="path">Example: D:\MyDirectory</param>
        public ConfigurationBase(string filename, string path)
        {
            FilePath = path;
            FileName = filename;
            LoadDefaults();
        }

        public ConfigurationBase()
        {
            LoadDefaults();
        }

        public virtual void LoadDefaults()
        {

        }
    }
}
