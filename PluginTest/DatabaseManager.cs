using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary;
using Tavstal.TLibrary.Compatibility.Classes.Database;
using Tavstal.TLibrary.Compatibility.Classes.Plugin;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Managers;

namespace PluginTest
{
    public class DatabaseManager : DatabaseManagerBase
    {
        public DatabaseManager(IConfigurationBase configuration) : base(configuration)
        {
        }

        public override void CheckSchema()
        {
            var connection = CreateConnection();
            if (connection.DoesTableExist<TestData>())
                connection.CheckTable<TestData>();
            else
                connection.CreateTable<TestData>();
        }
    }

    [SqlName("test_table")]
    public class TestData
    {
        [SqlPrimaryKey]
        [SqlNonNullable]
        public int Id { get; set; }
        [SqlUniqueKey]
        [SqlNonNullable]
        public ulong SteamId { get; set; }
    }
}
