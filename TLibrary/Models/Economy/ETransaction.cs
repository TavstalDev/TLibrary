namespace Tavstal.TLibrary.Models.Economy
{
    /// <summary>
    /// Enum list of the transaction types
    /// </summary>
    public enum ETransaction
    {
        /// <summary>
        /// When the player converts physical money to digital one. 
        /// <br/>from <see cref="EPaymentMethod.EXPERIENCE"/> or <see cref="EPaymentMethod.CASH"/>
        /// <br/>to <see cref="EPaymentMethod.BANK_ACCOUNT"/> or <see cref="EPaymentMethod.CRYPTO_WALLET"/>
        /// </summary>
        DEPOSIT,
        /// <summary>
        /// When the player converts digital money to physical one
        /// <br/>from <see cref="EPaymentMethod.BANK_ACCOUNT"/> or <see cref="EPaymentMethod.CRYPTO_WALLET"/>
        /// <br/>to <see cref="EPaymentMethod.EXPERIENCE"/> or <see cref="EPaymentMethod.CASH"/>
        /// </summary>
        WITHDRAW,
        /// <summary>
        /// When the player recieved the money as refund of a purchase
        /// </summary>
        REFUND,
        /// <summary>
        /// When the player sold something
        /// </summary>
        SALE,
        /// <summary>
        /// When the player purchased something
        /// </summary>
        PURCHASE,
        /// <summary>
        /// When the player recieved the money as wage
        /// </summary>
        PAYMENT
    }
}
