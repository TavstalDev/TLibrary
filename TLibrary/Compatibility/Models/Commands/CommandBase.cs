using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Player;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TLibrary.Compatibility.Models.Commands
{
    /// <summary>
    /// Abstract class for creating commands
    /// </summary>
    public abstract class CommandBase : IRocketCommand
    {
        /// <summary>
        /// The plugin that owns the command
        /// Example usage: MyPlugin.Instance
        /// </summary>
        protected abstract IPlugin Plugin { get; }

        /// <summary>
        /// The allowed caller of the command
        /// <br/>Values: Console, Player, Both
        /// </summary>
        public abstract AllowedCaller AllowedCaller { get; }
        /// <summary>
        /// Name of the command
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Description of the command
        /// </summary>
        public abstract string Help { get; }
        /// <summary>
        /// Example usage of the command
        /// <br/>Example: "help | action1 | action2"
        /// </summary>
        public abstract string Syntax { get; }
        /// <summary>
        /// Other acceptabla names
        /// </summary>
        public abstract List<string> Aliases { get; }
        /// <summary>
        /// Permissions that the command require
        /// </summary>
        public abstract List<string> Permissions { get; }

        /// <summary>
        /// Subcommands like /example help
        /// <br/>help can be modified with on ExecuteHelp
        /// </summary>
        protected abstract List<SubCommand> SubCommands { get; }

        /// <summary>
        /// Called when the command is executed
        /// </summary>
        /// <returns>True when the arguments were correctly used</returns>
        /// <param name="caller"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract bool ExecutionRequested(IRocketPlayer caller, string[] args);

        /// <summary>
        /// Called when the command is executed with help subcommand
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="isError"></param>
        /// <param name="subcommand"></param>
        /// <param name="args"></param>
        protected virtual void ExecuteHelp(IRocketPlayer caller, bool isError, string subcommand, string[] args)
        {
            string translation = isError ? "error_command_syntax" : "success_command_help";
            if (args == null || args.Length == 0)
            {
                Plugin.SendCommandReply(caller, translation, Name, Syntax);
                return;
            }

            if (subcommand != null)
            {
                SubCommand subCommand = GetSubCommandByName(subcommand);
                if (subCommand != null)
                {
                    Plugin.SendCommandReply(caller, translation, Name, subCommand.Syntax);
                    return;
                }
            }

            Plugin.SendCommandReply(caller, translation, Name, Syntax);
        }

        /// <summary>
        /// Rocket ICommand Execute implementation
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="args"></param>
        public void Execute(IRocketPlayer caller, string[] args)
        {
            bool isPlayer = caller is UnturnedPlayer;

            // Check AllowedCaller
            switch (AllowedCaller)
            {
                case AllowedCaller.Console:
                    {
                        if (caller is UnturnedPlayer)
                        {
                            Plugin.SendCommandReply(caller, "error_command_caller_not_console");
                            return;
                        }
                        break;
                    }
                case AllowedCaller.Player:
                    {
                        if (caller is ConsolePlayer)
                        {
                            Plugin.SendCommandReply(caller, "error_command_caller_not_player");
                            return;
                        }
                        break;
                    }
                case AllowedCaller.Both:
                default:
                    break;
            }

            // Check Permission
            if (isPlayer && !Permissions.Any(caller.HasPermission))
            {
                Plugin.SendCommandReply(caller, "error_command_no_permission");
                return;
            }

            if (args.Length > 0 && SubCommands.IsValidIndex(0))
            {
                SubCommand subCommand = GetSubCommandByName(args[0]);
                if (subCommand != null)
                {
                    if (isPlayer && !subCommand.Permissions.Any(caller.HasPermission))
                    {
                        Plugin.SendCommandReply(caller, "error_command_no_permission");
                        return;
                    }

                    List<string> argList = args.ToList();
                    argList.RemoveAt(0);
                    subCommand.Execute(caller, argList.ToArray());
                    return;
                }
            }
            
            if (args.Length > 0 && (args[0].ToLower() == "help" || args[0].ToLower() == "?"))
            {
                List<string> argList = args.ToList();
                argList.RemoveAt(0);
                ExecuteHelp(caller, false, null, argList.ToArray());
            }
            else
            {
                if (!ExecutionRequested(caller, args))
                    ExecuteHelp(caller, true, null, null);
            }
        }

        private SubCommand GetSubCommandByName(string arg)
        {
            return SubCommands.Find(x => x.Name.ToLower() == arg.ToLower() || x.Aliases.Contains(arg.ToLower()));
        }
    }
}
