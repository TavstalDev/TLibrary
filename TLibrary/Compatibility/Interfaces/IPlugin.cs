using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Tavstal.TLibrary.Compatibility.Interfaces
{
    public interface IPlugin
    {
        
        void OnLoad();

        void OnUnLoad();

        string GetPluginName();

        TLogger GetLogger();

        void CheckPluginFiles();

        void InvokeAction(float delay, System.Action action);

        string Localize(bool AddPrefix, string translationKey, params object[] args);

        string Localize(string translationKey, params object[] args);
    }
}
