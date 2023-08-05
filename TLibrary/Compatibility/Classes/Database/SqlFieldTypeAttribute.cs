using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Classes.Database
{
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SqlFieldTypeAttribute : Attribute
    {
        public readonly string Type;

        public SqlFieldTypeAttribute(string type)
        {
            Type = type;
        }
    }
}
