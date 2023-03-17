using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    public interface IPluginProvider
    {
        T GetConfigValue<T>(string VariableName);
        JObject GetConfig();
        string Translate(string translationKey, params object[] placeholder);
    }
}
