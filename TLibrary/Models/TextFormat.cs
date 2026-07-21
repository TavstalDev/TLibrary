namespace Tavstal.TLibrary.Models
{
    /// <summary>
    /// Format for unturned rich chat output.
    /// </summary>
    public class TextFormat
    {
        /// <summary>
        /// The key that identifies this format.
        /// </summary>
        public string? Key { get; set; }
        /// <summary>
        /// The opening tag of the format.
        /// </summary>
        public string StartTag { get; set; }
        /// <summary>
        /// The closing tag of the format.
        /// </summary>
        public string EndTag { get; set; }
        /// <summary>
        /// If <see langword="true"/>, this format is a decoration and not a color.
        /// </summary>
        public bool IsDecoration { get; set; }
        
        /// <summary>
        /// Creates a new text format.
        /// </summary>
        /// <param name="key">The key that identifies this format.</param>
        /// <param name="start">The opening tag.</param>
        /// <param name="end">The closing tag.</param>
        /// <param name="isdeco">If <see langword="true"/>, this format is a decoration.</param>
        public TextFormat(string? key, string start, string end, bool isdeco)
        {
            Key = key;
            StartTag = start;
            EndTag = end;
            IsDecoration = isdeco;
        }
    }
}
