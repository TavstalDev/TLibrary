using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.SymbolStore;
using System.CodeDom;
using Newtonsoft.Json;

namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Abstract class for all configurations.
    /// </summary>
    public abstract class ConfigurationBase : IConfigurationBase
    {
        [JsonIgnore]
        public string FilePath { get; set; }
        [JsonIgnore]
        public string FileName { get; set; }
        public string Locale { get; set; }
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
