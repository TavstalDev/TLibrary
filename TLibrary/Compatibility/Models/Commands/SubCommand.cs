using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Interfaces;

namespace Tavstal.TLibrary.Compatibility
{
    public class SubCommand : ICommand
    {
        public string Name { get; private set; }

        public string Help { get; private set; }

        public string Syntax { get; private set; }

        public List<string> Aliases { get; private set; }

        public List<string> Permissions { get; private set; }

        public Action<IRocketPlayer, string[]> ActionToExecute { get; private set; }

        public void Execute(IRocketPlayer caller, string[] args)
        {
            ActionToExecute.Invoke(caller, args);
        }

        public SubCommand() { }

        public SubCommand(string name, string help, string syntax, List<string> aliases, List<string> permissions, Action<IRocketPlayer, string[]> codeToExecute)
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
