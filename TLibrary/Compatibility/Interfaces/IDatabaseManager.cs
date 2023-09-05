using MySql.Data.MySqlClient;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    public interface IDatabaseManager
    {
        IConfigurationBase _configuration { get; }

        MySqlConnection CreateConnection();
    }
}
