using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Economy
{
    /// <summary>
    /// Represents a bank card used in the economy system.
    /// Extends <see cref="IEconomyCard"/> with bank-specific properties.
    /// </summary>
    public interface IBankCard : IEconomyCard
    {
        /// <summary>
        /// The PIN code of the bank card.
        /// </summary>
        [JsonProperty("PinCode")]
        string PinCode { get; set; }
        
        /// <summary>
        /// The amount of balance that has been used.
        /// </summary>
        [JsonProperty("BalanceUsed")]
        decimal BalanceUsed { get; set; }
        
        /// <summary>
        /// The maximum balance the card can hold.
        /// </summary>
        [JsonProperty("BalanceLimit")]
        decimal BalanceLimit { get; set; }
        
        /// <summary>
        /// Indicates whether the card is active.
        /// </summary>
        [JsonProperty("IsActive")]
        bool IsActive { get; set; }
    }
}
