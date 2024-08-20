using System;

namespace Tavstal.TLibrary.Models.Database.Attributes
{
    /// <summary>
    /// SQL Attribute used to declare the name of a table 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class SqlNameAttribute : Attribute
    {
        public string Name { get; set; }

        public SqlNameAttribute(string name)
        {
            Name = name;
        }
    }
}
