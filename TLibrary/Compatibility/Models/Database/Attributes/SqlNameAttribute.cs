using System;

namespace Tavstal.TLibrary.Compatibility.Database
{
    /// <summary>
    /// SQL Attribute used to declare the name of a table 
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SqlNameAttribute : Attribute
    {
        public string Name { get; set; }

        public SqlNameAttribute(string name)
        {
            Name = name;
        }
    }
}
