using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;

namespace Tavstal.TLibrary.Managers
{
    public class HookManager
    {
        private readonly IPlugin _plugin;
        private readonly TLogger _logger;
        private readonly Dictionary<string, Hook> _hooks = new Dictionary<string, Hook>();
        public IEnumerable<Hook> Hooks => _hooks.Values;

        public HookManager(IPlugin plugin) {
            _plugin = plugin;
            _logger = _plugin.GetLogger();
        }

        public void Load(Type type)
        {
            if (!typeof(Hook).IsAssignableFrom(type))
            {
                _logger.LogException("'{0}' is not a hook.");
                return;
            }

            if (type.IsAbstract)
            {
                _logger.LogException(string.Format("Cannot register {0} because it is abstract.", type.Name));
                return;
            }

            var hook = CreateInstance<Hook>(type);
            if (_hooks.ContainsKey(hook.Name))
            {
                _logger.LogException("Hook with '{0}' name already exists.");
                return;
            }

            _hooks.Add(hook.Name, hook);
            hook.Load();
        }

        public void LoadAll(Assembly pluginAssembly)
        {
            foreach (Type t in pluginAssembly.GetTypes().ToList().FindAll(x => !x.IsAbstract && typeof(Hook).IsAssignableFrom(x)))
            {
                var hook = CreateInstance<Hook>(t);
                try
                {
                    if (_hooks.ContainsKey(hook.Name))
                    {
                        _logger.LogException(string.Format("Hook with '{0}' name already exists.", hook.Name));
                        continue;
                    }

                    _hooks.Add(hook.Name, hook);
                    hook.Load();
                }
                catch (Exception ex)
                {
                    _logger.LogException(string.Format("Failed to add the '{0}' hook to the library.", hook.Name));
                    _logger.LogError(ex);
                }
            }
        }

        public void Unload(Type type)
        {
            if (type != typeof(Hook))
            {
                _logger.LogException(string.Format("'{0}' is not a hook.", type.Name));
                return;
            }


            if (!_hooks.Any(x => x.Value.GetType() == type))
            {

                _logger.LogException(string.Format("'{0}' is not loaded.", type.Name));
                return;
            }

            var hook = _hooks.Values.FirstOrDefault(x => x.GetType() == type);
            if (hook == null)
            {
                _logger.LogException(string.Format("'{0}' is not found.", type.Name));
                return;
            }

            hook.Unload();
            _hooks.Remove(hook.Name);
        }

        public void UnLoadAll()
        {
            foreach (var value in _hooks.Values)
            {
                value.Unload();
            }
            _hooks.Clear();
        }

        public bool IsHookFound<T>()
        {
            foreach (var hook in _hooks)
            {
                if (hook.Value.GetType() == typeof(T))
                    return true;
            }
            return false;
        }

        public bool IsHookLoadable<T>()
        {
            foreach (var hook in _hooks)
            {
                if (hook.Value.GetType() == typeof(T))
                    return hook.Value.CanBeLoaded();
            }
            return false;
        }

        public T GetHook<T>()
        {
            foreach (var hook in _hooks)
            {
                if (hook.Value.GetType() == typeof(T))
                    return (T)(object)hook.Value;
            }
            return default;
        }

        private T CreateInstance<T>(Type type)
        {
            try
            {
                return (T)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                _logger.LogException(string.Format("Failed to create instance for '{0}' hook.", type.Name));
                _logger.LogError(ex);
                return default;
            }

        }
    }
}
