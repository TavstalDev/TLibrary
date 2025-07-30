using System;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Models.Hooks
{
    /// <summary>
    /// Abstract hook class used for handling functions based on thirdparty plugins.
    /// </summary>
    public abstract class Hook
    {
        /// <summary>
        /// The plugin instance that loads this hook, NOT THE PLUGIN THAT IS BEING HOOKED.
        /// </summary>
        public IPlugin Plugin { get; private set; }
        /// <summary>
        /// Name of the plugin that should be hooked
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// If the hook is essential, the plugin will not load if the hook fails to load.
        /// </summary>
        public bool IsEssential { get; private set; }
        /// <summary>
        /// If the hook is loaded
        /// </summary>
        public bool IsLoaded { get; private set; }

        protected Hook() { }

        protected Hook(IPlugin loaderPluginInstance, string name, bool isEssential)
        {
            Plugin = loaderPluginInstance;
            Name = name;
            IsEssential = isEssential;
        }

        /// <summary>
        /// Function used to load the hook.
        /// </summary>
        internal void Load()
        {
            if (!CanBeLoaded())
                return;

            IsLoaded = true;

            try
            {
                OnLoad();
            }
            catch (Exception ex)
            {
                Plugin.GetLogger().Error($"Failed to load '{Name}' hook.");
                Plugin.GetLogger().Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Function used to unload the hook.
        /// </summary>
        internal void Unload()
        {
            IsLoaded = false;

            try
            {
                OnUnload();
            }
            catch (Exception ex)
            {
                IsLoaded = false;
                Plugin.GetLogger().Error($"Failed to unload '{Name}' hook.");
                Plugin.GetLogger().Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Abstract function that is called when the hook is loaded.
        /// </summary>
        public abstract void OnLoad();

        /// <summary>
        /// Abstract function that is called when the hook is unloaded.
        /// </summary>
        public abstract void OnUnload();

        /// <summary>
        /// Abstract function that is called to check if the hook can be loaded.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanBeLoaded();

    }
}
