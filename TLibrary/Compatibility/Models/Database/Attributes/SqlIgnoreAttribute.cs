using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Database
{
    /// <summary>
    /// SQL Attribute used to ignore variables by the database manager
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class SqlIgnoreAttribute : Attribute
    {
    }
}
