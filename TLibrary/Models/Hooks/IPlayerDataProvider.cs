using System;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Models.Hooks
{
    /// <summary>
    /// Interface for providing (offline) player data.
    /// </summary>
    public interface IPlayerDataProvider
    {
        /// <summary>
        /// Checks if a player with the specified Steam ID exists.
        /// </summary>
        /// <param name="steamId">The Steam ID of the player to check.</param>
        /// <returns>True if a player with the given Steam ID exists; otherwise, false.</returns>
        Task<bool> IsExistsAsync(ulong steamId);
        /// <summary>
        /// Retrieves the Steam name associated with the provided Steam ID.
        /// </summary>
        /// <param name="steamId">The unique identifier for the Steam account.</param>
        /// <returns>The Steam name corresponding to the given Steam ID.</returns>
        Task<string> GetSteamNameAsync(ulong steamId);
        /// <summary>
        /// Retrieves the character name associated with the specified Steam ID.
        /// </summary>
        /// <param name="steamId">The unique identifier of the Steam account.</param>
        /// <returns>The name of the character.</returns>
        Task<string> GetCharacterNameAsync(ulong steamId);
        /// <summary>
        /// Retrieves the first login date for a player identified by their Steam ID.
        /// </summary>
        /// <param name="steamId">The unique Steam ID of the player.</param>
        /// <returns>The DateTime representing the first login date of the player.</returns>
        Task<DateTime> GetFirstLoginAsync(ulong steamId);
        /// <summary>
        /// Retrieves the date and time of the last login for a specific Steam ID.
        /// </summary>
        /// <param name="steamId">The unique identifier associated with the Steam account.</param>
        /// <returns>The date and time of the last login for the specified Steam ID.</returns>
        Task<DateTime> GetLastLoginAsync(ulong steamId);
    }
}