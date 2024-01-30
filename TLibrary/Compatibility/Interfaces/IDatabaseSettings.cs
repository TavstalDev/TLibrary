namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Base database settings interface used in plugin configurations
    /// </summary>
    public interface IDatabaseSettings
    {
        /// <summary>
        /// Address of the server
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// Port of the server
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// Name of the database
        /// </summary>
        string DatabaseName { get; set; }
        /// <summary>
        /// Username of the database account
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// Password of the database account
        /// </summary>
        string UserPassword { get; set; }
        /// <summary>
        /// Timeout of the connection in seconds
        /// <br/>how long the connection should wait for a response from the server before disconnecting
        /// </summary>
        int TimeOut { get; set; }
    }
}
