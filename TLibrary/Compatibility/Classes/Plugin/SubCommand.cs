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

        public virtual void Execute(IRocketPlayer caller, string[] args)
        {

        }
    }
}
