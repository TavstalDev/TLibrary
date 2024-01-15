using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    /// <summary>
    /// Transaction
    /// </summary>
    [Serializable]
    public class Transaction
    {
        /// <summary>
        /// Type of the transaction to check the transaction was sent or recieved by the payee
        /// <br/>For values check <see cref="ETransaction"/>
        /// </summary>
        public ETransaction Type { get; set; }
        /// <summary>
        /// Type of the payment method, used to check the type of the money
        /// </summary>
        public EPaymentMethod PaymentMethod { get; set; }
        /// <summary>
        /// Name of the store, can be used as displayName if the transaction was made between a player and a plugin, like TShop
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// SteamId of the payer, the player who sends the money
        /// </summary>
        public ulong PayerId { get; set; }
        /// <summary>
        /// SteamId of the payer, the player who recieves the money
        /// </summary>
        public ulong PayeeId { get; set; }
        /// <summary>
        /// Amout of the transaction
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Date when the transaction was created
        /// </summary>
        public DateTime TransactionDate { get; set; }

        public Transaction(ETransaction type, EPaymentMethod method, string storename, ulong payer, ulong payee, decimal amount, DateTime date) 
        { 
            Type = type; 
            PaymentMethod = method; 
            StoreName = storename; 
            PayerId = payer; 
            PayeeId = payee;
            Amount = amount; 
            TransactionDate = date; 
        }

        public Transaction() { }
    }
}
