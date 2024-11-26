using System;
using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Economy
{
    public interface IEconomyCard
    {
        [JsonProperty("Iban")]
        string Iban  { get; set; }
        [JsonProperty("Number")]
        string Number  { get; set; }
        [JsonProperty("Cvc")]
        string Cvc { get; set; }
        [JsonProperty("HolderId")]
        ulong HolderId { get; set; }
        [JsonProperty("ExpireDate")]
        DateTime ExpireDate { get; set; }
    }
}
