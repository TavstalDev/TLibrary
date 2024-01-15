using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    /// <summary>
    /// Enum list of the transaction types
    /// </summary>
    public enum ETransaction
    {
        /// <summary>
        /// When the player converts physical money to virtual one. 
        /// <br/>from <see cref="EPaymentMethod.EXPERIENCE"/> or <see cref="EPaymentMethod.CASH"/>
        /// <br/>to 
        /// <br/><see cref="EPaymentMethod.BANK_ACCOUNT"/> or <see cref="EPaymentMethod.CRYPTO_WALLET"/>
        /// </summary>
        DEPOSIT,
        /// <summary>
        /// When the player 
        /// </summary>
        WITHDRAW,
        REFUND,
        SALE,
        PURCHASE,
        PAYMENT
    }
}
