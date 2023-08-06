using MySql.Data.MySqlClient;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Interfaces
{
    public interface IDatabaseManager
    {
        IConfigurationBase _configuration { get; }

        MySqlConnection CreateConnection();

        void CheckSchema();
    }
}
