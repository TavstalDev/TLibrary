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
    public static class UBarricadeHelper
    {
        public static List<Interactable2SalvageBarricade> GetBarricadesInRadius(Vector3 center, float sqrRadius)
        {
            List<Interactable2SalvageBarricade> result = new List<Interactable2SalvageBarricade>();
            var rayResult = Physics.SphereCastAll(center, sqrRadius, Vector3.forward, RayMasks.BARRICADE);
            foreach (RaycastHit ray in rayResult)
            {
                var barricadeDrop = BarricadeManager.FindBarricadeByRootTransform(ray.transform);
                if (barricadeDrop != null)
                    result.Add(ray.transform.GetComponent<Interactable2SalvageBarricade>());
            }

            return result;
        }

        public static BarricadeDrop GetBarricadeDrop(this Interactable2SalvageBarricade salvageBarricade)
        {
            return BarricadeManager.FindBarricadeByRootTransform(salvageBarricade.transform);
        }
    }
}
