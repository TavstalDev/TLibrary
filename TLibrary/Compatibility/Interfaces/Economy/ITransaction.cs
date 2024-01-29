using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Database;
using Tavstal.TLibrary.Compatibility.Economy;

namespace Tavstal.TLibrary.Compatibility.Interfaces.Economy
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
