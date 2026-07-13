namespace Tavstal.TLibrary.Models.Plugin
{
    /// <summary>
    /// Defines a component that can be attached to a player, providing page index tracking.
    /// </summary>
    public interface IPlayerComponent
    {
        /// <summary>
        /// Gets or sets the array of page index arrays used for pagination.
        /// </summary>
        int[][] PageIndexes { get; set; }
    }
}
