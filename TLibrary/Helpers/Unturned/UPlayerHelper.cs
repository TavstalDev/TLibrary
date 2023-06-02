using RestSharp.Extensions;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Extensions;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    public static class UPlayerHelper
    {
        public static bool RefillMagazine(UnturnedPlayer player, int max = -1)
        {
            bool success = false;

            int maxlocal = max > 0 ? max : int.MaxValue;
            try
            {
                PlayerInventory inventory = player.Inventory;
                List<InventorySearch> searchResult = inventory.Search(EItemType.MAGAZINE, true);

                foreach (InventorySearch search in searchResult)
                {
                    ItemMagazineAsset asset = (ItemMagazineAsset)Assets.find(EAssetType.ITEM, search.jar.item.id);
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

        public static void ForceTeleport(Player player, Vector3 location, float radious = 3f)
        {
            if (!player.teleportToLocation(location + UnityEngine.Random.insideUnitSphere * radious, player.transform.rotation.y))
                ForceTeleport(player, location, radious);
        }

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

        public static void ClearInvventory(UnturnedPlayer player)
        {
            PlayerInventory playerInv = player.Inventory;

            if (player.Player.equipment.isEquipped)
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

        private static void RemoveUnequipped(PlayerInventory inventory)
        {
            for (byte i = 0; i < inventory.getItemCount(2); i++)
            {
                inventory.removeItem(2, 0);
            }
        }

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
    }
}
