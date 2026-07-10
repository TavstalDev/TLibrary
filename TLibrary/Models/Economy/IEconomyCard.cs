using System;
using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Economy
{
    /// <summary>
    /// Represents an economy card with basic card information.
    /// </summary>
    public interface IEconomyCard
    {
        /// <summary>
        /// The IBAN (international bank account number) of the card.
        /// </summary>
        [JsonProperty("Iban")]
        string Iban  { get; set; }
        
        /// <summary>
        /// The card number.
        /// </summary>
        [JsonProperty("Number")]
        string Number  { get; set; }
        
        /// <summary>
        /// The CVC (Card Verification Code) of the card.
        /// </summary>
        [JsonProperty("Cvc")]
        string Cvc { get; set; }
        
        /// <summary>
        /// The unique ID of the cardholder.
        /// </summary>
        [JsonProperty("HolderId")]
        ulong HolderId { get; set; }
        
        /// <summary>
        /// The date when the card expires.
        /// </summary>
        [JsonProperty("ExpireDate")]
        DateTime ExpireDate { get; set; }
    }
}
