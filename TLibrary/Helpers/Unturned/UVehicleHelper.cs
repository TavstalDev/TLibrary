﻿using SDG.Unturned;
using System;
using System.Collections.Generic;
using Tavstal.TLibrary.Helpers.General;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    public static class UVehicleHelper
    {
        /// <summary>
        /// Retrieves a list of <see cref="InteractableVehicle"/> objects within a specified radius from the given center point.
        /// </summary>
        /// <param name="center">The center point from which to search for vehicles.</param>
        /// <param name="sqrRadius">The squared radius within which to search for vehicles.</param>
        /// <returns>A list of <see cref="InteractableVehicle"/> objects found within the specified radius.</returns>
        public static List<InteractableVehicle> GetVehiclesInRadius(Vector3 center, float sqrRadius)
        {
            List<InteractableVehicle> result = new List<InteractableVehicle>();
            try
            {
                if (VehicleManager.vehicles == null) return result;
                RaycastHit[] rayResult = new RaycastHit[] { };
                Physics.SphereCastNonAlloc(center, sqrRadius, Vector3.forward, rayResult, RayMasks.VEHICLE);
                foreach (RaycastHit ray in rayResult)
                {
                    var vehicle = ray.transform.GetComponent<InteractableVehicle>();
                    if (vehicle != null)
                        result.Add(vehicle);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Error in getVehiclesInRadius():" + ex);
            }
            return result;
        }
    }
}
