namespace Tavstal.TLibrary.Models
{
    /// <summary>
    /// Represents the kind of lock mechanism applied to a resource or operation.
    /// </summary>
    public enum ELockKind
    {
        /// <summary>
        /// The lock is managed via command-line / programmatic access.
        /// </summary>
        COMMAND = 0,

        /// <summary>
        /// The lock is managed via a user-interface control.
        /// </summary>
        UI = 1
    }
}