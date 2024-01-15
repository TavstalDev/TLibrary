using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    public enum EPaymentMethod
    {
        /// <summary>
        /// "Physical" money using unturned's experience system
        /// </summary>
        EXPERIENCE,
        /// <summary>
        /// Physical money in your inventory
        /// </summary>
        CASH,
        /// <summary>
        /// Digital money in your bank account
        /// </summary>
        BANK_ACCOUNT,
        /// <summary>
        /// Digital money in your crypto account
        /// </summary>
        CRYPTO_WALLET
    }
}
