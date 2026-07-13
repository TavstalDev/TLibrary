using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Hooks;
using Tavstal.TLibrary.Models.Logging;
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
            _logger = new TLogger(plugin.GetPluginName(), GetType(), plugin.GetLogLevel());
        }

        /// <summary>
        /// Loads hooks for the specified type.
        /// </summary>
        /// <param name="type">The type of hooks to load.</param>
        public void Load(Type type)
        {
            if (!typeof(Hook).IsAssignableFrom(type))
            {
                _logger.Debug($"'{type.Name}' is not a hook.");
                return;
            }

            if (type.IsAbstract)
            {
                _logger.Debug($"'{type.Name}' is not a abstract class.");
                return;
            }

            var hook = CreateInstance<Hook>(type);
            if (hook == null)
            {
                _logger.Debug($"Failed to create '{type.Name}' hook.");
                return;
            }
            
            if (_hooks.ContainsKey(hook.Name))
            {
                _logger.Debug($"Hook with '{hook.Name}' name already exists.");
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
            foreach (Type type in pluginAssembly.GetTypes().ToList().FindAll(x => !x.IsAbstract && typeof(Hook).IsAssignableFrom(x)))
            {
                var hook = CreateInstance<Hook>(type);
                if (hook == null)
                {
                    _logger.Debug($"'{type.Name}' is not a hook.");
                    continue;
                }
                
                try
                {
                    if (_hooks.ContainsKey(hook.Name))
                    {
                        _logger.Debug($"Hook with '{hook.Name}' name already exists.");
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
                    _logger.Error($"Failed to create '{type.Name}' hook.", ex);
                }
            }
        }

        /// <summary>
        /// Unloads the hooks of the specified type.
        /// </summary>
        /// <param name="type">The type of hooks to unload.</param>
        public void Unload(Type type)
        {
            if (!typeof(Hook).IsAssignableFrom(type))
            {
                _logger.Debug($"'{type.Name}' is not a hook.");
                return;
            }

            if (_hooks.All(x => x.Value.GetType() != type))
            {
                _logger.Debug($"'{type.Name}' is not loaded.");
                return;
            }

            var hook = _hooks.Values.FirstOrDefault(x => x.GetType() == type);
            if (hook == null)
            {
               _logger.Debug($"'{type.Name}' hook is not found.");
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
        public T? GetHook<T>() where T : Hook
        {
            foreach (var hook in _hooks)
            {
                if (hook.Value.GetType() == typeof(T))
                    return (T)hook.Value;
            }
            return null;
        }

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="type">The type of the instance to create.</param>
        /// <returns>An instance of the specified type if creation is successful; otherwise, null.</returns>
        private T? CreateInstance<T>(Type type) where T : Hook
        {
            try
            {
                return (T)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to create '{type.Name}' hook.", ex);
                return null;
            }
        }
    }
}
