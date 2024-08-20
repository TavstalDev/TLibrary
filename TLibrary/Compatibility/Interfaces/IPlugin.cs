using Tavstal.TLibrary.Compatibility.Models.Plugin;

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
