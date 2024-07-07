using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    public static class UAssetHelper
    {
        /// <summary>
        /// Retrieves a list of ItemAsset objects representing all the item assets in the game.
        /// </summary>
        /// <returns>A list of ItemAsset objects representing the item assets in the game.</returns>
        public static List<ItemAsset> GetItemAssets()
        {
            List<ItemAsset> values = new List<ItemAsset>();
            Assets.find(values);
            return values;
        }

        /// <summary>
        /// Retrieves a list of VehicleAsset objects representing all the vehicle assets in the game.
        /// </summary>
        /// <returns>A list of VehicleAsset objects representing the vehicle assets in the game.</returns>
        public static List<VehicleAsset> GetVehicleAssets()
        {
            List<VehicleAsset> values = new List<VehicleAsset>();
            Assets.find(values);
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
