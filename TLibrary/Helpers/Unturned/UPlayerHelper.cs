using Rocket.API.Serialisation;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Extensions.Unturned;
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
                            ItemAsset itemAsset = UAssetHelper.FindItemAsset(itemJar.item.id);
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
            while (true)
            {
                if (!player.teleportToLocation(location + UnityEngine.Random.insideUnitSphere * radious, player.transform.rotation.y)) 
                    continue;
                break;
            }
        }

        /// <summary>
        /// Destroys all barricades belonging to the specified player.
        /// </summary>
        /// <param name="player">The player whose barricades will be destroyed.</param>
        public static void DestroyPlayerBarricades(UnturnedPlayer player)
        {
            if (player == null)
                return;

            if (player.CSteamID == CSteamID.Nil)
                return;

            foreach (var region in BarricadeManager.regions)
            {
                foreach (var drop in region.drops)
                {
                    if (drop.GetServersideData().owner == player.CSteamID.m_SteamID)
                        continue;
                    
                    BarricadeManager.tryGetRegion(drop.model, out var x, out var y, out var plant, out var _);
                    BarricadeManager.destroyBarricade(drop, x, y, plant);
                }
            }
        }

        /// <summary>
        /// Retrieves the inventory of a specified player, optionally including equipped items.
        /// </summary>
        /// <param name="target">The player whose inventory is to be retrieved.</param>
        /// <param name="includeEquipment">If true, the method will include the player's equipped items in the inventory list; otherwise, it will exclude them.</param>
        /// <returns>A list of <see cref="Item"/> objects representing the player's inventory.</returns>
        public static List<Item> GetInventory(UnturnedPlayer target, bool includeEquipment)
        {
            PlayerClothing clothing = target.Player.clothing;

            List<Item> items = new List<Item>();

            items.AddRange(SaveItemsFromPage(target, 0)); // Primary slot
            items.AddRange(SaveItemsFromPage(target, 1)); // Secondary slot
            items.AddRange(SaveItemsFromPage(target, 2)); // Hands

            if (clothing.backpack != 0)
            {
                if (includeEquipment)
                    items.Add(new Item(clothing.backpack, 1, clothing.backpackQuality, clothing.backpackState));
                items.AddRange(SaveItemsFromPage(target, 3));
            }
            if (clothing.shirt != 0)
            {
                if (includeEquipment)
                    items.Add(new Item(clothing.shirt, 1, clothing.shirtQuality, clothing.shirtState));
                items.AddRange(SaveItemsFromPage(target, 5));
            }
            if (clothing.vest != 0)
            {
                if (includeEquipment)
                    items.Add(new Item(clothing.vest, 1, clothing.vestQuality, clothing.vestState));
                items.AddRange(SaveItemsFromPage(target, 4));
            }
            if (clothing.pants != 0)
            {
                if (includeEquipment)
                    items.Add(new Item(clothing.pants, 1, clothing.pantsQuality, clothing.pantsState));
                items.AddRange(SaveItemsFromPage(target, 6));
            }
            if (clothing.mask != 0 && includeEquipment)
                items.Add(new Item(clothing.mask, 1, clothing.maskQuality, clothing.maskState));
            if (clothing.hat != 0 && includeEquipment)
                items.Add(new Item(clothing.hat, 1, clothing.hatQuality, clothing.hatState));
            if (clothing.glasses != 0 && includeEquipment)
                items.Add(new Item(clothing.glasses, 1, clothing.glassesQuality, clothing.glassesState));

            return items;
        }

        /// <summary>
        /// Saves the items from a specific inventory page of the given player.
        /// </summary>
        /// <param name="player">The player from whom the items are to be saved.</param>
        /// <param name="page">The inventory page number from which the items should be saved.</param>
        /// <returns>A list of <see cref="Item"/> objects representing the items saved from the specified page.</returns>
        private static List<Item> SaveItemsFromPage(UnturnedPlayer player, byte page)
        {
            PlayerInventory inventory = player.Player.inventory;
            var count = inventory.getItemCount(page);
            List<Item> items = new List<Item>();

            for (byte index = 0; index < count; index++)
            {
                var item = inventory.getItem(page, index).item;

                if (item == null)
                    continue;

                ItemAsset asset = Assets.find(EAssetType.ITEM, item.id) as ItemAsset;

                items.Add(asset?.type == EItemType.MAGAZINE
                    ? new Item(item.id, item.amount, item.quality, item.state)
                    : new Item(item.id, 1, item.quality, item.state));
            }
            return items;
        }

        /// <summary>
        /// Removes a specified amount of an item from the target player's inventory.
        /// </summary>
        /// <param name="targetPlayer">The player from whose inventory the item is to be removed.</param>
        /// <param name="itemID">The ID of the item to be removed.</param>
        /// <param name="amount">The quantity of the item to remove. Defaults to 1 if not specified.</param>
        public static void RemoveItemFromInventory(UnturnedPlayer targetPlayer, ushort itemID, int amount = 1)
        {
            PlayerInventory inventory = targetPlayer.Player.inventory;

            for (byte page = 0; page < 7; page++)
            {
                var count = inventory.getItemCount(page);

                for (byte index = 0; index < count; index++)
                {
                    var item = inventory.getItem(page, index).item;

                    if (item == null)
                        continue;

                    ItemAsset asset = Assets.find(EAssetType.ITEM, item.id) as ItemAsset;

                    if (item.id != itemID)
                        continue;

                    inventory.removeItem(page, index);

                    amount--;
                    if (amount == 0)
                        break;
                }
                if (amount == 0)
                    break;
            }
        }

        /// <summary>
        /// Clears the entire inventory of the specified player.
        /// </summary>
        /// <param name="player">The player whose inventory will be cleared.</param>
        public static void ClearInventory(UnturnedPlayer player)
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

            player.Player.clothing.askWearBackpack(0, 0, Array.Empty<byte>(), true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearGlasses(0, 0, Array.Empty<byte>(), true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearHat(0, 0, Array.Empty<byte>(), true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearPants(0, 0, Array.Empty<byte>(), true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearMask(0, 0, Array.Empty<byte>(), true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearShirt(0, 0, Array.Empty<byte>(), true);
            RemoveUnequipped(playerInv);

            player.Player.clothing.askWearVest(0, 0, Array.Empty<byte>(), true);
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

        /// <summary>
        /// Retrieves a list of mutual permission groups that both specified players belong to.
        /// </summary>
        /// <param name="player1">The first player for whom mutual groups are being checked.</param>
        /// <param name="player2">The second player for whom mutual groups are being checked.</param>
        /// <returns>A list of <see cref="RocketPermissionsGroup"/> objects representing the permission groups that both players have in common.</returns>
        public static List<RocketPermissionsGroup> GetMutualGroups(UnturnedPlayer player1, UnturnedPlayer player2)
        {
            List<RocketPermissionsGroup> p1Groups = Rocket.Core.R.Permissions.GetGroups(player1, true);
            List<RocketPermissionsGroup> p2Groups = Rocket.Core.R.Permissions.GetGroups(player2, true);

            return p1Groups.Intersect(p2Groups).ToList();
        }

        /// <summary>
        /// Generates a unique frequency value.
        /// </summary>
        /// <returns>A <see cref="uint"/> representing the generated frequency.</returns>
        public static uint GenerateFrequency()
        {
            return Convert.ToUInt32(MathHelper.Next(300000, 900000));
        }
    }
}
