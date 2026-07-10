namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents a file to be uploaded in a multipart form data request.
    /// </summary>
    public struct FileParameter
    {
        /// <summary>
        /// The raw byte data of the file.
        /// </summary>
        public byte[] File { get; set; }
        
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// The MIME content type of the file (e.g. "application/octet-stream").
        /// </summary>
        public string ContentType { get; set; }
    }
}
