using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Interfaces
{
    public interface ICommand
    {

        string Name { get; }

        string Help { get; }

        string Syntax { get; }

        List<string> Aliases { get; }

        List<string> Permissions { get; }

        void Execute(IRocketPlayer caller, string[] command);
    }
}
