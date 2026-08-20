namespace Tavstal.TLibrary.Models.Config
{
    public interface IRedisConfig
    {
        /// <summary>
        /// 
        /// </summary>
        bool Enable { get; set; }
        
        /// <summary>
        /// Address of the server
        /// </summary>
        string Host { get; set; }
        
        /// <summary>
        /// Port of the server
        /// </summary>
        int Port { get; set; }
        
        /// <summary>
        ///
        /// </summary>
        string Prefix { get; set; }
        
        /// <summary>
        /// Name of the user.
        /// </summary>
        string UserName { get; set; }
        
        /// <summary>
        /// Password of the user.
        /// </summary>
        string UserPassword { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        bool UseSsl { get; set; }
    }
}