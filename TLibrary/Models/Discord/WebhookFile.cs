namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents a file that can be sent through a Discord webhook.
    /// </summary>
    public struct WebhookFile
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The format or extension of the file (e.g. "png", "txt").
        /// </summary>
        public string Format { get; set; }
        
        /// <summary>
        /// The raw byte data of the file content.
        /// </summary>
        public byte[] Content { get; set; }
    }
}