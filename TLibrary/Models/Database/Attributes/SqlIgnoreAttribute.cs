using System;

namespace Tavstal.TLibrary.Models.Database.Attributes
{
    /// <summary>
    /// SQL Attribute used to ignore variables by the database manager
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SqlIgnoreAttribute : Attribute
    {
    }
}
