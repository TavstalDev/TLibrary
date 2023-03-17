using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Tavstal.TLibrary.Helpers.LoggerHelper;

namespace Tavstal.TLibrary.Compatibility
{
    public abstract class Hook
    {
        public string Name { get; private set; }
        public bool IsEssential { get; private set; }
        public bool IsLoaded { get; private set; }

        protected Hook() { }

        protected Hook(string name, bool isEssential)
        {
            Name = name;
            IsEssential = isEssential;
        }

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
                Logger.LogError($"Failed to load '{Name}' hook.");
                Logger.LogException(ex.ToString());
            }
        }

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
                Logger.LogError($"Failed to unload '{Name}' hook.");
                Logger.LogException(ex.ToString());
            }
        }

        public abstract void OnLoad();

        public abstract void OnUnload();

        public abstract bool CanBeLoaded();

    }
}
