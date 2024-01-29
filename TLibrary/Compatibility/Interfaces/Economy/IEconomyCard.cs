using System;

namespace Tavstal.TLibrary.Compatibility.Interfaces.Economy
{
    public interface IEconomyCard
    {
        string Id { get; set; }
        ulong HolderId { get; set; }
        string SecurityCode { get; set; }
        string PinCode { get; set; }
        decimal BalanceUse { get; set; }
        decimal BalanceLimit { get; set; }
        bool IsActive { get; set; }
        DateTime ExpireDate { get; set; }
    }
}
