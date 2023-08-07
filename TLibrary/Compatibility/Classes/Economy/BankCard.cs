using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    [Serializable]
    public class BankCard
    {
        public string CardID { get; set; }
        public string CardPinCode { get; set; }
        public ulong CardOwnerID { get; set; }
        public decimal CardBalance { get; set; }
        public decimal MaxCardBalance { get; set; }
        public bool IsActive { get; set; }
        public bool IsInATM { get; set; }
        public List<Transaction> Transactions { get; set; }
        public DateTime ExpireDate { get; set; }

        public BankCard(string code, string pincode, ulong owner, decimal balance, decimal maxbalance, DateTime expireDate)
        {
            CardID = code;
            CardPinCode = pincode;
            CardOwnerID = owner;
            CardBalance = balance;
            MaxCardBalance = maxbalance;
            ExpireDate = expireDate;
            IsInATM = false;
            IsActive = false;
            Transactions = new List<Transaction>();
        }

        public BankCard(string code, string pincode, ulong owner, decimal balance, decimal maxbalance, DateTime expireDate, bool isactive, bool isinatm, List<Transaction> transactions)
        {
            CardID = code;
            CardPinCode = pincode;
            CardOwnerID = owner;
            CardBalance = balance;
            MaxCardBalance = maxbalance;
            ExpireDate = expireDate;
            IsInATM = isinatm;
            IsActive = isactive;
            Transactions = transactions;
        }

        public BankCard() { }

        public void UpdateDetails(BankCardDetails details)
        {
            if (details.CardBalance >= 0)
                CardBalance = details.CardBalance;
            IsActive = details.IsActive;
            IsInATM = details.IsInATM;
            if (details.Transactions.Count >= 0 && details.Transactions != null)
                Transactions = details.Transactions;
        }
    }
}
