namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents a single part of a multipart form data request.
    /// </summary>
    public struct MultipartChunk
    {
        /// <summary>
        /// The header bytes of the chunk (e.g. content-disposition and content-type).
        /// </summary>
        public byte[] Header { get; set; }
        
        /// <summary>
        /// The separate payload data of the chunk (e.g. file bytes).
        /// </summary>
        public byte[] Payload { get; set; }
        
        /// <summary>
        /// If <see langword="true"/>, the chunk has a separate payload that must be written after the header.
        /// </summary>
        public bool HasSeparatePayload { get; set; }
    }
}