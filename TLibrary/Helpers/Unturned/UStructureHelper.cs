using RestSharp.Extensions;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    public static class UStructureHelper
    {
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

        public static StructureDrop GetStructureDrop(this Interactable2SalvageStructure salvageStructure)
        {
            return StructureManager.FindStructureByRootTransform(salvageStructure.transform);
        }
    }
}
