namespace Tavstal.TLibrary.Models.FormatHelper
{
    /// <summary>
    /// Format for unturned rich chat output.
    /// </summary>
    public class TextFormat
    {
        public string Key { get; set; }
        public string StartTag { get; set; }
        public string EndTag { get; set; }
        public bool IsDecoration { get; set; }

        public TextFormat() { }

        public TextFormat(string key, string start, string end, bool isdeco)
        {
            Key = key;
            StartTag = start;
            EndTag = end;
            IsDecoration = isdeco;
        }
    }
}
