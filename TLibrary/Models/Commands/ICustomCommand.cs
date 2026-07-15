using System.Collections.Generic;
using Rocket.API;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Models.Commands
{
    /// <summary>
    /// Extends <see cref="IRocketCommand"/> with plugin ownership, threading, and sub-command support.
    /// </summary>
    public interface ICustomCommand : IRocketCommand
    {
        /// <summary>
        /// The plugin that owns this command.
        /// </summary>
        IPlugin Plugin { get; }

        /// <summary>
        /// Whether the command should run on a background thread.
        /// </summary>
        bool UseBackgroundThread { get; }

        /// <summary>
        /// Optional list of sub-commands supported by this command.
        /// </summary>
        List<ISubcommand>? SubCommands { get; }
    }
}