﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers;

namespace Tavstal.TLibrary.Compatibility
{
    public abstract class CommandBase : IRocketCommand
    {
        public abstract IPlugin Plugin { get; }
        public abstract AllowedCaller AllowedCaller { get; }
        public abstract string Name { get; }
        public abstract string Help { get; }
        public abstract string Syntax { get; }
        public abstract List<string> Aliases { get; }
        public abstract List<string> Permissions { get; }

        public abstract List<SubCommand> SubCommands { get; }

        public abstract void ExecutionRequested(IRocketPlayer caller, string[] args);

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
                            UChatHelper.SendCommandReply(Plugin, caller, "command_caller_not_console");
                            return;
                        }
                        break;
                    }
                case AllowedCaller.Player:
                    {
                        if (caller is ConsolePlayer)
                        {
                            UChatHelper.SendCommandReply(Plugin, caller, "command_caller_not_player");
                            return;
                        }
                        break;
                    }
            }

            // Check Permission
            if (isPlayer && !Permissions.Any(x => caller.HasPermission(x)))
            {
                UChatHelper.SendCommandReply(Plugin, caller, "error_command_no_permission");
                return;
            }

            if (args.Length > 0 && SubCommands.IsValidIndex(0))
            {
                SubCommand subCommand = GetSubCommandByName(args[0]);
                if (subCommand != null)
                {
                    if (isPlayer && !subCommand.Permissions.Any(x => caller.HasPermission(x)))
                    {
                        UChatHelper.SendCommandReply(Plugin, caller, "error_command_no_permission");
                        return;
                    }

                    if (args.Remove(x => x == args[0]))
                        subCommand.Execute(caller, args);
                    else
                        UChatHelper.SendCommandReply(Plugin, caller, "error_subcommand_not_found", Syntax);
                }
            }
            else
                ExecutionRequested(caller, args);
        }

        private SubCommand GetSubCommandByName(string arg)
        {
            return SubCommands.Find(x => x.Name.ToLower() == arg.ToLower() || x.Aliases.Contains(arg.ToLower()));
        }
    }
}
