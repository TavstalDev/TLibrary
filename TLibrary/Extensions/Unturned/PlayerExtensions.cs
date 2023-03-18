using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers;
using static SDG.Provider.SteamGetInventoryResponse;
using UnityEngine;

namespace Tavstal.TLibrary.Extensions
{
    public static class PlayerExtensions
    {
        public static bool isOnline(this CSteamID id)
        {
            return Provider.clients.Any(x => x.playerID.steamID == id);
        }

        public static bool isOnline(this UnturnedPlayer player)
        {
            return Provider.clients.Any(x => x.playerID.steamID == player.CSteamID);
        }

        public static List<InventorySearch> Search(this PlayerInventory inventory, EItemType type, bool findEmpty = false) => UPlayerHelper.Search(inventory, type, findEmpty);

        public static List<InventorySearch> Search(this PlayerInventory inventory, ushort itemID, bool findEmpty = false) => UPlayerHelper.Search(inventory, itemID, findEmpty);

        public static void ForceTeleport(this Player player, Vector3 location, float radious = 3f) => UPlayerHelper.ForceTeleport(player, location);

        public static void DestroyPlayerBarricades(this UnturnedPlayer player) => UPlayerHelper.DestroyPlayerBarricades(player);
    }
}
