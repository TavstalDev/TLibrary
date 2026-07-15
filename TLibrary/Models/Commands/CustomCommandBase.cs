using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API;
using Tavstal.TLibrary.Extensions.Unturned;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Threading;

namespace Tavstal.TLibrary.Models.Commands
{
    /// <summary>
    /// Base implementation of <see cref="ICustomCommand"/> that handles caller validation, permission checks,
    /// sub-command routing, and background-thread dispatching.
    /// </summary>
    public abstract class CustomCommandBase : ICustomCommand
    {
        public abstract AllowedCaller AllowedCaller { get; }
        public abstract string Name { get; }
        public abstract string Help { get; }
        public abstract string Syntax { get; }
        public abstract List<string> Aliases { get; }
        public abstract List<string> Permissions { get; }
        public abstract IPlugin Plugin { get; }
        public abstract bool UseBackgroundThread { get; }
        public abstract List<ISubcommand>? SubCommands { get; }

        /// <summary>
        /// Entry point called by the command framework. Validates the caller, routes to sub-commands,
        /// and delegates to <see cref="HandleExecute"/> or <see cref="HandleExecuteAsync"/>.
        /// </summary>
        /// <param name="caller">The player or console executing the command.</param>
        /// <param name="args">Arguments passed to the command.</param>
        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (!this.IsAllowedCaller(caller))
                return;
                
            if (!this.HasPermission(caller))
                return;
            
            bool hasArgs = args.Length > 0;
            if (hasArgs && SubCommands?.Count > 0)
            {
                var subCommand = SubCommands.FirstOrDefault(x => x.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase) || 
                                                            x.Aliases.Exists(alias => alias.Equals(args[0], StringComparison.OrdinalIgnoreCase)));
                if (subCommand != null)
                {
                    if (!subCommand.IsAllowedCaller(caller))
                        return;
                    
                    if (!subCommand.HasPermission(caller))
                        return;

                    if (!subCommand.UseBackgroundThread)
                    {
                        subCommand.Action?.Invoke(caller, args.Skip(1).ToArray());
                        return;
                    }

                    BackgroundThreadDispatcher.Run(async () =>
                    {
                        await LockHelper.WaitForLockAsync(caller, ELockKind.COMMAND);
                        try
                        {
                            await subCommand.Task?.Invoke(caller, args.Skip(1).ToArray())!;
                        }
                        finally
                        {
                            LockHelper.ReleaseLock(caller, ELockKind.COMMAND);
                        }
                    });
                    return;
                }
            }

            if (hasArgs && (args[0].Equals("help", StringComparison.OrdinalIgnoreCase) || args[0].Equals("?")))
            {
                this.ExecuteHelp(caller, false);
                return;
            }

            if (!UseBackgroundThread)
            {
                if (!HandleExecute(caller, args))
                    this.ExecuteHelp(caller, true);
                return;
            }
            
            BackgroundThreadDispatcher.Run(async () =>
            {
                await LockHelper.WaitForLockAsync(caller, ELockKind.COMMAND);
                try
                {
                    if (!await HandleExecuteAsync(caller, args))
                        await MainThreadDispatcher.RunAsync(() => this.ExecuteHelp(caller, true));
                }
                finally
                {
                    LockHelper.ReleaseLock(caller, ELockKind.COMMAND);
                }
            });
        }

        /// <summary>Synchronous command handler. Override to implement the command logic.</summary>
        /// <param name="caller">The player or console executing the command.</param>
        /// <param name="command">Arguments passed to the command.</param>
        /// <returns><c>true</c> if the command was handled successfully; <c>false</c> to show usage help.</returns>
        protected virtual bool HandleExecute(IRocketPlayer caller, string[] command) => false;

        /// <summary>Asynchronous command handler. Override to implement the command logic on a background thread.</summary>
        /// <param name="caller">The player or console executing the command.</param>
        /// <param name="command">Arguments passed to the command.</param>
        /// <returns><c>true</c> if the command was handled successfully; <c>false</c> to show usage help.</returns>
        protected virtual Task<bool> HandleExecuteAsync(IRocketPlayer caller, string[] command) => Task.FromResult(false);
    }
}