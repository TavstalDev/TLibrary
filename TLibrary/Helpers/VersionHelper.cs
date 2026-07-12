using System;
using System.Reflection;

namespace Tavstal.TLibrary.Helpers
{
    /// <summary>
    /// Extracts <see cref="Version"/> info from assemblies.
    /// </summary>
    public static class VersionHelper
    {
        /// <summary>
        /// Parses the informational version of an assembly, stripping build metadata (+), pre-release (-), and underscore (_) suffixes. Returns <c>0.0.0</c> on failure.
        /// </summary>
        public static Version GetVersion(Assembly assembly)
        {
            var infoAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (infoAttr == null)
                return new Version(0, 0, 0);
            
            string version = infoAttr.InformationalVersion;
            if (version.Contains("+"))
                version = version.Split('+')[0];
            
            if (version.Contains("-"))
                version = version.Split('-')[0];
            
            if (version.Contains("_"))
                version = version.Split('_')[0];
            
            return !Version.TryParse(version, out Version result) ? new Version(0, 0, 0) : result;
        }
    }
}