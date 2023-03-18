using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers;

namespace Tavstal.TLibrary.Extensions
{
    public static class IntegerExtensions
    {
        public static int Clamp(this int value, int minValue, int maxValue)
        {
            return MathHelper.Clamp(value, minValue, maxValue);
        }
    }
}
