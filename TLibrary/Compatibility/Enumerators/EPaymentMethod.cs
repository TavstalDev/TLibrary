namespace Tavstal.TLibrary.Compatibility.Enumerators
{
    /// <summary>
    /// Enum list of the payment methods
    /// </summary>
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
