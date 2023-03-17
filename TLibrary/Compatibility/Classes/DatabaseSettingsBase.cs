using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    public abstract class DatabaseSettingsBase
    {
        public string Host = "127.0.0.1";
        public int Port = 3306;
        public string DatabaseName = "unturned";
        public string UserName = "root";
        public string UserPassword = "ascent";
        public int TimeOut = 120;
    }
}
