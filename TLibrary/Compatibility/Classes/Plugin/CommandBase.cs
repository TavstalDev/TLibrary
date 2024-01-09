using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Commands;

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


        public abstract void Execute(IRocketPlayer caller, string[] command);
    }
}
