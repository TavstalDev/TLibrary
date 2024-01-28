using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    /// <summary>
    /// Bank card details class used to make updating BankCards more maintanable
    /// </summary>
    [Serializable]
    public class BankCardDetails
    {
        /// <summary>
        /// Balance of the card
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Is the card active or not
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Is the card in an ATM or not (If it's true then the card shouldn't be able to be used outside of that specific ATM)
        /// </summary>
        public bool IsInATM { get; set; }
        /// <summary>
        /// Transactions list of the card
        /// </summary>
        public List<Transaction> Transactions { get; set; }

        public BankCardDetails(BankCard card, List<Transaction> transactions)
        {
            Balance = card.Balance;
            IsActive = card.IsActive;
            IsInATM = card.IsInATM;
            Transactions = transactions;
        }

        public BankCardDetails(decimal balance, List<Transaction> transactions)
        {
            Balance = balance;
            IsInATM = false;
            Transactions = transactions;
        }

        public BankCardDetails(decimal balance, bool isactive, bool isinatm, List<Transaction> transactions)
        {
            Balance = balance;
            IsInATM = isinatm;
            IsActive = isactive;
            Transactions = transactions;
        }

        public BankCardDetails() { }
    }
}
