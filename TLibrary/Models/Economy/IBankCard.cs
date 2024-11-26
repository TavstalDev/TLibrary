using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Economy
{
    public interface IBankCard : IEconomyCard
    {
        [JsonProperty("PinCode")]
        string PinCode { get; set; }
        [JsonProperty("BalanceUsed")]
        decimal BalanceUsed { get; set; }
        [JsonProperty("BalanceLimit")]
        decimal BalanceLimit { get; set; }
        [JsonProperty("IsActive")]
        bool IsActive { get; set; }
    }
}
