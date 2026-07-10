namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Holds a JSON string and its content type for use in multipart form data.
    /// </summary>
    public struct JsonParameter
    {
        /// <summary>
        /// The serialized JSON content.
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// The MIME content type (e.g. "application/json").
        /// </summary>
        public string ContentType { get; set; }
    }
}
