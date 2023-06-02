using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Classes.Database
{
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class SqlNonNullableAttribute : Attribute
    {
    }
}
