using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Economy;
using Tavstal.TLibrary.Helpers;

namespace Tavstal.TLibrary.Extensions
{
    public static class EnumExtensions
    {
        public static ECurrency ToCurrency(this EPaymentMethod type)
        {
            switch (type)
            {
                case EPaymentMethod.WALLET:
                    {
                        return ECurrency.CASH;
                    }
                case EPaymentMethod.BANK:
                    {
                        return ECurrency.BANK;
                    }
                case EPaymentMethod.CRYPTO:
                    {
                        return ECurrency.CRYPTO;
                    }
                default:
                    {
                        return ECurrency.CASH;
                    }
            }
        }
    }
}
