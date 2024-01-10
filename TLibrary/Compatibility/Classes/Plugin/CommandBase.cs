using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;

namespace Tavstal.TLibrary.Compatibility
{
    public abstract class CommandBase : IRocketCommand
    {
        public abstract AllowedCaller AllowedCaller { get; }
        public abstract string Name { get; }
        public abstract string Help { get; }
        public abstract string Syntax { get; }
        public abstract List<string> Aliases { get; }
        public abstract List<string> Permissions { get; }

        public abstract List<SubCommand> SubCommands { get; }


        public virtual void Execute(IRocketPlayer caller, string[] command)
        {
            switch (AllowedCaller)
            {
                case AllowedCaller.Console:
                    {
                        if (caller is UnturnedPlayer)
                        {
                            // not allowed to use command
                            return;
                        }
                        break;
                    }
                case AllowedCaller.Player:
                    {
                        if (caller is ConsolePlayer)
                        {
                            // not allowed to use command
                            return;
                        }
                        break;
                    }
            }
        }

        public virtual void IsSubCommand()
        {

        }
    }
}
