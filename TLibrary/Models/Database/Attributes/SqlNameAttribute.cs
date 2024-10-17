using System;

namespace Tavstal.TLibrary.Models.Database.Attributes
{
    /// <summary>
    /// SQL Attribute used to declare the name of a table 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SqlNameAttribute : Attribute
    {
        public string Name { get; set; }

        public SqlNameAttribute(string name)
        {
            Name = name;
        }
    }
}
