using System;

namespace Tavstal.TLibrary.Models.Economy
{
    public interface ITransaction
    {
        Guid Id { get; set; }
        ETransaction Type { get; set; }
        EPaymentMethod PaymentMethod { get; set; }
        string StoreName { get; set; }
        ulong PayerId { get; set; }
        ulong PayeeId { get; set; }
        decimal Amount { get; set; }
        DateTime Date { get; set; }
    }
}
