namespace Tavstal.TLibrary.Models.Economy
{
    public interface IBankCard : IEconomyCard
    {
        string PinCode { get; set; }
        decimal BalanceUse { get; set; }
        decimal BalanceLimit { get; set; }
        bool IsActive { get; set; }
    }
}
