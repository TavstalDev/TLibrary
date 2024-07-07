using Rocket.API.Serialisation;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    public static class UPlayerHelper
    {
        /// <summary>
        /// Refills the magazine of the specified player with the given magazine ID and maximum amount to refill.<br/>
        /// If the magazine ID is not provided, every magazine will be refilled.<br/>
        /// If the maximum amount is not provided, it will be assumed as -1 (unlimited).
        /// </summary>
        /// <param name="player">The UnturnedPlayer to refill the magazine for.</param>
        /// <param name="magazineId">The ID of the magazine to refill. (Default is 0)</param>
        /// <param name="maxAmount">The maximum amount of magazine to refill. (Default is -1 for unlimited)</param>
        /// <returns>Returns true if the magazine is successfully refilled, otherwise false.</returns>
        public static bool RefillMagazine(UnturnedPlayer player, ushort magazineId = 0, int maxAmount = -1)
        {
            bool success = false;

            int maxlocal = maxAmount > 0 ? maxAmount : int.MaxValue;
            try
            {
                PlayerInventory inventory = player.Inventory;
                List<InventorySearch> searchResult = inventory.Search(EItemType.MAGAZINE, true);

                foreach (InventorySearch search in searchResult)
                {
                    ItemMagazineAsset asset = (ItemMagazineAsset)Assets.find(EAssetType.ITEM, search.jar.item.id);
                    if (magazineId > 0)
                        if (asset.id != magazineId)
                            continue;
                    if (asset.countMax > search.jar.item.amount || search.jar.item.amount == 0)
                    {
                        success = true;
                        inventory.sendUpdateAmount(search.page, search.jar.x, search.jar.y, asset.countMax > 0 ? asset.countMax : asset.count);
                        maxlocal--;
                        if (maxlocal <= 0)
                            break;
                    }
                }

                if (!success || maxlocal > 0)
                {
                    searchResult = inventory.Search(EItemType.GUN, true);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Error in RefillMagazine(): " + ex);
            }

            return success;
        }

        /// <summary>
        /// Searches for inventory items of the specified type in the player's inventory.
        /// </summary>
        /// <param name="inventory">The PlayerInventory to search.</param>
        /// <param name="type">The EItemType representing the type of items to search for.</param>
        /// <param name="findEmpty">Flag indicating whether to include empty amount in the search. (Default is false)</param>
        /// <returns>A List of InventorySearch containing the search results.</returns>
        public static List<InventorySearch> Search(PlayerInventory inventory, EItemType type, bool findEmpty = false)
        {
            List<InventorySearch> search = new List<InventorySearch>();
            try
            {
                var searchItems = inventory.items;
                foreach (Items _items in searchItems)
                {
                    if (_items == null)
                        continue;

                    if (_items.items == null)
                        continue;

                    var items = _items.items;
                    for (byte b = 0; b < items.Count; b = (byte)(b + 1))
                    {
                        if (!items.IsValidIndex(b))
                            continue;
                        ItemJar itemJar = items[b];
                        if (itemJar.item.amount > 0 || findEmpty)
                        {
                            ItemAsset itemAsset = Assets.find(EAssetType.ITEM, itemJar.item.id) as ItemAsset;
                            if (itemAsset != null && itemAsset.type == type)
                            {
                                search.Add(new InventorySearch(_items.page, itemJar));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Error in Search(): " + ex);
            }
            return search;
        }

        /// <summary>
        /// Searches for inventory items with the specified item ID in the player's inventory.
        /// </summary>
        /// <param name="inventory">The PlayerInventory to search.</param>
        /// <param name="itemID">The item ID to search for.</param>
        /// <param name="findEmpty">Flag indicating whether to include empty amount in the search. (Default is false)</param>
        /// <returns>A List of InventorySearch containing the search results.</returns>
        public static List<InventorySearch> Search(PlayerInventory inventory, ushort itemID, bool findEmpty = false)
        {
            List<InventorySearch> search = new List<InventorySearch>();
            try
            {
                var searchItems = inventory.items;
                foreach (Items _items in searchItems)
                {
                    if (_items == null)
                        continue;

                    if (_items.items == null)
                        continue;

                    var items = _items.items.FindAll(x => x.item.id == itemID);
                    for (byte b = 0; b < items.Count; b = (byte)(b + 1))
                    {
                        if (!items.IsValidIndex(b))
                            continue;
                        ItemJar itemJar = items[b];
                        if (itemJar.item.amount > 0 || findEmpty)
                            search.Add(new InventorySearch(_items.page, itemJar));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Error in Search(): " + ex);
            }
            return search;
        }

        /// <summary>
        /// Forces the player to teleport to the specified location within a given radius.
        /// </summary>
        /// <param name="player">The player to teleport.</param>
        /// <param name="location">The destination location for the teleportation.</param>
        /// <param name="radious">The maximum distance from the destination location where the player can teleport. (Default is 3.0f)</param>
        public static void ForceTeleport(Player player, Vector3 location, float radious = 3f)
        {
            if (!player.teleportToLocation(location + UnityEngine.Random.insideUnitSphere * radious, player.transform.rotation.y))
                ForceTeleport(player, location, radious);
        }

        /// <summary>
        /// Destroys all barricades belonging to the specified player.
        /// </summary>
        /// <param name="player">The player whose barricades will be destroyed.</param>
        public static void DestroyPlayerBarricades(UnturnedPlayer player)
        {
            foreach (var region in BarricadeManager.regions)
            {
                var drops = region.drops.FindAll(x => UnturnedPlayer.FromCSteamID((CSteamID)x.GetServersideData().owner) != null);
                foreach (var drop in drops)
                {
                    BarricadeManager.tryGetRegion(drop.model, out var x, out var y, out var plant, out var _);
                    BarricadeManager.destroyBarricade(drop, x, y, plant);
                }
            }
        }

        /// <summary>
        /// Clears the entire inventory of the specified player.
        /// </summary>
        /// <param name="player">The player whose inventory will be cleared.</param>
        public static void ClearInvventory(UnturnedPlayer player)
        {
            PlayerInventory playerInv = player.Inventory;

            if (player.Player.equipment.itemID != 0)
                player.Player.equipment.dequip();

            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                if (page == PlayerInventory.AREA)
                    continue;

                var count = playerInv.getItemCount(page);

                for (byte index = 0; index < count; index++)
                    playerInv.removeItem(page, 0);
            }

            player.Player.clothing.askWearBackpack(0, 0, new byte[0], true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearGlasses(0, 0, new byte[0], true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearHat(0, 0, new byte[0], true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearPants(0, 0, new byte[0], true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearMask(0, 0, new byte[0], true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearShirt(0, 0, new byte[0], true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearVest(0, 0, new byte[0], true);
            RemoveUnequipped(playerInv);
        }

        /// <summary>
        /// Removes all unequipped items from the player's inventory.
        /// </summary>
        /// <param name="inventory">The inventory to remove unequipped items from.</param>
        private static void RemoveUnequipped(PlayerInventory inventory)
        {
            for (byte i = 0; i < inventory.getItemCount(2); i++)
            {
                inventory.removeItem(2, 0);
            }
        }

        /// <summary>
        /// Gets a list of Unturned players who are nearby a given position within a specified distance.
        /// </summary>
        /// <param name="position">The position to check for nearby players.</param>
        /// <param name="distance">The maximum distance within which players are considered nearby.</param>
        /// <returns>A list of Unturned players who are nearby the given position.</returns>
        public static List<UnturnedPlayer> GetNearbyPlayers(Vector3 position, float distance)
        {
            List<UnturnedPlayer> players = new List<UnturnedPlayer>();

            foreach (var player in Provider.clients)
            {
                if (Vector3.Distance(player.player.transform.position, position) < distance)
                    players.Add(UnturnedPlayer.FromSteamPlayer(player));
            }

            return players;
        }

        public static List<RocketPermissionsGroup> GetMutualGroups(UnturnedPlayer player1, UnturnedPlayer player2)
        {
            List<RocketPermissionsGroup> p1Groups = Rocket.Core.R.Permissions.GetGroups(player1, true);
            List<RocketPermissionsGroup> p2Groups = Rocket.Core.R.Permissions.GetGroups(player2, true);

            return p1Groups.Intersect(p2Groups).ToList();
        }

        internal static uint GenerateFrequency()
        {
            uint freq = 0;

            freq = Convert.ToUInt32(MathHelper.Next(300000, 900000));
            /*if (!InUseFrequencies.Contains(freq))
                InUseFrequencies.Add(freq);
            else
                freq = GenerateFrequency();*/

            return freq;
        }
    }
}
