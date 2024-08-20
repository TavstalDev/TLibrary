using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;

namespace Tavstal.TLibrary.Extensions.Unturned
{
    public static class PlayerExtensions
    {
        /// <summary>
        /// Checks if the Steam user associated with the specified Steam ID is currently online.
        /// </summary>
        /// <param name="id">The Steam ID of the user to check.</param>
        /// <returns>True if the user is online, false otherwise.</returns>
        public static bool isOnline(this CSteamID id)
        {
            return Provider.clients.Any(x => x.playerID.steamID == id);
        }

        /// <summary>
        /// Checks if the Unturned player is currently online.
        /// </summary>
        /// <param name="player">The Unturned player to check.</param>
        /// <returns>True if the player is online, false otherwise.</returns>
        public static bool isOnline(this UnturnedPlayer player)
        {
            return isOnline(player.CSteamID);
        }

        /// <summary>
        /// Gets a list of Unturned players who are nearby a given position within a specified distance.
        /// </summary>
        /// <param name="uplayer">The position of the player to check for nearby players.</param>
        /// <param name="distance">The maximum distance within which players are considered nearby.</param>
        /// <returns>A list of Unturned players who are nearby the given position.</returns>
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
        /// Searches for inventory items of the specified type in the player's inventory.
        /// </summary>
        /// <param name="inventory">The PlayerInventory to search.</param>
        /// <param name="type">The EItemType representing the type of items to search for.</param>
        /// <param name="findEmpty">Flag indicating whether to include empty amount in the search. (Default is false)</param>
        /// <returns>A List of InventorySearch containing the search results.</returns>
        public static List<InventorySearch> Search(this PlayerInventory inventory, EItemType type, bool findEmpty = false) => UPlayerHelper.Search(inventory, type, findEmpty);

        /// <summary>
        /// Searches for inventory items with the specified item ID in the player's inventory.
        /// </summary>
        /// <param name="inventory">The PlayerInventory to search.</param>
        /// <param name="itemID">The item ID to search for.</param>
        /// <param name="findEmpty">Flag indicating whether to include empty amount in the search. (Default is false)</param>
        /// <returns>A List of InventorySearch containing the search results.</returns>
        public static List<InventorySearch> Search(this PlayerInventory inventory, ushort itemID, bool findEmpty = false) => UPlayerHelper.Search(inventory, itemID, findEmpty);

        /// <summary>
        /// Forces the player to teleport to the specified location within a given radius.
        /// </summary>
        /// <param name="player">The player to teleport.</param>
        /// <param name="location">The destination location for the teleportation.</param>
        /// <param name="radious">The maximum distance from the destination location where the player can teleport. (Default is 3.0f)</param>
        public static void ForceTeleport(this Player player, Vector3 location, float radious = 3f) => UPlayerHelper.ForceTeleport(player, location, radious);

        /// <summary>
        /// Destroys all barricades belonging to the specified player.
        /// </summary>
        /// <param name="player">The player whose barricades will be destroyed.</param>
        public static void DestroyPlayerBarricades(this UnturnedPlayer player) => UPlayerHelper.DestroyPlayerBarricades(player);
    }
}
