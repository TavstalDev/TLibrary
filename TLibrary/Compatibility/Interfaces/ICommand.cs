﻿using Rocket.API;
using System.Collections.Generic;

namespace Tavstal.TLibrary.Compatibility.Interfaces
{
    /// <summary>
    /// Base interface for all commands.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Name of the command, this is what the user will type in chat to execute the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the command, this will be displayed in the help menu.
        /// </summary>
        string Help { get; }

        /// <summary>
        /// How the command should be used, this will be displayed in the help menu.
        /// </summary>
        string Syntax { get; }

        /// <summary>
        /// Alternate names for the command, these can also be used to execute the command.
        /// </summary>
        List<string> Aliases { get; }

        /// <summary>
        /// Permissions required to execute the command.
        /// </summary>
        List<string> Permissions { get; }

        /// <summary>
        /// The function that will be executed when the command is executed.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="args"></param>
        void Execute(IRocketPlayer caller, string[] args);
    }
}
