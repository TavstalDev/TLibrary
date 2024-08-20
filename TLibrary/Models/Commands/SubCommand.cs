using System;
using System.Collections.Generic;
using Rocket.API;

namespace Tavstal.TLibrary.Models.Commands
{
    /// <summary>
    /// Class used to help in creating subcommands
    /// </summary>
    public abstract class SubCommand : ICommand
    {
        /// <summary>
        /// Name of the subcommand
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Description of the subcommand
        /// </summary>
        public string Help { get; private set; }
        /// <summary>
        /// Example usage of the subcommand
        /// <br/>Example: "give [itemId]&lt;amount>"
        /// </summary>
        public string Syntax { get; private set; }
        /// <summary>
        /// Other acceptabla names
        /// </summary>
        public List<string> Aliases { get; private set; }
        /// <summary>
        /// Permissions that the subcommand require
        /// </summary>
        public List<string> Permissions { get; private set; }

        /// <summary>
        /// The action that stores what the subcommand should do
        /// </summary>
        public Action<IRocketPlayer, string[]> ActionToExecute { get; private set; }

        /// <summary>
        /// Executes the <see cref="ActionToExecute"/> action
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="args"></param>
        public void Execute(IRocketPlayer caller, string[] args)
        {
            ActionToExecute.Invoke(caller, args);
        }

        protected SubCommand() { }

        protected SubCommand(string name, string help, string syntax, List<string> aliases, List<string> permissions, Action<IRocketPlayer, string[]> codeToExecute)
        {
            Name = name;
            Help = help;
            Syntax = syntax;
            Aliases = aliases;
            Permissions = permissions;
            ActionToExecute = codeToExecute;
        }
    }
}
