using System;

namespace Tavstal.TLibrary.Compatibility.Interfaces.Economy
{
    public interface IEconomyCard
    {
        string Id { get; set; }
        ulong HolderId { get; set; }
        string SecurityCode { get; set; }
        DateTime ExpireDate { get; set; }
    }
}
