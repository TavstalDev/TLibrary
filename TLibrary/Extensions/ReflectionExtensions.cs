using System;
using System.Reflection;

namespace Tavstal.TLibrary.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Retrieves the <see cref="MethodInfo"/> object for a method with the specified name, binding flags, and parameter types from the given type.
        /// </summary>
        /// <param name="self">The type from which to retrieve the method information.</param>
        /// <param name="name">The name of the method to retrieve.</param>
        /// <param name="flags">The binding flags that control the search behavior (e.g., public, non-public, static, instance).</param>
        /// <param name="types">An array of <see cref="Type"/> objects representing the parameter types of the method.</param>
        /// <returns>The <see cref="MethodInfo"/> object representing the method that matches the specified criteria, or null if no match is found.</returns>
        public static MethodInfo GetMethod(this Type self, string name, BindingFlags flags, Type[] types)
        {
            return self.GetMethod(name, flags, null, types, null);
        }
    }
}
