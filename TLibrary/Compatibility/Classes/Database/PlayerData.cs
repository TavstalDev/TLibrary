using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Classes.Database
{
    public class PlayerData
    {
        [SqlPrimaryKey]
        [SqlNonNullable]
        [SqlFieldType("int(11)")]
        public int Id { get; private set; }
        [SqlNonNullable]
        [SqlUniqueKey]
        public ulong SteamId { get; private set; }
        [SqlNonNullable]
        [SqlFieldType("varchar(255)")]
        public string Name { get; private set; }
        [SqlNonNullable]
        [SqlFieldType("varchar(5)")]
        public string Language { get; private set; }
        public DateTime LastUpdate { get; set; }
    }
}
