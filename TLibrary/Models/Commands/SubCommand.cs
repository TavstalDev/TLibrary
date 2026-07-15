using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Models.Commands
{
    /// <inheritdoc/>
    public class SubCommand : ISubcommand
    {
        /// <inheritdoc/>
        public AllowedCaller AllowedCaller { get; }

        /// <inheritdoc/>
        public string Name { get; }
        
        /// <inheritdoc/>
        public string Help { get; }
        
        /// <inheritdoc/>
        public string Syntax { get; }
        
        /// <inheritdoc/>
        public List<string> Aliases { get; }
        
        /// <inheritdoc/>
        public List<string> Permissions { get; }

        /// <inheritdoc/>
        public IPlugin Plugin { get; }

        /// <inheritdoc/>
        public bool UseBackgroundThread { get; }

        /// <inheritdoc/>
        public List<ISubcommand>? SubCommands { get; }
        
        /// <inheritdoc/>
        public Func<IRocketPlayer, string[], Action>? Action { get; }
        
        /// <inheritdoc/>
        public Func<IRocketPlayer, string[], Task>? Task { get; }

        public SubCommand(string name, string help, string syntax, List<string> aliases, List<string> permissions,
            IPlugin plugin, AllowedCaller allowedCaller, Func<IRocketPlayer, string[], Action> action)
        {
            AllowedCaller = allowedCaller;
            Name = name;
            Help = help;
            Syntax = syntax;
            Aliases = aliases;
            Permissions = permissions;
            Plugin = plugin;
            UseBackgroundThread = false;
            SubCommands = null;
            Action = action;
            Task = null;
        }
        
        public SubCommand(string name, string help, string syntax, List<string> aliases, List<string> permissions,
            IPlugin plugin, AllowedCaller allowedCaller, List<ISubcommand>? subCommands, Func<IRocketPlayer, string[], Action> action)
        {
            AllowedCaller = allowedCaller;
            Name = name;
            Help = help;
            Syntax = syntax;
            Aliases = aliases;
            Permissions = permissions;
            Plugin = plugin;
            UseBackgroundThread = false;
            SubCommands = subCommands;
            Action = action;
            Task = null;
        }
        
        public SubCommand(string name, string help, string syntax, List<string> aliases, List<string> permissions,
            IPlugin plugin, AllowedCaller allowedCaller, Func<IRocketPlayer, string[], Task>? task)
        {
            AllowedCaller = allowedCaller;
            Name = name;
            Help = help;
            Syntax = syntax;
            Aliases = aliases;
            Permissions = permissions;
            Plugin = plugin;
            UseBackgroundThread = true;
            SubCommands = null;
            Action = null;
            Task = task;
        }
        
        public SubCommand(string name, string help, string syntax, List<string> aliases, List<string> permissions,
            IPlugin plugin, AllowedCaller allowedCaller, List<ISubcommand>? subCommands, Func<IRocketPlayer, string[], Task>? task)
        {
            AllowedCaller = allowedCaller;
            Name = name;
            Help = help;
            Syntax = syntax;
            Aliases = aliases;
            Permissions = permissions;
            Plugin = plugin;
            UseBackgroundThread = true;
            SubCommands = subCommands;
            Action = null;
            Task = task;
        }

        void IRocketCommand.Execute(IRocketPlayer caller, string[] command)
        {
            throw new InvalidOperationException("Subcommands should not be executed directly. They should be executed through their parent command.");
        }
    }
}
