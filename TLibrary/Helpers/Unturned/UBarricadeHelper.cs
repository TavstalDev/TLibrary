using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    public static class UBarricadeHelper
    {
        /// <summary>
        /// Retrieves a list of Interactable2SalvageBarricade objects that are within the specified radius from the given center position.
        /// </summary>
        /// <param name="center">The center position of the radius.</param>
        /// <param name="sqrRadius">The squared radius within which to find barricades.</param>
        /// <returns>A list of <see cref="Interactable2SalvageBarricade"></see> objects that are within the specified radius from the center position.</returns>
        public static List<Interactable2SalvageBarricade> GetBarricadesInRadius(Vector3 center, float sqrRadius)
        {
            List<Interactable2SalvageBarricade> result = new List<Interactable2SalvageBarricade>();
            RaycastHit[] rayResult = new RaycastHit[] { };
            Physics.SphereCastNonAlloc(center, sqrRadius, Vector3.forward, rayResult, RayMasks.BARRICADE);
            foreach (RaycastHit ray in rayResult)
            {
                var barricadeDrop = BarricadeManager.FindBarricadeByRootTransform(ray.transform);
                if (barricadeDrop != null)
                    result.Add(ray.transform.GetComponent<Interactable2SalvageBarricade>());
            }

            return result;
        }

        /// <summary>
        /// Retrieves the BarricadeDrop component attached to the Interactable2SalvageBarricade.
        /// </summary>
        /// <param name="salvageBarricade">The Interactable2SalvageBarricade to retrieve the BarricadeDrop component from.</param>
        /// <returns>The <see cref="BarricadeDrop"></see> component attached to the Interactable2SalvageBarricade.</returns>
        public static BarricadeDrop GetBarricadeDrop(this Interactable2SalvageBarricade salvageBarricade)
        {
            return BarricadeManager.FindBarricadeByRootTransform(salvageBarricade.transform);
        }
    }
}
