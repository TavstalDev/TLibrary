using MySql.Data.MySqlClient;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Interfaces
{
    public interface IDatabaseSettings
    {
        string Host { get; set; }
        int Port { get; set; }
        string DatabaseName { get; set; }
        string UserName { get; set; }
        string UserPassword { get; set; }
        int TimeOut { get; set; }
    }
}
