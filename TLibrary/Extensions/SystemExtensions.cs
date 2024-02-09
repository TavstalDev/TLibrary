using System;
using System.Reflection;

namespace Tavstal.TLibrary.Extensions
{
    public static class SystemExtensions
    {
        public static MethodInfo GetMethod(this Type self, string name, BindingFlags flags, Type[] types)
        {
            return self.GetMethod(name, flags, null, types, null);
        }
    }
}
