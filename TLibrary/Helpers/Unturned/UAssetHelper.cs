using SDG.Unturned;
using System;
using System.Collections.Generic;
using Tavstal.TLibrary.Extensions.General;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    /// <summary>
    /// Provides helper methods to find and get Unturned assets like items, vehicles, and effects.
    /// </summary>
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
            List<Asset> values = new List<Asset>();
            List<VehicleAsset> vehicles = new List<VehicleAsset>();
            Assets.find(values);
            values = values.FindAll(x => x.assetCategory == EAssetType.VEHICLE);
            
            foreach (Asset asset in values)
            {
                switch (asset)
                {
                    case VehicleAsset va:
                        vehicles.Add(va);
                        break;
                    case VehicleRedirectorAsset vra:
                        vehicles.Add(vra.TargetVehicle.Find());
                        break;
                }
            }
            return vehicles;
        }

        /// <summary>
        /// Finds item asset by ingame dispaly name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ItemAsset? FindItemAsset(string name)
        {
            ItemAsset? asset = null;
            foreach (ItemAsset itemAsset in GetItemAssets())
            {
                if (!string.IsNullOrEmpty(itemAsset.itemName))
                {
                    if (itemAsset.itemName.ContainsIgnoreCase(name))
                    {
                        asset = itemAsset;
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
        public static VehicleAsset? FindVehicleAsset(string name)
        {
            VehicleAsset? asset = null;
            foreach (VehicleAsset vehicleAsset in GetVehicleAssets())
            {
                if (!string.IsNullOrEmpty(vehicleAsset.vehicleName))
                {
                    if (vehicleAsset.vehicleName.ContainsIgnoreCase(name))
                    {
                        asset = vehicleAsset;
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
        public static ItemAsset? FindItemAsset(ushort id) => (ItemAsset)Assets.find(EAssetType.ITEM, id);

        /// <summary>
        /// Finds and retrieves a VehicleAsset object with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the vehicle asset to find.</param>
        /// <returns>A VehicleAsset object representing the vehicle asset with the specified ID, or null if not found.</returns>
        public static VehicleAsset? FindVehicleAsset(ushort id) {
            Asset asset = Assets.FindBaseVehicleAssetByGuidOrLegacyId(Guid.Empty, id) ?? Assets.FindVehicleAssetByGuidOrLegacyId(Guid.Empty, id);
            switch (asset)
            {
                case null:
                    return null;
                case VehicleAsset vehicleAsset:
                    return vehicleAsset;
                case VehicleRedirectorAsset vehicleRedirectorAsset:
                    return vehicleRedirectorAsset.TargetVehicle.Find();
                default:
                    return null;
            }
        }
        
        public static EffectAsset? FindEffectAsset(ushort id) => (EffectAsset)Assets.find(EAssetType.EFFECT, id);
    }
}
