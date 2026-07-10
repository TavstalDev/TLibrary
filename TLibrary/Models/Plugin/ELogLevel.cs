namespace Tavstal.TLibrary.Models.Plugin
{
    /// <summary>
    /// Defines the severity levels for log messages.
    /// </summary>
    public enum ELogLevel
    {
        /// <summary>
        /// Verbose diagnostic information, typically used during development and troubleshooting.
        /// </summary>
        DEBUG = 0,

        /// <summary>
        /// General information about the normal operation of the application.
        /// </summary>
        INFO = 1,

        /// <summary>
        /// Indication of a potential issue that does not prevent the application from running.
        /// </summary>
        WARNING = 2,

        /// <summary>
        /// A failure that affects the current operation but does not require the application to stop.
        /// </summary>
        ERROR = 3,

        /// <summary>
        /// A message related to a command executed by a player or the server console.
        /// </summary>
        COMMAND = 4,
    }
}
