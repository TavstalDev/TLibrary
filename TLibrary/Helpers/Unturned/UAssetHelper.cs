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
    public static class UAssetHelper
    {
        /// <summary>
        /// Retrieves a list of ItemAsset objects representing all the item assets in the game.
        /// </summary>
        /// <returns>A list of ItemAsset objects representing the item assets in the game.</returns>
        public static List<ItemAsset> GetItemAssets()
        {
            Asset[] assets = null;
            List<ItemAsset> values = new List<ItemAsset>();

            assets = Assets.find(EAssetType.ITEM);

            foreach (Asset a in assets)
            {
                values.Add((ItemAsset)a);
            }
            return values;
        }

        /// <summary>
        /// Retrieves a list of VehicleAsset objects representing all the vehicle assets in the game.
        /// </summary>
        /// <returns>A list of VehicleAsset objects representing the vehicle assets in the game.</returns>
        public static List<VehicleAsset> GetVehicleAssets()
        {
            Asset[] assets = null;
            List<VehicleAsset> values = new List<VehicleAsset>();

            assets = Assets.find(EAssetType.VEHICLE);

            foreach (Asset a in assets)
            {
                values.Add((VehicleAsset)a);
            }
            return values;
        }

        /// <summary>
        /// Finds item asset by ingame dispaly name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ItemAsset FindItemAsset(string name)
        {
            ItemAsset asset = null;
            foreach (ItemAsset a in GetItemAssets())
            {
                if (a.itemName != null && a.itemName.Length > 0)
                {
                    if (a.itemName.ContainsIgnoreCase(name))
                    {
                        asset = a;
                        break;
                    }
                }
            }

            return asset;
        }

        /// <summary>
        /// Finds vehicle asset by ingame dispaly name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static VehicleAsset FindVehicleAsset(string name)
        {
            VehicleAsset asset = null;
            foreach (VehicleAsset a in GetVehicleAssets())
            {
                if (a.vehicleName != null && a.vehicleName.Length > 0)
                {
                    if (a.vehicleName.ContainsIgnoreCase(name))
                    {
                        asset = a;
                        break;
                    }
                }
            }

            return asset;
        }

        /// <summary>
        /// Finds and retrieves an ItemAsset object with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the item asset to find.</param>
        /// <returns>An ItemAsset object representing the item asset with the specified ID, or null if not found.</returns>
        public static ItemAsset FindItemAsset(ushort id) => (ItemAsset)Assets.find(EAssetType.ITEM, id);

        /// <summary>
        /// Finds and retrieves a VehicleAsset object with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the vehicle asset to find.</param>
        /// <returns>A VehicleAsset object representing the vehicle asset with the specified ID, or null if not found.</returns>
        public static VehicleAsset FindVehicleAsset(ushort id) => (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);
    }
}
