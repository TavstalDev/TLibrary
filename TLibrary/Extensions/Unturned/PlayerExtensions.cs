using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;

namespace Tavstal.TLibrary.Extensions.Unturned
{
    /// <summary>
    /// Provides extension methods for players.
    /// </summary>
    public static class PlayerExtensions
    {
        /// <summary>
        /// Checks if the user with this Steam ID is currently online.
        /// </summary>
        /// <param name="id">The Steam ID to check.</param>
        /// <returns>True if the user is online, false if not.</returns>
        public static bool isOnline(this CSteamID id)
        {
            return Provider.clients.Any(x => x.playerID.steamID == id);
        }

        /// <summary>
        /// Checks if the player is currently online.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>True if the player is online, false if not.</returns>
        public static bool isOnline(this UnturnedPlayer player)
        {
            return isOnline(player.CSteamID);
        }

        /// <summary>
        /// Finds all players that are close to this player within a given distance.
        /// </summary>
        /// <param name="uplayer">The player to check around.</param>
        /// <param name="distance">How far to look for other players.</param>
        /// <returns>A list of nearby players.</returns>
        public static List<UnturnedPlayer> GetNearbyPlayers(this UnturnedPlayer uplayer, float distance)
        {
            List<UnturnedPlayer> players = new List<UnturnedPlayer>();

            foreach (var player in Provider.clients)
            {
                if (Vector3.Distance(player.player.transform.position, uplayer.Position) < distance)
                    players.Add(UnturnedPlayer.FromSteamPlayer(player));
            }

            return players;
        }

        /// <summary>
        /// Looks for items of the given type in the player's inventory.
        /// </summary>
        /// <param name="inventory">The inventory to search.</param>
        /// <param name="type">The item type to look for.</param>
        /// <param name="findEmpty">If true, also includes items with empty amount. Default is false.</param>
        /// <returns>A list of search results.</returns>
        public static List<InventorySearch> Search(this PlayerInventory inventory, EItemType type, bool findEmpty = false) => UPlayerHelper.Search(inventory, type, findEmpty);

        /// <summary>
        /// Looks for items with the given ID in the player's inventory.
        /// </summary>
        /// <param name="inventory">The inventory to search.</param>
        /// <param name="itemID">The item ID to look for.</param>
        /// <param name="findEmpty">If true, also includes items with empty amount. Default is false.</param>
        /// <returns>A list of search results.</returns>
        public static List<InventorySearch> Search(this PlayerInventory inventory, ushort itemID, bool findEmpty = false) => UPlayerHelper.Search(inventory, itemID, findEmpty);

        /// <summary>
        /// Teleports the player to a location within a given radius.
        /// </summary>
        /// <param name="player">The player to teleport.</param>
        /// <param name="location">Where to teleport the player.</param>
        /// <param name="radious">The maximum distance from the target location. Default is 3.</param>
        public static void ForceTeleport(this Player player, Vector3 location, float radious = 3f) => UPlayerHelper.ForceTeleport(player, location, radious);

        /// <summary>
        /// Destroys all barricades owned by the player.
        /// </summary>
        /// <param name="player">The player whose barricades to destroy.</param>
        public static void DestroyPlayerBarricades(this UnturnedPlayer player) => UPlayerHelper.DestroyPlayerBarricades(player);
    }
}
