using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    [Serializable]
    public class BankCardDetails
    {
        public decimal CardBalance { get; set; }
        public bool IsActive { get; set; }
        public bool IsInATM { get; set; }
        public List<Transaction> Transactions { get; set; }

        public BankCardDetails(BankCard card)
        {
            CardBalance = card.CardBalance;
            IsActive = card.IsActive;
            IsInATM = card.IsInATM;
            Transactions = card.Transactions;
        }

        public BankCardDetails(decimal balance)
        {
            CardBalance = balance;
            IsInATM = false;
            Transactions = new List<Transaction>();
        }

        public BankCardDetails(decimal balance, bool isactive, bool isinatm, List<Transaction> transactions)
        {
            CardBalance = balance;
            IsInATM = isinatm;
            IsActive = isactive;
            Transactions = transactions;
        }

        public BankCardDetails() { }
    }
}
