using System;

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
