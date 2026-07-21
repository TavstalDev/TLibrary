namespace Tavstal.TLibrary.Models.Database
{
    /// <summary>
    /// Represents the result state of a database operation.
    /// </summary>
    public enum EDatabaseState
    {
        /// <summary>The operation completed successfully.</summary>
        SUCCESS = 0,

        /// <summary>Authentication with the database server failed.</summary>
        AUTHENTICATION_FAILED = 1,

        /// <summary>An error occurred during the operation.</summary>
        ERROR = 2,
    }
}