using System;

namespace Tavstal.TLibrary.Models.Economy
{
    /// <summary>
    /// Represents a financial transaction in the economy system.
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// The unique identifier of the transaction.
        /// </summary>
        string Id { get; set; }
        
        /// <summary>
        /// The type of transaction (e.g. deposit, withdraw, purchase).
        /// </summary>
        ETransaction Type { get; set; }
        
        /// <summary>
        /// The payment method used (e.g. cash, card, bank).
        /// </summary>
        EPaymentMethod PaymentMethod { get; set; }
        
        /// <summary>
        /// The name of the store where the transaction happened.
        /// </summary>
        string StoreName { get; set; }
        
        /// <summary>
        /// The unique ID of the person who paid.
        /// </summary>
        ulong PayerId { get; set; }
        
        /// <summary>
        /// The unique ID of the person who received the payment.
        /// </summary>
        ulong PayeeId { get; set; }
        
        /// <summary>
        /// The amount of money involved in the transaction.
        /// </summary>
        decimal Amount { get; set; }
        
        /// <summary>
        /// The date and time when the transaction occurred.
        /// </summary>
        DateTime Date { get; set; }
    }
}
