using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Player;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;

namespace Tavstal.TLibrary.Extensions.Unturned
{
    /// <summary>
    /// Provides extension methods for <see cref="ICustomCommand"/>.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Checks whether the <paramref name="caller"/> is allowed to invoke the <paramref name="command"/> based on its <see cref="ICustomCommand.AllowedCaller"/> setting.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <param name="caller">The player attempting to execute the command.</param>
        /// <returns><c>true</c> if the caller is allowed; otherwise <c>false</c>.</returns>
        public static bool IsAllowedCaller(this ICustomCommand command, IRocketPlayer caller)
        {
            switch (command.AllowedCaller)
            {
                case AllowedCaller.Console:
                {
                    if (caller is UnturnedPlayer)
                    {
                        command.Plugin.SendCommandReply(caller, "commands_common_error_player_caller");
                        return false;
                    }
                    break;
                }
                case AllowedCaller.Player:
                {
                    if (caller is ConsolePlayer)
                    {
                        command.Plugin.SendCommandReply(caller, "commands_common_error_console_caller");
                        return false;
                    }
                    break;
                }
                case AllowedCaller.Both:
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the <paramref name="caller"/> holds at least one of the permissions required by the <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command whose permissions are checked.</param>
        /// <param name="caller">The player whose permissions are evaluated.</param>
        /// <returns><c>true</c> if the caller has the required permissions or no permissions are defined; otherwise <c>false</c>.</returns>
        public static bool HasPermission(this ICustomCommand command, IRocketPlayer caller)
        {
            if (caller is ConsolePlayer)
                return true;
            
            if (command.Permissions.Count == 0)
                return true;
            
            if (!command.Permissions.Any(caller.HasPermission))
            {
                command.Plugin.SendCommandReply(caller, "commands_common_error_permission");
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Sends a help or error message to the <paramref name="caller"/> showing the syntax of the command or one of its sub-commands.
        /// </summary>
        /// <param name="command">The command whose syntax is displayed.</param>
        /// <param name="caller">The player receiving the message.</param>
        /// <param name="isError">When set to <c>true</c> an error translation is used; otherwise a usage translation is used.</param>
        /// <param name="subCommandName">Optional name or alias of a sub-command whose syntax should be shown instead.</param>
        public static void ExecuteHelp(this ICustomCommand command, IRocketPlayer caller, bool isError, string? subCommandName = null)
        {
            string translation = isError ? "commands_common_error_syntax" : "commands_common_usage";
            if (subCommandName != null)
            {
                var subCommand = command.SubCommands?.Find(x => x.Name.Equals(subCommandName, StringComparison.OrdinalIgnoreCase) || x.Aliases.Exists(alias => alias.Equals(subCommandName, StringComparison.OrdinalIgnoreCase)));
                if (subCommand != null)
                {
                    command.Plugin.SendCommandReply(caller, translation, command.Name, subCommand.Syntax);
                    return;
                }
            }
            
            command.Plugin.SendCommandReply(caller, translation, command.Name, command.Syntax);
        }
    }
}