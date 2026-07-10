using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API;

namespace Tavstal.TLibrary.Models.Commands
{
    /// <inheritdoc/>
    public class SubCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name { get; private set; }
        
        /// <inheritdoc/>
        public string Help { get; private set; }
        
        /// <inheritdoc/>
        public string Syntax { get; private set; }
        
        /// <inheritdoc/>
        public List<string> Aliases { get; private set; }
        
        /// <inheritdoc/>
        public List<string> Permissions { get; private set; }

        /// <summary>
        /// The action that stores what the subcommand should do
        /// </summary>
        public Func<IRocketPlayer, string[], Task> ActionToExecute { get; private set; }

        /// <summary>
        /// Executes the <see cref="ActionToExecute"/> action.
        /// </summary>
        /// <param name="caller">The player who executed the command.</param>
        /// <param name="args">The arguments passed to the command.</param>
        public async Task Execute(IRocketPlayer caller, string[] args) =>
            await ActionToExecute(caller, args);

        /// <summary>
        /// Creates a new subcommand with the given properties.
        /// </summary>
        /// <param name="name">The name of the subcommand.</param>
        /// <param name="help">The help text for the subcommand.</param>
        /// <param name="syntax">The syntax description of the subcommand.</param>
        /// <param name="aliases">Alternative names for the subcommand.</param>
        /// <param name="permissions">The permissions required to use the subcommand.</param>
        /// <param name="codeToExecute">The function to run when the subcommand is executed.</param>
        public SubCommand(string name, string help, string syntax, List<string> aliases, List<string> permissions, Func<IRocketPlayer, string[], Task> codeToExecute)
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
