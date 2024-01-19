using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    public static class UStructureHelper
    {
        /// <summary>
        /// Retrieves a list of Interactable2SalvageStructure objects within a specified radius from the given center point.
        /// </summary>
        /// <param name="center">The center point of the search radius.</param>
        /// <param name="sqrRadius">The squared radius within which to search for structures.</param>
        /// <returns>A list of <see cref="Interactable2SalvageStructure"></see> objects within the specified radius.</returns>
        public static List<Interactable2SalvageStructure> GetStructuresInRadius(Vector3 center, float sqrRadius)
        {
            List<Interactable2SalvageStructure> result = new List<Interactable2SalvageStructure>();
            var rayResult = Physics.SphereCastAll(center, sqrRadius, Vector3.forward, RayMasks.BARRICADE);
            foreach (RaycastHit ray in rayResult)
            {
                var barricadeDrop = StructureManager.FindStructureByRootTransform(ray.transform);
                if (barricadeDrop != null)
                    result.Add(ray.transform.GetComponent<Interactable2SalvageStructure>());
            }

            return result;
        }

        /// <summary>
        /// Retrieves the StructureDrop associated with the given Interactable2SalvageStructure.
        /// </summary>
        /// <param name="salvageStructure">The Interactable2SalvageStructure for which to retrieve the StructureDrop.</param>
        /// <returns>The <see cref="StructureDrop"></see> associated with the given Interactable2SalvageStructure.</returns>
        public static StructureDrop GetStructureDrop(this Interactable2SalvageStructure salvageStructure)
        {
            return StructureManager.FindStructureByRootTransform(salvageStructure.transform);
        }
    }
}
