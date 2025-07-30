using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tavstal.TLibrary.Models.Hooks;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Managers
{
    /// <summary>
    /// Manages hooks for plugins.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HookManager
    {
        private readonly TLogger _logger;
        private readonly Dictionary<string, Hook> _hooks = new Dictionary<string, Hook>();
        public IEnumerable<Hook> Hooks => _hooks.Values;

        public HookManager(IPlugin plugin) {
            _logger = TLogger.CreateInstance("TLibrary", this.GetType(), plugin.GetLogger().IsDebugEnabled);
        }

        /// <summary>
        /// Loads hooks for the specified type.
        /// </summary>
        /// <param name="type">The type of hooks to load.</param>
        public void Load(Type type)
        {
            if (!typeof(Hook).IsAssignableFrom(type))
            {
                _logger.Exception("'{0}' is not a hook.");
                return;
            }

            if (type.IsAbstract)
            {
                _logger.Exception($"Cannot register {type.Name} because it is abstract.");
                return;
            }

            var hook = CreateInstance<Hook>(type);
            if (_hooks.ContainsKey(hook.Name))
            {
                _logger.Exception("Hook with '{0}' name already exists.");
                return;
            }

            _hooks.Add(hook.Name, hook);
            hook.Load();
        }

        /// <summary>
        /// Loads all hooks from the specified plugin assembly.
        /// </summary>
        /// <param name="pluginAssembly">The assembly containing the plugin hooks to load.</param>
        /// <param name="ignoreDuplicate"></param>
        public void LoadAll(Assembly pluginAssembly, bool ignoreDuplicate = false)
        {
            foreach (Type t in pluginAssembly.GetTypes().ToList().FindAll(x => !x.IsAbstract && typeof(Hook).IsAssignableFrom(x)))
            {
                var hook = CreateInstance<Hook>(t);
                try
                {
                    if (_hooks.ContainsKey(hook.Name))
                    {
                        _logger.Exception($"Hook with '{hook.Name}' name already exists.");
                        continue;
                    }

                    if (_hooks.ContainsKey(hook.Name))
                    {
                        if (!ignoreDuplicate)
                            throw new Exception($"Hook with '{hook.Name}' name already exists.");
                        continue;
                    }
                    
                    _hooks.Add(hook.Name, hook);
                    hook.Load();
                }
                catch (Exception ex)
                {
                    _logger.Exception($"Failed to add the '{hook.Name}' hook to the library.");
                    _logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// Unloads the hooks of the specified type.
        /// </summary>
        /// <param name="type">The type of hooks to unload.</param>
        public void Unload(Type type)
        {
            if (type != typeof(Hook))
            {
                _logger.Exception($"'{type.Name}' is not a hook.");
                return;
            }

            if (!_hooks.Any(x => x.Value.GetType() == type))
            {
                _logger.Exception($"'{type.Name}' is not loaded.");
                return;
            }

            var hook = _hooks.Values.FirstOrDefault(x => x.GetType() == type);
            if (hook == null)
            {
                _logger.Exception($"'{type.Name}' is not found.");
                return;
            }

            hook.Unload();
            _hooks.Remove(hook.Name);
        }

        /// <summary>
        /// Unloads all hooks.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var value in _hooks.Values)
            {
                value.Unload();
            }
            _hooks.Clear();
        }

        /// <summary>
        /// Checks if a hook of the specified type is found.
        /// </summary>
        /// <typeparam name="T">The type of hook to check for.</typeparam>
        /// <returns>True if a hook of the specified type is found; otherwise, false.</returns>
        public bool IsHookFound<T>()
        {
            foreach (var hook in _hooks)
            {
                if (hook.Value.GetType() == typeof(T))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a hook of the specified type is loadable.
        /// </summary>
        /// <typeparam name="T">The type of hook to check.</typeparam>
        /// <returns>True if a hook of the specified type is loadable; otherwise, false.</returns>
        public bool IsHookLoadable<T>()
        {
            foreach (var hook in _hooks)
            {
                if (hook.Value.GetType() == typeof(T))
                    return hook.Value.CanBeLoaded();
            }
            return false;
        }

        /// <summary>
        /// Gets the hook instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of hook to retrieve.</typeparam>
        /// <returns>The hook instance of the specified type if found; otherwise, null.</returns>
        public T GetHook<T>()
        {
            foreach (var hook in _hooks)
            {
                if (hook.Value.GetType() == typeof(T))
                    return (T)(object)hook.Value;
            }
            return default;
        }

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="type">The type of the instance to create.</param>
        /// <returns>An instance of the specified type if creation is successful; otherwise, null.</returns>
        private T CreateInstance<T>(Type type)
        {
            try
            {
                return (T)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                _logger.Exception($"Failed to create instance for '{type.Name}' hook.");
                _logger.Error(ex);
                return default;
            }
        }
    }
}
