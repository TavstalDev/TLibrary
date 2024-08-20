using System;

namespace Tavstal.TLibrary.Models.Economy
{
    public interface IEconomyCard
    {
        string Id { get; set; }
        ulong HolderId { get; set; }
        string SecurityCode { get; set; }
        DateTime ExpireDate { get; set; }
    }
}
