using System;
using System.Threading.Tasks;
using Rocket.API;

namespace Tavstal.TLibrary.Models.Commands
{
    /// <summary>
    /// A sub-command that can be nested inside an <see cref="ICustomCommand"/>.
    /// </summary>
    public interface ISubcommand : ICustomCommand
    {
        /// <summary>Synchronous handler. Receives the caller and arguments, returns an optional post-execution action.</summary>
        Func<IRocketPlayer, string[], Action>? Action { get; }

        /// <summary>Asynchronous handler. Receives the caller and arguments, returns an optional post-execution task.</summary>
        Func<IRocketPlayer, string[], Task>? Task { get; }
    }
}