namespace Tavstal.TLibrary.Models.Plugin
{
    public interface IPlugin
    {
        
        void OnLoad();

        void OnUnLoad();

        string GetPluginName();

        TLogger GetLogger();

        void CheckPluginFiles();

        void InvokeAction(float delay, System.Action action);

        string Localize(bool addPrefix, string translationKey, params object[] args);

        string Localize(string translationKey, params object[] args);
    }
}
