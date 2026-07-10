using System;
using System.Reflection;

namespace Tavstal.TLibrary.Extensions.General
{
    /// <summary>
    /// Provides extension methods for reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Finds a method by name, binding flags, and parameter types from a type.
        /// </summary>
        /// <param name="self">The type to search in.</param>
        /// <param name="name">The name of the method to find.</param>
        /// <param name="flags">Settings that control the search (like public, private, static).</param>
        /// <param name="types">The parameter types the method expects.</param>
        /// <returns>The method info if found, or null if not.</returns>
        public static MethodInfo? GetMethod(this Type self, string name, BindingFlags flags, Type[] types) =>
            self.GetMethod(name, flags, null, types, null);
    }
}
